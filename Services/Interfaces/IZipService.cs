namespace Services.Interfaces
{
    public interface IZipService
    {
        /// <summary>
        /// 將多個檔案壓縮成 Zip 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Stream> GenerateZipStreamAsync(List<(Stream Stream, string FileName)> files, CancellationToken cancellationToken);
    }
}
