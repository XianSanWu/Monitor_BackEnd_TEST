using AutoMapper;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using System.IO.Compression;

namespace Services.Implementations
{
    public class ZipService(
        ILogger<ZipService> logger,
        IMapper mapper
            ) : IZipService
    {
        private ILogger<ZipService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// 將多個檔案壓縮成 Zip 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Stream> GenerateZipStreamAsync(List<(Stream Stream, string FileName)> files, CancellationToken cancellationToken)
        {
            var memoryStream = new MemoryStream();

            // 使用 ZipArchive 來生成 ZIP 檔案
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    // 為每個檔案創建一個條目
                    var zipEntry = zipArchive.CreateEntry(file.FileName);

                    using (var entryStream = zipEntry.Open())
                    {
                        // 將檔案內容複製到 ZIP 條目中
                        await file.Stream.CopyToAsync(entryStream, cancellationToken);
                    }
                }
            }

            // 重設流位置，以便下載
            memoryStream.Position = 0;

            return memoryStream;
        }

    }
}
