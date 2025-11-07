using Microsoft.AspNetCore.Mvc;
using static Models.Dto.Requests.MailHunterRequest;
using static Models.Dto.Responses.MailHunterResponse;

namespace WebAPi.Controllers
{
    public partial class MailHunterController : BaseController
    {
        /// <summary>
        /// 匯出 查詢專案發送數量
        /// </summary>
        /// <param name="req">前端傳入的查詢條件</param>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="cancellationToken">取消非同步</param>
        /// <returns name="result">查詢結果 </returns>
        [Tags("MailHunter")]  //分組(可多標籤)        
        [HttpPost("ExportProjectMailCountCSV")]
        public async Task<IActionResult> ExportProjectMailCountCSV([FromQuery] string fileName, MailHunterSearchListRequest req, CancellationToken cancellationToken = default)
        {
            #region 參數宣告
            var data = new MailHunterSearchListResponse();
            // fileName = "專案寄件數";
            #endregion

            #region 流程

            data = await _mailHunterService.GetProjectMailCountList(req, cancellationToken).ConfigureAwait(false);

            var files = await _fileService.GenerateCsvStreamsAsync(fileName, data.SearchItem, cancellationToken).ConfigureAwait(false);

            if (files.Count == 1)
            {
                var (stream, export_fileName) = files.First();
                return File(stream, "text/csv", export_fileName);
            }
            else
            {
                var zipStream = await _zipService.GenerateZipStreamAsync(files, cancellationToken).ConfigureAwait(false);
                return File(zipStream, "application/zip", fileName);
            }

            #endregion

        }
    }
}
