using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;
using Remp.Repository.Repositories;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using Remp.Service.Services;
using System.IO.Compression;

namespace Remp.UnitTests.Services;

public class ListingCaseServiceTests
{
    private readonly Mock<IListingCaseRepository> _listingCaseRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
    private readonly Mock<IMediaRepository> _mediaRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILoggerService> _loggerServiceMock;
    private readonly ListingCaseService _listingCaseServices;

    public ListingCaseServiceTests()
    {
        _listingCaseRepositoryMock = new Mock<IListingCaseRepository>();
        _mapperMock = new Mock<IMapper>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _mediaRepositoryMock = new Mock<IMediaRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerServiceMock = new Mock<ILoggerService>();
        _listingCaseServices = new ListingCaseService(
            _listingCaseRepositoryMock.Object,
            _mapperMock.Object,
            _blobStorageServiceMock.Object,
            _mediaRepositoryMock.Object,
            _configurationMock.Object,
            _loggerServiceMock.Object);
    }

    // Helper function to create a mocked form file
    private static IFormFile CreateMockedFormFile(string fileName)
    {
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.FileName).Returns(fileName);
        return formFileMock.Object;
    }

    [Fact]
    public async Task CreateCaseContactByListingCaseIdAsync_WhenListingCaseIdNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var listingCaseId = -1;
        var contactRequestDto = new CreateCaseContactRequestDto();
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync((ListingCase?)null);

        // Act
        var act = async () => await _listingCaseServices.CreateCaseContactByListingCaseIdAsync(listingCaseId, contactRequestDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task CreateCaseContactByListingCaseIdAsync_WhenListingCaseIdExist_ShouldReturnCaseContactDto()
    {
        // Arrange
        var listingCaseId = 1;
        var contactRequestDto = new CreateCaseContactRequestDto();
        var listingCase = new ListingCase { Id = listingCaseId };
        var caseContact = new CaseContact { ContactId = 1 };
        caseContact.ListingCaseId = listingCaseId;
        var expectedCaseContactDto = new CaseContactDto { ContactId = 1 };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);
        _mapperMock.Setup(m => m.Map<CaseContact>(contactRequestDto)).Returns(caseContact);
        _listingCaseRepositoryMock.Setup(r => r.AddCaseContactAsync(caseContact)).ReturnsAsync(caseContact);
        _mapperMock.Setup(m => m.Map<CaseContactDto>(caseContact)).Returns(expectedCaseContactDto);

        // Act
        var result = await _listingCaseServices.CreateCaseContactByListingCaseIdAsync(listingCaseId, contactRequestDto);

        // Assert
        result.Should().Be(expectedCaseContactDto);
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
        _mapperMock.Verify(m => m.Map<CaseContact>(contactRequestDto), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.AddCaseContactAsync(caseContact), Times.Once);
        _mapperMock.Verify(m => m.Map<CaseContactDto>(caseContact), Times.Once);
    }

    [Fact]
    public async Task CreateListingCaseAsync_WhenUserNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var listingCaseRequestDto = new CreateListingCaseRequestDto { UserId = "1" };
        _listingCaseRepositoryMock.Setup(r => r.FindUserByIdAsync(listingCaseRequestDto.UserId)).ReturnsAsync((User?)null);

        // Act
        var act = async () => await _listingCaseServices.CreateListingCaseAsync(listingCaseRequestDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindUserByIdAsync(listingCaseRequestDto.UserId), Times.Once);
    }

    [Fact]
    public async Task CreateListingCaseAsync_WhenUserExist_ShouldReturnListingCaseResponseDto()
    {
        // Arrange
        var user = new User { Id = "1" };
        var createListingCaseRequestDto = new CreateListingCaseRequestDto { UserId = user.Id };

        _listingCaseRepositoryMock.Setup(r => r.FindUserByIdAsync(createListingCaseRequestDto.UserId)).ReturnsAsync(user);

        var listingCase = new ListingCase { Id = 1 };
        _mapperMock.Setup(m => m.Map<ListingCase>(createListingCaseRequestDto)).Returns(listingCase);

        var createdListingCase = new ListingCase { Id = 1, UserId = user.Id };
        _listingCaseRepositoryMock.Setup(r => r.AddListingCaseAsync(listingCase)).ReturnsAsync(createdListingCase);
        _loggerServiceMock
            .Setup(l => l.LogCreateListingCase(createdListingCase.Id.ToString(), createdListingCase.UserId, It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(Task.CompletedTask);

        var expectedListingCaseResponseDto = new ListingCaseResponseDto { Id = 1 };
        _mapperMock.Setup(m => m.Map<ListingCaseResponseDto>(createdListingCase)).Returns(expectedListingCaseResponseDto);

        // Act
        var result = await _listingCaseServices.CreateListingCaseAsync(createListingCaseRequestDto);

        // Assert
        result.Should().Be(expectedListingCaseResponseDto);
        _listingCaseRepositoryMock.Verify(r => r.FindUserByIdAsync(createListingCaseRequestDto.UserId), Times.Once);
        _mapperMock.Verify(m => m.Map<ListingCase>(createListingCaseRequestDto), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.AddListingCaseAsync(listingCase), Times.Once);
        _loggerServiceMock.Verify(l => l.LogCreateListingCase(createdListingCase.Id.ToString(), createdListingCase.UserId, It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
        _mapperMock.Verify(m => m.Map<ListingCaseResponseDto>(createdListingCase), Times.Once);
    }

    [Fact]
    public async Task CreateMediaByListingCaseIdAsync_WhenUserUploadManyVideos_ShouldThrowArgumentException()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var filesMock = new[] { CreateMockedFormFile("video1.mp4"), CreateMockedFormFile("video2.mp4") };

        // Act
        var act = async () => await _listingCaseServices.CreateMediaByListingCaseIdAsync(filesMock, MediaType.Videography, listingCaseId, userId);
    
        // Assert
        await act.Should().ThrowAsync<ArgumentErrorException>();
    }

    [Fact]
    public async Task CreateMediaByListingCaseIdAsync_WhenListingCaseNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var filesMock = new[] { CreateMockedFormFile("image1.jpg"), CreateMockedFormFile("image2.jpg") };

        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync((ListingCase?)null);

        // Act
        var act = async () => await _listingCaseServices.CreateMediaByListingCaseIdAsync(filesMock, MediaType.Photo, listingCaseId, userId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task CreateMediaByListingCaseIdAsync_WhenUserIsNotOwner_ShouldThrowForbiddenException()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var filesMock = new[] { CreateMockedFormFile("image1.jpg"), CreateMockedFormFile("image2.jpg") };
        var listingCase = new ListingCase { UserId = "2" };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);
        
        // Act
        var act = async () => await _listingCaseServices.CreateMediaByListingCaseIdAsync(filesMock, MediaType.Photo, listingCaseId, userId);
        
        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task CreateMediaByListingCaseIdAsync_WhenArgumentsAreValid_ShouldReturnListOfMediaAssetDto()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var filesMock = new[] { CreateMockedFormFile("image1.jpg"), CreateMockedFormFile("image2.jpg") };
        var listingCase = new ListingCase { UserId = userId };

        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);

        _blobStorageServiceMock
            .SetupSequence(b => b.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("image1_url.jpg")
            .ReturnsAsync("image2_url.jpg");

        // Capture the media assets the service creates
        var createdMediaAssets = new List<MediaAsset>();
        _listingCaseRepositoryMock
            .Setup(r => r.AddMediaAssetAsync(It.IsAny<IEnumerable<MediaAsset>>()))
            .Callback<IEnumerable<MediaAsset>>(assets => createdMediaAssets = assets.ToList())
            .ReturnsAsync(() => createdMediaAssets);

        IEnumerable<MediaAssetDto> expectedMediaAssetDto = new[] 
        { 
            new MediaAssetDto { Id = 1, MediaUrl = "image1_url.jpg" }, 
            new MediaAssetDto { Id = 2, MediaUrl = "image2_url.jpg" } 
        };
        _mapperMock.Setup(m => m.Map<IEnumerable<MediaAssetDto>>(It.IsAny<IEnumerable<MediaAsset>>())).Returns(expectedMediaAssetDto);

        // Act
        var result = await _listingCaseServices.CreateMediaByListingCaseIdAsync(filesMock, MediaType.Photo, listingCaseId, userId);

        // Assert
        result.Should().BeEquivalentTo(expectedMediaAssetDto);
        createdMediaAssets.Should().HaveCount(2);
        createdMediaAssets[0].MediaUrl.Should().Be("image1_url.jpg");
        createdMediaAssets[1].MediaUrl.Should().Be("image2_url.jpg");

        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
        _blobStorageServiceMock.Verify(b => b.UploadFileAsync(filesMock[0]), Times.Once);
        _blobStorageServiceMock.Verify(b => b.UploadFileAsync(filesMock[1]), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.AddMediaAssetAsync(createdMediaAssets), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<MediaAssetDto>>(createdMediaAssets), Times.Once);
    }

    [Fact]
    public async Task DeleteListingCaseByListingCaseIdAsync_WhenListingCaseNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync((ListingCase?)null);

        // Act
        var act = async () => await _listingCaseServices.DeleteListingCaseByListingCaseIdAsync(listingCaseId, userId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task DeleteListingCaseByListingCaseIdAsync_WhenUserIsNotOwner_ShouldThrowForbiddenException()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var listingCase = new ListingCase { UserId = "2" };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);

        // Act
        var act = async () => await _listingCaseServices.DeleteListingCaseByListingCaseIdAsync(listingCaseId, userId);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task DeleteListingCaseByListingCaseIdAsync_WhenArgumentsAreValid_ShouldDeleteListingCase()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var listingCase = new ListingCase { Id = listingCaseId, UserId = userId };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);
        _listingCaseRepositoryMock.Setup(r => r.DeleteListingCaseAsync(listingCase)).Returns(Task.CompletedTask);
        _loggerServiceMock.Setup(l => l.LogDeleteListingCase(listingCaseId.ToString(), userId, It.IsAny<string>(), It.IsAny<string>(), null)).Returns(Task.CompletedTask);

        // Act
        await _listingCaseServices.DeleteListingCaseByListingCaseIdAsync(listingCaseId, userId);

        // Assert
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.DeleteListingCaseAsync(listingCase), Times.Once);
        _loggerServiceMock.Verify(l => l.LogDeleteListingCase(listingCaseId.ToString(), userId, It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task DownloadAllMediaByListingCaseIdAsync_WhenListingCaseNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var listingCaseId = 1;
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync((ListingCase?)null);

        // Act
        var act = async () => await _listingCaseServices.DownloadAllMediaByListingCaseIdAsync(listingCaseId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task DownloadAllMediaByListingCaseIdAsync_WhenFindMediaAssetsReturnsNull_ShouldThrowNotFoundException()
    {
        // Arrange
        var listingCaseId = 1;
        var listingCase = new ListingCase { Id = listingCaseId };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);
        _listingCaseRepositoryMock.Setup(r => r.FindMediaAssetsByListingCaseIdAsync(listingCaseId)).ReturnsAsync((IEnumerable<MediaAsset>?)null);

        // Act
        var act = async () => await _listingCaseServices.DownloadAllMediaByListingCaseIdAsync(listingCaseId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.FindMediaAssetsByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task DownloadAllMediaByListingCaseIdAsync_WhenFindMediaAssetsReturnsEmpty_ShouldThrowNotFoundException()
    {
        // Arrange
        var listingCaseId = 1;
        var listingCase = new ListingCase { Id = listingCaseId };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);
        _listingCaseRepositoryMock.Setup(r => r.FindMediaAssetsByListingCaseIdAsync(listingCaseId)).ReturnsAsync(new List<MediaAsset>());

        // Act
        var act = async () => await _listingCaseServices.DownloadAllMediaByListingCaseIdAsync(listingCaseId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.FindMediaAssetsByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task DownloadAllMediaByListingCaseIdAsync_WhenArgumentsAreValid_ShouldReturnZipFile()
    {
        // Arrange
        var listingCaseId = 1;
        var listingCase = new ListingCase { Id = listingCaseId };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);

        var mediaAssets = new List<MediaAsset>
        {
            new MediaAsset { Id = 1, ListingCaseId = listingCaseId, MediaUrl = "url1.jpg" },
            new MediaAsset { Id = 2, ListingCaseId = listingCaseId, MediaUrl = "url2.jpg" }
        };

        _listingCaseRepositoryMock.Setup(r => r.FindMediaAssetsByListingCaseIdAsync(listingCaseId)).ReturnsAsync(mediaAssets);
    
        var file1 = new byte[] { 1, 2, 3 };
        var file2 = new byte[] { 4, 5, 6 };
        _blobStorageServiceMock
            .SetupSequence(b => b.DownloadFileAsync(It.IsAny<string>()))
            .ReturnsAsync((
            Content: (Stream)new MemoryStream(file1),
            ContentType: "image/jpeg",
            FileName: "image1.jpg"))
            .ReturnsAsync((
            Content: (Stream)new MemoryStream(file2),
            ContentType: "image/jpeg",
            FileName: "image2.jpg"));

        // Act
        var result = await _listingCaseServices.DownloadAllMediaByListingCaseIdAsync(listingCaseId);

        // Assert
        result.Should().NotBeNull();
        // Verify the zip content
        result.ZipContent.Should().NotBeNull();
        using var zipStream = new MemoryStream(result.ZipContent);
        using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        zipArchive.Entries.Should().HaveCount(2);
        var entry1 = zipArchive.Entries.Single(e => e.Name == "image1.jpg");
        var entry2 = zipArchive.Entries.Single(e => e.Name == "image2.jpg");

        result.ContentType.Should().Be("application/zip");
        result.ZipFileName.Should().Be($"listingcase_{listingCaseId}_media.zip");
    
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.FindMediaAssetsByListingCaseIdAsync(listingCaseId), Times.Once);
        _blobStorageServiceMock.Verify(b => b.DownloadFileAsync(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GenerateSharedUrlAsync_WhenListingCaseDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var listingCaseId = 1;
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync((ListingCase?)null);

        // Act
        var act = async () => await _listingCaseServices.GenerateSharedUrlAsync(listingCaseId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task GenerateSharedUrlAsync_WhenListingCaseExists_ShouldReturnSharedUrl()
    {
        // Arrange
        var listingCaseId = 1;
        var listingCase = new ListingCase { Id = listingCaseId };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);
        _configurationMock.Setup(c => c["Url:BaseUrl"]).Returns("https://test.com");
        _listingCaseRepositoryMock.Setup(r => r.UpdateListingCaseAsync(listingCase)).Returns(Task.CompletedTask);

        // Act
        var result = await _listingCaseServices.GenerateSharedUrlAsync(listingCaseId);

        // Assert
        result.Should().StartWith("https://test.com/listings/share/");
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.UpdateListingCaseAsync(listingCase), Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(2, 0)]
    [InlineData(2, -1)]
    [InlineData(-1, -1)]
    public async Task GetAllListingCasesAsync_WhenArgumentsAreInvalid_ShouldThrowArgumentErrorException(int pageNumber, int pageSize)
    {
        // Arrange
        var userId = "1";
        var userRole = RoleNames.Agent;

        // Act
        var act = async () => await _listingCaseServices.GetAllListingCasesAsync(pageNumber, pageSize, userId, userRole);

        // Assert
        await act.Should().ThrowAsync<ArgumentErrorException>();
    }

    [Fact]
    public async Task GetAllListingCasesAsync_WhenUserRoleIsPhotographyCompanyButListingCaseDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var userId = "1";
        var userRole = RoleNames.PhotographyCompany;
        _listingCaseRepositoryMock
            .Setup(r => r.FindListingCasesByPhotographyCompanyIdAsync(pageNumber, pageSize, userId))
            .ReturnsAsync((IEnumerable<ListingCase>?)null);

        // Act
        var act = async () => await _listingCaseServices.GetAllListingCasesAsync(pageNumber, pageSize, userId, userRole);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCasesByPhotographyCompanyIdAsync(pageNumber, pageSize, userId), Times.Once);
    }

    [Fact]
    public async Task GetAllListingCasesAsync_WhenUserRoleIsPhotographyCompanyButListingCaseIsEmpty_ShouldThrowNotFoundException()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var userId = "1";
        var userRole = RoleNames.PhotographyCompany;
        _listingCaseRepositoryMock
            .Setup(r => r.FindListingCasesByPhotographyCompanyIdAsync(pageNumber, pageSize, userId))
            .ReturnsAsync(new List<ListingCase>());

        // Act
        var act = async () => await _listingCaseServices.GetAllListingCasesAsync(pageNumber, pageSize, userId, userRole);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCasesByPhotographyCompanyIdAsync(pageNumber, pageSize, userId), Times.Once);
    }

    [Fact]
    public async Task GetAllListingCasesAsync_WhenUserRoleIsPhotographyCompanyAndListingCaseExists_ShouldReturnPagedListingCases()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var userId = "1";
        var userRole = RoleNames.PhotographyCompany;
        var listingCases = new List<ListingCase>
        {
            new ListingCase { Id = 1 },
            new ListingCase { Id = 2 }
        };

        _listingCaseRepositoryMock
            .Setup(r => r.FindListingCasesByPhotographyCompanyIdAsync(pageNumber, pageSize, userId))
            .ReturnsAsync(listingCases);

        var totalCount = 2;
        _listingCaseRepositoryMock
            .Setup(r => r.CountListingCasesByPhotographyCompanyIdAsync(userId))
            .ReturnsAsync(totalCount);

        var listingCaseResponseDto = new List<ListingCaseResponseDto>
        {
            new ListingCaseResponseDto { Id = 1 },
            new ListingCaseResponseDto { Id = 2 }
        };

        _mapperMock
            .Setup(m => m.Map<IEnumerable<ListingCaseResponseDto>>(listingCases))
            .Returns(listingCaseResponseDto);

        var expectedResult = new PagedResult<ListingCaseResponseDto>
        (
            pageNumber,
            pageSize,
            totalCount,
            listingCaseResponseDto
        );

        // Act
        var result = await _listingCaseServices.GetAllListingCasesAsync(pageNumber, pageSize, userId, userRole);
    
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _listingCaseRepositoryMock.Verify(r => r.FindListingCasesByPhotographyCompanyIdAsync(pageNumber, pageSize, userId), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.CountListingCasesByPhotographyCompanyIdAsync(userId), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<ListingCaseResponseDto>>(listingCases), Times.Once);
    }

    [Fact]
    public async Task GetAllListingCasesAsync_WhenUserRoleIsAgentButListingCaseDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var userId = "1";
        var userRole = RoleNames.Agent;
        _listingCaseRepositoryMock
            .Setup(r => r.FindListingCasesByAgentIdAsync(pageNumber, pageSize, userId))
            .ReturnsAsync((IEnumerable<ListingCase>?)null);

        // Act
        var act = async () => await _listingCaseServices.GetAllListingCasesAsync(pageNumber, pageSize, userId, userRole);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCasesByAgentIdAsync(pageNumber, pageSize, userId), Times.Once);
    }

    [Fact]
    public async Task GetAllListingCasesAsync_WhenUserRoleIsAgentButListingCaseIsEmpty_ShouldThrowNotFoundException()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var userId = "1";
        var userRole = RoleNames.Agent;
        _listingCaseRepositoryMock
            .Setup(r => r.FindListingCasesByAgentIdAsync(pageNumber, pageSize, userId))
            .ReturnsAsync(new List<ListingCase>());

        // Act
        var act = async () => await _listingCaseServices.GetAllListingCasesAsync(pageNumber, pageSize, userId, userRole);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCasesByAgentIdAsync(pageNumber, pageSize, userId), Times.Once);
    }

    [Fact]
    public async Task GetAllListingCasesAsync_WhenUserRoleIsAgentAndListingCaseExists_ShouldReturnPagedListingCases()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var userId = "1";
        var userRole = RoleNames.Agent;
        var listingCases = new List<ListingCase>
        {
            new ListingCase { Id = 1 },
            new ListingCase { Id = 2 }
        };

        _listingCaseRepositoryMock
            .Setup(r => r.FindListingCasesByAgentIdAsync(pageNumber, pageSize, userId))
            .ReturnsAsync(listingCases);

        var totalCount = 2;
        _listingCaseRepositoryMock
            .Setup(r => r.CountListingCasesByAgentIdAsync(userId))
            .ReturnsAsync(totalCount);

        var listingCaseResponseDto = new List<ListingCaseResponseDto>
        {
            new ListingCaseResponseDto { Id = 1 },
            new ListingCaseResponseDto { Id = 2 }
        };

        _mapperMock
            .Setup(m => m.Map<IEnumerable<ListingCaseResponseDto>>(listingCases))
            .Returns(listingCaseResponseDto);

        var expectedResult = new PagedResult<ListingCaseResponseDto>
        (
            pageNumber,
            pageSize,
            totalCount,
            listingCaseResponseDto
        );

        // Act
        var result = await _listingCaseServices.GetAllListingCasesAsync(pageNumber, pageSize, userId, userRole);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _listingCaseRepositoryMock.Verify(r => r.FindListingCasesByAgentIdAsync(pageNumber, pageSize, userId), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.CountListingCasesByAgentIdAsync(userId), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<ListingCaseResponseDto>>(listingCases), Times.Once);
    }

    [Fact]
    public async Task GetFinalSelectionByListingCaseIdAsync_WhenListingCaseDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var listingCaseId = 1;
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync((ListingCase?)null);

        // Act
        var act = async () => await _listingCaseServices.GetFinalSelectionByListingCaseIdAsync(listingCaseId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task GetFinalSelectionByListingCaseIdAsync_WhenListingCaseStatusIsNotDelivered_ShouldThrowNotInvalidException()
    {
        // Arrange
        var listingCaseId = 1;
        var listingCase = new ListingCase { Id = listingCaseId, ListingCaseStatus = ListingCaseStatus.Created };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);
    
        // Act
        var act = async () => await _listingCaseServices.GetFinalSelectionByListingCaseIdAsync(listingCaseId);
    
        // Assert
        await act.Should().ThrowAsync<InvalidException>();
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task GetFinalSelectionByListingCaseIdAsync_WhenListingCaseStatusIsDelivered_ShouldReturnAllMediaAssets()
    {
        // Arrange
        var listingCaseId = 1;
        var listingCase = new ListingCase { Id = listingCaseId, ListingCaseStatus = ListingCaseStatus.Delivered };
        _listingCaseRepositoryMock.Setup(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId)).ReturnsAsync(listingCase);
        var mediaAssets = new List<MediaAsset>
        {
            new MediaAsset { Id = 1 },
            new MediaAsset { Id = 2 }
        };
        _listingCaseRepositoryMock.Setup(r => r.GetFinalSelectionByListingCaseIdAsync(listingCaseId)).ReturnsAsync(mediaAssets);

        var expectedResult = new List<MediaAssetDto>
        {
            new MediaAssetDto { Id = 1 },
            new MediaAssetDto { Id = 2 }
        };
        _mapperMock.Setup(m => m.Map<IEnumerable<MediaAssetDto>>(mediaAssets)).Returns(expectedResult);

        // Act
        var result = await _listingCaseServices.GetFinalSelectionByListingCaseIdAsync(listingCaseId);
    
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _listingCaseRepositoryMock.Verify(r => r.FindListingCaseByListingCaseIdAsync(listingCaseId), Times.Once);
        _listingCaseRepositoryMock.Verify(r => r.GetFinalSelectionByListingCaseIdAsync(listingCaseId), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<MediaAssetDto>>(mediaAssets), Times.Once);
    }
}
