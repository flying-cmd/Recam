using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using Remp.Repository.Interfaces;
using Remp.Service.Interfaces;
using Remp.Service.Services;

namespace Remp.UnitTests.Services;

public class ListingCaseServiceTests
{
    private readonly Mock<IListingCaseRepository> _listingCaseRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
    private readonly Mock<IMediaRepository> _mediaRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ListingCaseService _listingCaseServices;

    public ListingCaseServiceTests()
    {
        _listingCaseRepositoryMock = new Mock<IListingCaseRepository>();
        _mapperMock = new Mock<IMapper>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _mediaRepositoryMock = new Mock<IMediaRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _listingCaseServices = new ListingCaseService(
            _listingCaseRepositoryMock.Object,
            _mapperMock.Object,
            _blobStorageServiceMock.Object,
            _mediaRepositoryMock.Object,
            _configurationMock.Object);
    }

    //[Fact]
    //public async Task CreateCaseContactByListingCaseIdAsync_WhenListingCaseIdNotExist_ShouldThrowNotFoundException()
    //{
    //    // Arrange
    //    var listingCaseId = -1;
    //    var contactRequestDto = new ContactRequestDto();
    //    _listingCaseRepositoryMock.Setup(r => r.FindByIdAsync(listingCaseId)).ReturnsAsync((ListingCase?)null);
    //}
}
