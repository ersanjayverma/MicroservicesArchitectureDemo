using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileService.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<byte[]> DownloadFileAsync(string fileName);
    }
}
