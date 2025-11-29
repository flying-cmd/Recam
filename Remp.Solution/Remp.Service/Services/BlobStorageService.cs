using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration.GetSection("AzureBlobStorage")["ContainerName"]!;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        
        // Generate a unique name for the file
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        var blobClient = containerClient.GetBlobClient(fileName);
        
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, true);
        
        return blobClient.Uri.ToString();
    }
}
