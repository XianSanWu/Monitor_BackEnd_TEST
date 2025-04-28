
namespace Services.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// 產出CSV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <param name="_config"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<(Stream Stream, string FileName)>> GenerateCsvStreamsAsync<T>(string fileName, IEnumerable<T> data, CancellationToken cancellationToken = default) where T : class;

    }
}
