using AutoMapper;
using cuteDVDCore.Models;
using cuteDVDCore.Services.Interfaces;
using cuteDVDNet.Models;
using cuteDVDNet.Models.RDTO;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.VisualBasic;

namespace cuteDVDNet.Services
{
    public class NetDriveFileService(IDriveFileService driveService, ILogger<NetDriveFileService> logger, IMapper mapper) :  INetDriveFileService
    {
        private readonly IDriveFileService driveService = driveService;
        private readonly ILogger<NetDriveFileService> logger = logger;
        private readonly IMapper mapper = mapper;

        public async Task<IEnumerable<RDTOFileFromCard>?> GetCardFiles()
        {
           
            List<RDTOFileFromCard> MapFiles = new List<RDTOFileFromCard>();
            await foreach (var item in driveService.GetListFiles())
            {
                MapFiles.Add(Map(item));
            }
            return MapFiles;
        }

        public async Task<IEnumerable<ModelDVDFiles>?> GetFilesList()
        {
            List<ModelDVDFiles> files = new();
            await foreach (var item in driveService.GetListFiles())
            {
                files.Add(item);
            }
            return files;
        }

        public async Task<Stream?> StreamFileAsync(string Name)
        {
            var file = await driveService.GetFileStream(Name);
            if (file is null) return null;
            return file;
        }

        public async Task<Stream?> StreamFileAsync(DTOFileSearch DTOFile)
        {
            if (DTOFile is null) return null;
            ModelDVDFiles modelDVDFiles = new(Name: DTOFile.name, default, default, DTOFile.path);
            var file = await driveService.GetFileStream(modelDVDFiles);
            if (file is null) return null;
            return file;

        }

        public async IAsyncEnumerable<RDTOAudioCD> GetRDTOAudiosAsync() {
            await foreach (var item in driveService.GetListFiles())
            {
                if (item.Type != ".mp3") continue;
                using var file = TagLib.File.Create(item.Path);

                yield return new RDTOAudioCD
                (
                    Name: file.Tag.Title ?? "N/A",
                    Type:  item.Type,
                    Artist:  file.Tag.Artists is { Length: > 0 } ? string.Join(", ",file.Tag.Artists) : "N?A",
                    Album: file.Tag.Album ?? "N/A",
                    Duration:  file.Properties.Duration.ToString(@"mm\:ss"),
                    FileName: item.Name,
                    NumberInAlbum:  Convert.ToInt16(file.Tag.Track),
                    Path: item.Path 
                );

                await Task.Yield();
            }
        }

        private RDTOFileFromCard Map(ModelDVDFiles model) => mapper.Map<RDTOFileFromCard>(model);
    }
}
