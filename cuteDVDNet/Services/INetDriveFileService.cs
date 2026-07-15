using cuteDVDCore.Models;
using cuteDVDNet.Models;
using cuteDVDNet.Models.RDTO;

namespace cuteDVDNet.Services
{
    public interface INetDriveFileService
    {
        Task<IEnumerable<RDTOFileFromCard>?> GetCardFiles();
        Task<IEnumerable<ModelDVDFiles>?> GetFilesList();
        Task<Stream?> StreamFileAsync(DTOFileSearch DTOFile);
        Task<Stream?> StreamFileAsync(string Name);
        IAsyncEnumerable<RDTOAudioCD> GetRDTOAudiosAsync();
    }
}