using Microsoft.AspNetCore.Http;

namespace Remp.Service.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(IFormFile file);
}
