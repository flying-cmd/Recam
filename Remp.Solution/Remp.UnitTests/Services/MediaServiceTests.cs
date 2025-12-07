using FluentAssertions;
using Moq;
using Remp.Common.Exceptions;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Service.Interfaces;
using Remp.Service.Services;

namespace Remp.UnitTests.Services;

public class MediaServiceTests
{
    private readonly Mock<IMediaRepository> _mediaRepositoryMock;
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
    private readonly Mock<ILoggerService> _loggerServiceMock;
    private IMediaService mediaService;

    public MediaServiceTests()
    {
        _mediaRepositoryMock = new Mock<IMediaRepository>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _loggerServiceMock = new Mock<ILoggerService>();
        mediaService = new MediaService(_mediaRepositoryMock.Object, _blobStorageServiceMock.Object, _loggerServiceMock.Object);
    }

    [Fact]
    public async Task DeleteMediaByIdAsync_WhenMediaDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var mediaId = 1;
        var userId = "1";
        _mediaRepositoryMock.Setup(r => r.FindMediaByIdAsync(mediaId)).ReturnsAsync((MediaAsset?)null);
        _loggerServiceMock.Setup(l => l.LogDeleteMedia(null, userId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await mediaService.DeleteMediaByIdAsync(mediaId, userId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _mediaRepositoryMock.Verify(r => r.FindMediaByIdAsync(mediaId), Times.Once);
        _loggerServiceMock.Verify(l => l.LogDeleteMedia(null, userId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMediaByIdAsync_WhenUserIdIsNotOwner_ShouldThrowForbiddenException()
    {
        var mediaId = 1;
        var userId = "1";
        var media = new MediaAsset { UserId = "2" };
        _mediaRepositoryMock.Setup(r => r.FindMediaByIdAsync(mediaId)).ReturnsAsync(media);
        _loggerServiceMock.Setup(l => l.LogDeleteMedia(mediaId.ToString(), userId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await mediaService.DeleteMediaByIdAsync(mediaId, userId);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _mediaRepositoryMock.Verify(r => r.FindMediaByIdAsync(mediaId), Times.Once);
        _loggerServiceMock.Verify(l => l.LogDeleteMedia(mediaId.ToString(), userId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMediaByIdAsync_WhenUserIdIsOwner_ShouldDeleteFile()
    {
        var mediaId = 1;
        var userId = "1";
        var media = new MediaAsset { UserId = userId, MediaUrl = "image_url.jpg" };
        _mediaRepositoryMock.Setup(r => r.FindMediaByIdAsync(mediaId)).ReturnsAsync(media);
        
        _blobStorageServiceMock.Setup(b => b.DeleteFileAsync(media.MediaUrl)).Returns(Task.CompletedTask);

        _mediaRepositoryMock.Setup(r => r.DeleteMediaByIdAsync(media)).Returns(Task.CompletedTask);
        
        _loggerServiceMock.Setup(l => l.LogDeleteMedia(mediaId.ToString(), userId, It.IsAny<string>(), It.IsAny<string>(), null)).Returns(Task.CompletedTask);

        // Act
        await mediaService.DeleteMediaByIdAsync(mediaId, userId);

        // Assert
        _mediaRepositoryMock.Verify(r => r.FindMediaByIdAsync(mediaId), Times.Once);
        _blobStorageServiceMock.Verify(b => b.DeleteFileAsync(media.MediaUrl), Times.Once);
        _mediaRepositoryMock.Verify(r => r.DeleteMediaByIdAsync(media), Times.Once);
        _loggerServiceMock.Verify(l => l.LogDeleteMedia(mediaId.ToString(), userId, It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task DownloadMediaByIdAsync_WhenMediaDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var mediaId = 1;
        _mediaRepositoryMock.Setup(r => r.FindMediaByIdAsync(mediaId)).ReturnsAsync((MediaAsset?)null);

        // Act
        var act = async () => await mediaService.DownloadMediaByIdAsync(mediaId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _mediaRepositoryMock.Verify(r => r.FindMediaByIdAsync(mediaId), Times.Once);
        _blobStorageServiceMock.Verify(b => b.DownloadFileAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DownloadMediaByIdAsync_WhenMediaExists_ShouldDownloadFile()
    {
        // Arrange
        var mediaId = 1;
        var mediaUrl = "http://blob/image1.jpg";
        var media = new MediaAsset { MediaUrl = mediaUrl };
        _mediaRepositoryMock.Setup(r => r.FindMediaByIdAsync(mediaId)).ReturnsAsync(media);

        var fileBytes = new byte[] { 1, 2, 3 };
        var fileStream = new MemoryStream(fileBytes);
        var contentType = "image/jpg";
        var fileName = "image1.jpg";
        _blobStorageServiceMock
            .Setup(b => b.DownloadFileAsync(media.MediaUrl))
            .ReturnsAsync((Content: fileStream, ContentType: contentType, FileName: fileName));

        // Act
        var (resultFileStream, resultContentType, resultFileName) = await mediaService.DownloadMediaByIdAsync(mediaId);

        // Assert
        using var memoryStream = new MemoryStream();
        await resultFileStream.CopyToAsync(memoryStream);
        memoryStream.ToArray().Should().BeEquivalentTo(fileBytes);
        resultContentType.Should().Be(contentType);
        resultFileName.Should().Be(fileName);

        _mediaRepositoryMock.Verify(r => r.FindMediaByIdAsync(mediaId), Times.Once);
        _blobStorageServiceMock.Verify(b => b.DownloadFileAsync(media.MediaUrl), Times.Once);
    }
}
