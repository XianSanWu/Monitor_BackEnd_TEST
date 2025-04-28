using Microsoft.AspNetCore.Mvc;

namespace WebAPi.Controllers
{
    public partial class ZipController : BaseController
    {
        /// <summary>
        /// ZIP測試
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="files"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Tags("zip")]  //分組(可多標籤)        
        [HttpPost("download-zip")]
        public async Task<IActionResult> DownloadZip(string fileName, List<IFormFile> files, CancellationToken cancellationToken = default)
        {
            var fileStreams = new List<(Stream Stream, string FileName)>();

            foreach (var file in files)
            {
                var fileStream = file.OpenReadStream();
                fileStreams.Add((fileStream, file.FileName));
            }

            // Generate the zip stream using the ZipService
            var zipStream = await _zipService.GenerateZipStreamAsync(fileStreams, cancellationToken);

            // Return the zip file as a download
            return File(zipStream, "application/zip", fileName ?? "files.zip");
        }
    }
}
