using Microsoft.AspNetCore.Mvc;

namespace WebAPi.Controllers
{
    public partial class FileController : BaseController
    {
        /// <summary>
        /// 產出CSV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("File")]  //分組(可多標籤)        
        [HttpPost("ExportCsv")]
        public async Task<IActionResult> ExportCsv<T>(string fileName, IEnumerable<T> data, CancellationToken cancellationToken) where T : class
        {
            #region 參數宣告
            #endregion

            #region 流程

            var files = await _fileService.GenerateCsvStreamsAsync(fileName, data, cancellationToken);

            if (files.Count == 1)
            {
                var (stream, export_fileName) = files.First();
                return File(stream, "text/csv", export_fileName);
            }
            else
            {
                // 3. 使用 ZipService 來產生 ZIP 檔案流
                var zipStream = await _zipService.GenerateZipStreamAsync(files, cancellationToken).ConfigureAwait(false);

                return File(zipStream, "application/zip", fileName);
            }

            #endregion

        }

    }
}
