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
        public async Task<List<(Stream Stream, string FileName)>> GenerateCsvStreamsAsync<T>(string fileName, IEnumerable<T> data, CancellationToken cancellationToken = default) where T : class
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
            var currentStream = new MemoryStream();
            var writer = new StreamWriter(currentStream, Encoding.UTF8);

            // 寫入標頭列
            await writer.WriteLineAsync(string.Join(",", headers));

            long currentSize = currentStream.Length;

            foreach (var item in data)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item)?.ToString() ?? string.Empty;
                    if (value.Contains(",") || value.Contains("\""))
                    {
                        value = $"\"{value.Replace("\"", "\"\"")}\""; // CSV的轉義
                    }
                    return value;
                });

                var line = string.Join(",", values);
                await writer.WriteLineAsync(line);

                await writer.FlushAsync();
                currentSize = currentStream.Length;

                // 如果超過限制，先關掉這個檔案，開始下一個
                if (currentSize >= maxFileSizeBytes)
                {
                    await writer.FlushAsync();
                    currentStream.Position = 0;

                    streams.Add((currentStream, $"{fileName}_Export_{DateTime.UtcNow:yyyyMMddHHmmss}_{fileIndex}.csv"));

                    fileIndex++;

                    // 開新的流
                    currentStream = new MemoryStream();
                    writer = new StreamWriter(currentStream, Encoding.UTF8);

                    // 寫標頭
                    await writer.WriteLineAsync(string.Join(",", headers));
                }
            }

            // 最後一個流
            await writer.FlushAsync();
            currentStream.Position = 0;

            if (currentStream.Length > 0)
            {
                streams.Add((currentStream, $"{fileName}_Export_{DateTime.UtcNow:yyyyMMddHHmmss}_{fileIndex}.csv"));
            }

            return streams;
        }


    }
}
