using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using static Models.Dto.Requests.MsmqQueueRequest;
using static Models.Dto.Responses.MsmqQueueResponse;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Services.Implementations
{
    public class MsmqService(
        ILogger<MsmqService> logger
        ) : IMsmqService
    {
        private ILogger<MsmqService> _logger = logger;

        /// <summary>
        /// 取得全部MSMQ佇列訊息
        /// </summary>
        /// <param name="searchReq"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MsmqQueueDetailsResponse> GetAllQueueInfo(MsmqQueueInfoRequest searchReq, IConfiguration _config, CancellationToken cancellationToken = default)
        {
            var result = new MsmqQueueDetailsResponse();

            // 固定 PowerShell 腳本，不使用查詢條件
            string powershellScript = @"
# 獲取所有佇列
$queues = Get-WmiObject -Namespace 'root\cimv2' -Class 'Win32_PerfFormattedData_msmq_MSMQQueue' | Select-Object *

$result = @()

foreach ($q in $queues) {
    $result += [PSCustomObject]@{
        QueueName = $q.Name
        MessagesInQueue = $q.MessagesInQueue
        BytesInQueue = $q.BytesInQueue
        BytesinJournalQueue = $q.BytesinJournalQueue
        MessagesInJournalQueue = $q.MessagesinJournalQueue
        PSComputerName = $q.PSComputerName  
    }
}

# 格式化結果為 JSON
$result = @{
    Value = $result
    Count = $result.Count # 計算總佇列數量
}

# 返回 JSON 格式的結果
$result | ConvertTo-Json -Depth 5
";

            // 建立 PowerShell Process
            var psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-Command \"{powershellScript.Replace("\"", "`\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                _logger.LogError("PowerShell 執行失敗：Process 為 null");
                return result;
            }

            string output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            string error = await process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode != 0)
            {
                _logger.LogError($"PowerShell 錯誤輸出：{error}");
                return result;
            }

            if (string.IsNullOrWhiteSpace(output))
            {
                _logger.LogError("PowerShell 輸出為空，無法反序列化");
                return result;
            }

            var queues = JsonSerializer.Deserialize<MsmqQueueDetailsResponse>(output);

            if (queues != null && queues.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(searchReq.QueueName))
                {
                    queues.Value = [.. queues.Value.Select(s => s).Where(w => (w.QueueName).Contains(searchReq.QueueName ?? ""))];
                    queues.Count = queues.Value.Count();
                }
            }

            result = queues ?? new MsmqQueueDetailsResponse();
            return result;
        }



    }
}
