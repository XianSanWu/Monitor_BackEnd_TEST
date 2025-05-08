using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services.Implementations
{
    public class FileService(
        ILogger<FileService> logger,
         IConfiguration config,
        IMapper mapper
            ) : IFileService
    {
        private readonly ILogger<FileService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _config = config;

        /// <summary>
        /// 產出CSV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<(Stream Stream, string FileName)>> GenerateCsvStreamsAsync<T>(
            string fileName,
            IEnumerable<T> data,
            CancellationToken cancellationToken = default) where T : class
        {
            var maxFileSizeBytes = _config.GetValue<long>("File:MaxFileSizeBytes");
            var streams = new List<(Stream Stream, string FileName)>();
            var properties = typeof(T).GetProperties();

            // 取得標頭
            var headers = properties.Select(p =>
            {
                var displayAttr = p.GetCustomAttributes(typeof(DisplayAttribute), false)
                                   .FirstOrDefault() as DisplayAttribute;
                return displayAttr?.Name ?? p.Name;
            }).ToList();

            int fileIndex = 1;
            MemoryStream? currentStream = null;
            StreamWriter? writer = null;
            bool isFirstFile = true;

            try
            {
                foreach (var item in data)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // 如果 currentStream 尚未建立，則初始化
                    if (currentStream == null)
                    {
                        currentStream = new MemoryStream();
                        var encoding = isFirstFile
                            ? new UTF8Encoding(encoderShouldEmitUTF8Identifier: true) // 加 BOM
                            : new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

                        writer = new StreamWriter(currentStream, encoding, leaveOpen: true);
                        await writer.WriteLineAsync(string.Join(",", headers));
                        isFirstFile = false;
                    }

                    var values = properties.Select(p =>
                    {
                        var value = p.GetValue(item)?.ToString() ?? string.Empty;
                        if (value.Contains(",") || value.Contains("\""))
                        {
                            value = $"\"{value.Replace("\"", "\"\"")}\""; // CSV escape
                        }
                        return value;
                    });

                    writer = writer ?? throw new InvalidOperationException("StreamWriter is not initialized.");
                    await writer.WriteLineAsync(string.Join(",", values));
                    await writer.FlushAsync();

                    if (currentStream.Length >= maxFileSizeBytes)
                    {
                        writer.Dispose();
                        currentStream.Position = 0;
                        streams.Add((currentStream, $"{fileName}_Export_{DateTime.UtcNow:yyyyMMddHHmmss}_{fileIndex}.csv"));
                        fileIndex++;

                        currentStream = null;
                        writer = null;
                    }
                }

                // 最後剩餘的 stream（若有）
                if (currentStream != null && currentStream.Length > 0)
                {
                    writer?.Dispose();
                    currentStream.Position = 0;
                    streams.Add((currentStream, $"{fileName}_Export_{DateTime.UtcNow:yyyyMMddHHmmss}_{fileIndex}.csv"));
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"GenerateCsvStreamsAsync：EX：{ex}，EX_MSG：{ex.Message}")
                writer?.Dispose();
                currentStream?.Dispose();
                throw;
            }

            return streams;
        }



    }
}
