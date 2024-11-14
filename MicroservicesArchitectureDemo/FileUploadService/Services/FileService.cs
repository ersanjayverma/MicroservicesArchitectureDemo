using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FileService.Services
{
    public class InMemoryFileStore
    {
        private readonly Dictionary<string, byte[]> _files = new();

        public bool SaveFile(string fileName, byte[] content)
        {
            if (_files.ContainsKey(fileName)) return false;
            _files.Add(fileName, content);
            return true;
        }

        public byte[] GetFile(string fileName)
        {
            _files.TryGetValue(fileName, out var content);
            return content;
        }
    }
    public class FileService : IFileService
    {

        private readonly InMemoryFileStore _fileStore;

        public FileService( InMemoryFileStore fileStore)
        {
            _fileStore = fileStore;

        }


        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "No file uploaded.";
            }

            // Store file content in memory
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                // Store the file in the in-memory file store
                _fileStore.SaveFile(file.FileName, fileContent);
            }

            return $"File {file.FileName} uploaded successfully!";
        }
        public async Task<byte[]> DownloadFileAsync(string fileName)
        {
            return _fileStore.GetFile(fileName);
        }
    }
}
