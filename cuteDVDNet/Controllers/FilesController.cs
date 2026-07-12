using cuteDVDNet.Models;
using cuteDVDNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace cuteDVDNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(INetDriveFileService driveFileService) : ControllerBase
    {
        private readonly INetDriveFileService driveFileService = driveFileService;

        [HttpGet]
        public async Task<IActionResult> GetCardAsync() {
            var Files = await driveFileService.GetCardFiles();
            if (Files is null) return NotFound();
            return Ok(Files);
        }

        [HttpGet("/full")]
        public async Task<IActionResult> GetFilesAsync()
        {
            var Files = await driveFileService.GetFilesList();
            if (Files is null) return NotFound();
            return Ok(Files);
        }

        [HttpGet("/stream")]
        public async Task<IActionResult> GetStreamFile(string name) {
            var StreamFile = await driveFileService.StreamFileAsync(name);
            if (StreamFile is null) return NotFound();
            return File(StreamFile, "application/octet-stream", true);
        }

        [HttpGet("/dto")]
        public async Task<IActionResult> GetStreamFile(DTOFileSearch model)
        {
            var StreamFile = await driveFileService.StreamFileAsync(model);
            if (StreamFile is null) return NotFound();
            return File(StreamFile, "application/octet-stream",true);
        }
    }
}
