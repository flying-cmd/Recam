using Microsoft.AspNetCore.Http;

namespace Remp.Service.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobUrl);
    Task DeleteFileAsync(string blobUrl);
}
