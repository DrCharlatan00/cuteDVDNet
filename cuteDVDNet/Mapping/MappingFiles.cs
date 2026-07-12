using AutoMapper;
using cuteDVDCore.Models;
using cuteDVDNet.Models.RDTO;
namespace cuteDVDNet.Mapping
{
    public class MappingFiles : Profile
    {
        public MappingFiles()
        {
            CreateMap<ModelDVDFiles, RDTOFileFromCard>();
        }
    }
}
