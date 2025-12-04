using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Remp.Service.Services;
using System.Text;

namespace Remp.UnitTests.Services;

public class BlobStorageServiceTests
{
    private readonly Mock<BlobServiceClient> _blobServiceClientMock;
    private readonly string _containerName = "test-container";
    private readonly BlobStorageService _blobStorageService;

    public BlobStorageServiceTests()
    {
        _blobServiceClientMock = new Mock<BlobServiceClient>();

        // in-memory configuration
        var inMemorySetting = new Dictionary<string, string>
        {
            {
                "AzureBlobStorage:ContainerName", _containerName
            }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySetting!)
            .Build();

        _blobStorageService = new BlobStorageService(_blobServiceClientMock.Object, configuration);
    }

    [Fact]
    public async Task UploadFileAsync_WhenPassAFile_ShouldUploadToBlobAndReturnUrl()
    {
        // Arrange
        var formFileMock = new Mock<IFormFile>();
        var fileContent = "test file content";
        var fileName = "avatar.jpg";

        var containerClientMock = new Mock<BlobContainerClient>();
        var blobClientMock = new Mock<BlobClient>();

        _blobServiceClientMock
            .Setup(s => s.GetBlobContainerClient(_containerName))
            .Returns(containerClientMock.Object);

        formFileMock.Setup(f => f.FileName).Returns(fileName);

        // Capture the blob name used
        string? capturedBlobName = null;

        containerClientMock
            .Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Returns<string>(name =>
            {
                capturedBlobName = name; // Capture the actual blob name
                return blobClientMock.Object;
            });

        // Mock the file stream
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        formFileMock.Setup(f => f.OpenReadStream()).Returns(fileStream);

        // Don't care about the contents, just need it to be a valid Azure response so the await works
        // Return a fake Response<BlobContentInfo> created with BlobsModelFactory.BlobContentInfo and Response.FromValue
        blobClientMock
            .Setup(b => b.UploadAsync(
                It.IsAny<Stream>(),
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(
                BlobsModelFactory.BlobContentInfo(
                    eTag: new ETag("\"0x0\""),
                    lastModified: DateTimeOffset.UtcNow,
                    contentHash: null,
                    encryptionKeySha256: null,
                    encryptionScope: null,
                    blobSequenceNumber: 0),
                Mock.Of<Response>()));

        var expectedUri = new Uri("https://test-container/test_url.jpg");
        blobClientMock
            .Setup(b => b.Uri)
            .Returns(expectedUri);

        // Act
        var resultUrl = await _blobStorageService.UploadFileAsync(formFileMock.Object);

        // Assert
        resultUrl.Should().Be(expectedUri.ToString());

        _blobServiceClientMock.Verify(b => b.GetBlobContainerClient(_containerName), Times.Once);

        containerClientMock.Verify(c => c.GetBlobClient(It.IsAny<string>()), Times.Once);

        // Blob file name should end with original extension
        capturedBlobName.Should().NotBeNull();
        Path.GetExtension(capturedBlobName).Should().Be(".jpg");

        blobClientMock
            .Verify(b => b.UploadAsync(
                    It.IsAny<Stream>(),
                    true,
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DownloadFileAsync_WhenPassBlobUrl_ShouldReturnContentAndContentTypeAndFileName()
    {
        // Arrange
        var blobUrl = "https://test-container/test_url.jpg";
        var expectedFileName = "test_url.jpg";
        var expectedContentType = "image/jpg";
        var fileContent = "test file content";

        var containerClientMock = new Mock<BlobContainerClient>();
        var blobClientMock = new Mock<BlobClient>();

        _blobServiceClientMock
            .Setup(s => s.GetBlobContainerClient(_containerName))
            .Returns(containerClientMock.Object);

        containerClientMock
            .Setup(c => c.GetBlobClient(expectedFileName))
            .Returns(blobClientMock.Object);

        var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        // BlobsModelFactory.BlobDownloadStreamingResult is a helper to create a BlobDownloadStreamingResult object
        // Pass the stream as the Content that represents the blob’s data
        // Mimic what Azure would normally give you when you call DownloadStreamingAsync
        var downloadResult = BlobsModelFactory.BlobDownloadStreamingResult(contentStream);

        // DownloadStreamingAsync returns Response<BlobDownloadStreamingResult>
        // Response.FromValue wraps downloadResult into a fake Response<T> instance
        // Now downloadResponse behaves like: Azure responded successfully, with downloadResult as the value
        var downloadResponse = Response.FromValue(downloadResult, Mock.Of<Response>());

        blobClientMock
            .Setup(b => b.DownloadStreamingAsync(It.IsAny<BlobDownloadOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(downloadResponse);

        // Use BlobsModelFactory.BlobProperties to create a fake BlobProperties object
        var blobProperties = BlobsModelFactory.BlobProperties(
            contentType: expectedContentType
            );

        // Wrap blobProperties into a Response<BlobProperties>
        var propertiesResponse = Response.FromValue(blobProperties, Mock.Of<Response>());

        blobClientMock
            .Setup(b => b.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propertiesResponse);

        // Act
        var (content, contentType, fileName) = await _blobStorageService.DownloadFileAsync(blobUrl);

        // Assert
        // Wrap the returned Stream content in a StreamReader
        using var streamReader = new StreamReader(content);
        // Read all the bytes and converts them to a string
        var downloadedContent = await streamReader.ReadToEndAsync();
        // Verify the actual content
        downloadedContent.Should().Be(fileContent);
        contentType.Should().Be(expectedContentType);
        fileName.Should().Be(expectedFileName);

        _blobServiceClientMock.Verify(b => b.GetBlobContainerClient(_containerName), Times.Once);
        containerClientMock.Verify(c => c.GetBlobClient(expectedFileName), Times.Once);
        blobClientMock.Verify(b => b.DownloadStreamingAsync(It.IsAny<BlobDownloadOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        blobClientMock.Verify(b => b.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
