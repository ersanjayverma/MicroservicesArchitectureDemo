using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FileService.Services;

namespace FileService.Controllers
{
    [ApiController]
    [Route("files")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Uploads a file.
        /// </summary>
        /// <param name="file">The file to be uploaded.</param>
        /// <returns>The name of the uploaded file.</returns>
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var fileName = await _fileService.UploadFileAsync(file);
            return Ok(new { fileName });
        }

        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <param name="fileName">The name of the file to be downloaded.</param>
        /// <returns>The file content as a byte array.</returns>
        [HttpGet("download/{fileName}")]
        [Authorize]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var fileContent = await _fileService.DownloadFileAsync(fileName);
            return File(fileContent, "application/octet-stream", fileName);
        }

    }
}
