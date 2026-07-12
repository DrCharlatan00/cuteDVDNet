using cuteDVDCore.Models;

namespace cuteDVDCore.Services.Interfaces
{
    public interface IDriveFileService
    {
        void CheckDisk();
        Task<Stream?> GetFileStream(ModelDVDFiles fileModel);
        Task<Stream?> GetFileStream(string fileName);
        IAsyncEnumerable<ModelDVDFiles>? GetListFiles();
    }
}