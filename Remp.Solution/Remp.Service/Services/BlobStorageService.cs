using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Search;
using Remp.Service.Interfaces;
using System.Diagnostics.Metrics;

namespace Remp.Service.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration.GetSection("BlobStorage")["ContainerName"]!;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        // Get the blob container client which lets you manage blobs (upload, download, delete, etc.) within that container
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        
        // Generate a unique name for the file
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        // Gets a BlobClient for this specific blob name inside the container.
        // BlobClient represents the individual blob (file) and is used to upload, download, retrieve properties, etc.
        var blobClient = containerClient.GetBlobClient(fileName);

        // Opens a readable stream for the uploaded file
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, true);
        
        return blobClient.Uri.ToString();
    }

    // Stream Content: the file data, string ContentType: MIME type (e.g. image/jpeg), string FileName: file name
    public async Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobUrl)
    {
        // Get the file name from the URL
        var fileName = Path.GetFileName(blobUrl);

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        // The result download contains:
        // download.Value.Content — the stream with binary data.
        // download.Value.Details — metadata like content type, length, etc
        var download = await blobClient.DownloadStreamingAsync();

        // Creates an in-memory stream where you’ll copy the downloaded content.
        var memoryStream = new MemoryStream();

        // Asynchronously copies the data from the blob’s content stream into the MemoryStream
        await download.Value.Content.CopyToAsync(memoryStream);

        // Resets the position to the beginning of the stream.
        // After copying, the position is at the end, so if you don’t reset it, reading from it later would give you an empty result.
        memoryStream.Position = 0;

        // Used to tell the browser/client what kind of file it is
        var contentType = (await blobClient.GetPropertiesAsync()).Value.ContentType;
        
        return (memoryStream, contentType, fileName);
    }

    public async Task DeleteFileAsync(string blobUrl)
    {
        var fileName = Path.GetFileName(blobUrl);

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync();
    }
}
