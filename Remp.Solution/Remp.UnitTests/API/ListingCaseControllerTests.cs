using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Remp.API.Controllers;
using Remp.Common.Helpers;
using Remp.Common.Helpers.ApiResponse;
using Remp.Models.Constants;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.Security.Claims;

namespace Remp.UnitTests.API;

public class ListingCaseControllerTests
{
    private readonly Mock<IListingCaseService> _listingCaseServiceMock;

    public ListingCaseControllerTests()
    {
        _listingCaseServiceMock = new Mock<IListingCaseService>();
    }

    private ListingCaseController CreateController(ClaimsPrincipal? user = null)
    {
        var controller = new ListingCaseController(_listingCaseServiceMock.Object);

        var httpContext = new DefaultHttpContext();
        if (user != null)
        {
            httpContext.User = user;
        }

        controller.ControllerContext.HttpContext = httpContext;

        return controller;
    }

    private ClaimsPrincipal CreateUser(string? userId = null, string? role = null)
    {
        var claims = new List<Claim>();
        if (!string.IsNullOrEmpty(userId))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }
        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim("scopes", role));
        }

        var identity = new ClaimsIdentity(claims, "Test");

        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task GetListingCaseByListingCaseId_WhenNoUserId_ReturnsForBid()
    {
        // Arrange
        var user = CreateUser(userId: null, role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);

        // Act
        var result = await controller.GetListingCaseByListingCaseIdAsync(1);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetListingCaseByListingCaseId_WhenListingCaseIdExist_ShouldReturnOk()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);

        var detailListingCaseResponse  = new ListingCaseDetailResponseDto { Id = 1 };
        _listingCaseServiceMock
            .Setup(x => x.GetListingCaseByListingCaseIdAsync(1, "1", RoleNames.PhotographyCompany))
            .ReturnsAsync(detailListingCaseResponse);

        // Act
        var result = await controller.GetListingCaseByListingCaseIdAsync(1);

        // Assert
        result.Result.Should().NotBeNull();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GetResponse<ListingCaseDetailResponseDto>>(okResult.Value);
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(detailListingCaseResponse);
    }

    [Fact]
    public async Task CreateListingCase_WhenRequestIsValid_ShouldReturnCreatedAtRoute()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);

        var request = new CreateListingCaseRequestDto
        {
            Title = "Test listing case",
            Description = "This is a test listing case description.",
            Street = "123 Main St",
            City = "Melbourne",
            State = "Victoria",
            Postcode = 3000,
            Longitude = 23.45m,
            Latitude = 67.89m,
            Price = 1000000,
            Bedrooms = 2,
            Bathrooms = 1,
            Garages = 1,
            FloorArea = 134.23,
            PropertyType = "House",
            SaleCategory = "ForSale",
            UserId = "1"
        };

        var responseDto = new ListingCaseResponseDto { Id = 18 };

        _listingCaseServiceMock
            .Setup(x => x.CreateListingCaseAsync(request))
            .ReturnsAsync(responseDto);

        // Act
        var result = await controller.CreateListingCase(request);

        // Assert
        result.Result.Should().NotBeNull();
        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var response = Assert.IsType<PostResponse<ListingCaseResponseDto>>(createdResult.Value);
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(responseDto);
        response.Message.Should().Be("Created successfully");

        _listingCaseServiceMock.Verify(x => x.CreateListingCaseAsync(request), Times.Once);
    }

    [Fact]
    public async Task GetAllListingCasesAsync_WhenNoUserId_ShouldReturnsForBid()
    {
        // Arrange
        var user = CreateUser(userId: null, role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);

        // Act
        var result = await controller.GetAllListingCasesAsync(1, 10);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetAllListingCasesAsync_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseResponse = new List<ListingCaseResponseDto>
        { 
            new ListingCaseResponseDto { Id = 1 },
            new ListingCaseResponseDto { Id = 2 }
        };
        var pagedResult = new PagedResult<ListingCaseResponseDto>(
            pageNumber: 1,
            pageSize: 10,
            totalCount: 2,
            items: listingCaseResponse
            );

        _listingCaseServiceMock
            .Setup(x => x.GetAllListingCasesAsync(1, 10, "1", RoleNames.PhotographyCompany))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await controller.GetAllListingCasesAsync(1, 10);

        // Assert
        result.Result.Should().NotBeNull();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GetResponse<PagedResult<ListingCaseResponseDto>>>(okResult.Value);
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(pagedResult);
    }

    [Fact]
    public async Task UpdateListingCaseAsync_WhenNoUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        var user = CreateUser(userId: null, role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var request = new UpdateListingCaseRequestDto
        {
            Title = "Test listing case",
            Description = "This is a test listing case description.",
            Street = "123 Main St",
            City = "Melbourne",
            State = "Victoria",
            Postcode = 3000,
            Longitude = 23.45m,
            Latitude = 67.89m,
            Price = 1000000,
            Bedrooms = 2,
            Bathrooms = 1,
            Garages = 1,
            FloorArea = 134.23,
            PropertyType = "House",
            SaleCategory = "ForSale",
        };

        // Act
        var result = await controller.UpdateListingCaseAsync(listingCaseId, request);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);

        _listingCaseServiceMock.Verify(x => x.UpdateListingCaseAsync(listingCaseId, request, "1"), Times.Never);
    }

    [Fact]
    public async Task UpdateListingCaseAsync_WhenRequestIsValid_ShouldReturn204()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var request = new UpdateListingCaseRequestDto
        {
            Title = "Test listing case",
            Description = "This is a test listing case description.",
            Street = "123 Main St",
            City = "Melbourne",
            State = "Victoria",
            Postcode = 3000,
            Longitude = 23.45m,
            Latitude = 67.89m,
            Price = 1000000,
            Bedrooms = 2,
            Bathrooms = 1,
            Garages = 1,
            FloorArea = 134.23,
            PropertyType = "House",
            SaleCategory = "ForSale",
        };

        // Act
        var result = await controller.UpdateListingCaseAsync(listingCaseId, request);

        // Assert
        result.Result.Should().NotBeNull();
        var requestResult = Assert.IsType<ObjectResult>(result.Result);
        requestResult.StatusCode.Should().Be(204);
        var response = Assert.IsType<PutResponse>(requestResult.Value);
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Updated successfully");
        _listingCaseServiceMock.Verify(x => x.UpdateListingCaseAsync(listingCaseId, request, "1"), Times.Once);
    }

    [Fact]
    public async Task DeleteListingCaseByListingCaseIdAsync_WhenNoUserId_ShouldReturnForBid()
    {
        // Arrange
        var user = CreateUser(userId: null, role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseId = 1;

        // Act
        var result = await controller.DeleteListingCaseByListingCaseIdAsync(listingCaseId);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task DeleteListingCaseByListingCaseIdAsync_WhenRequestIsValid_ShouldReturn204()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;

        // Act
        var result = await controller.DeleteListingCaseByListingCaseIdAsync(listingCaseId);

        // Assert
        var requestResult = Assert.IsType<ObjectResult>(result.Result);
        requestResult.StatusCode.Should().Be(204);
        var response = Assert.IsType<DeleteResponse>(requestResult.Value);
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Deleted successfully");

        _listingCaseServiceMock.Verify(x => x.DeleteListingCaseByListingCaseIdAsync(listingCaseId, "1"), Times.Once);
    }

    [Fact]
    public async Task UpdateListingCaseStatusAsync_WhenNoUserId_ShouldReturnForBid()
    {
        // Arrange
        var user = CreateUser(userId: null, role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseId = 1;

        // Act
        var result = await controller.UpdateListingCaseStatusAsync(listingCaseId);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task UpdateListingCaseStatusAsync_WhenRequestIsValid_ShouldReturn204()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseId = 1;

        // Act
        var result = await controller.UpdateListingCaseStatusAsync(listingCaseId);

        // Assert
        var requestResult = Assert.IsType<ObjectResult>(result.Result);
        requestResult.StatusCode.Should().Be(204);
        var response = Assert.IsType<PutResponse>(requestResult.Value);
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Updated successfully");

        _listingCaseServiceMock.Verify(x => x.UpdateListingCaseStatusAsync(listingCaseId, "1"), Times.Once);
    }

    [Fact]
    public async Task GetListingCaseMediaByListingCaseIdAsync_WhenNoUserId_ShouldReturnForBid()
    {
        // Arrange
        var user = CreateUser(userId: null, role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseId = 1;

        // Act
        var result = await controller.GetListingCaseMediaByListingCaseIdAsync(listingCaseId);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetListingCaseMediaByListingCaseIdAsync_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var mediaLists = new List<MediaAssetDto> { 
            new MediaAssetDto { Id = 1 },
            new MediaAssetDto { Id = 2 }
        };

        _listingCaseServiceMock
            .Setup(x => x.GetListingCaseMediaByListingCaseIdAsync(listingCaseId, "1", RoleNames.Agent))
            .ReturnsAsync(mediaLists);

        // Act
        var result = await controller.GetListingCaseMediaByListingCaseIdAsync(listingCaseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GetResponse<IEnumerable<MediaAssetDto>>>(okResult.Value);
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(mediaLists);

        _listingCaseServiceMock.Verify(x => x.GetListingCaseMediaByListingCaseIdAsync(listingCaseId, "1", RoleNames.Agent), Times.Once);
    }

    [Fact]
    public async Task GetListingCaseContactByListingCaseIdAsync_WhenNoUserId_ShouldReturnForBid()
    {
        // Arrange
        var user = CreateUser(userId: null, role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseId = 1;

        // Act
        var result = await controller.GetListingCaseContactByListingCaseIdAsync(listingCaseId);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetListingCaseContactByListingCaseIdAsync_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var contactLists = new List<CaseContactDto> { 
            new CaseContactDto { ContactId = 1 },
            new CaseContactDto { ContactId = 2 }
        };

        _listingCaseServiceMock
            .Setup(x => x.GetListingCaseContactByListingCaseIdAsync(listingCaseId, "1", RoleNames.Agent))
            .ReturnsAsync(contactLists);

        // Act
        var result = await controller.GetListingCaseContactByListingCaseIdAsync(listingCaseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GetResponse<IEnumerable<CaseContactDto>>>(okResult.Value);
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(contactLists);

        _listingCaseServiceMock.Verify(x => x.GetListingCaseContactByListingCaseIdAsync(listingCaseId, "1", RoleNames.Agent), Times.Once);
    }

    [Fact]
    public async Task CreateCaseContactByListingCaseIdAsync_WhenRequestIsValid_ShouldReturnCreatedAtRoute()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var request = new CreateCaseContactRequestDto();

        // Act
        var result = await controller.CreateCaseContactByListingCaseIdAsync(listingCaseId, request);

        // Assert
        var okResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var response = Assert.IsType<PostResponse<CaseContactDto>>(okResult.Value);
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Created successfully");
    }

    [Fact]
    public async Task DownloadAllMediaByListingCaseIdAsync_WhenResponseIsValid_ShouldReturnFileContentResult()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;

        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var contentType = "application/zip";
        var fileName = "test.zip";

        _listingCaseServiceMock
            .Setup(x => x.DownloadAllMediaByListingCaseIdAsync(listingCaseId))
            .ReturnsAsync((stream.ToArray(), contentType, fileName));

        // Act
        var result = await controller.DownloadAllMediaByListingCaseIdAsync(listingCaseId);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        fileResult.FileContents.Should().BeEquivalentTo(stream.ToArray());
        fileResult.ContentType.Should().Be(contentType);
        fileResult.FileDownloadName.Should().Be(fileName);

        _listingCaseServiceMock.Verify(x => x.DownloadAllMediaByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task SetCoverImageByListingCaseIdAsync_WhenRequestIsValid_ShouldReturn204()
    {
        // Arrange
        var userId = "1";
        var user = CreateUser(userId: "1", role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var mediaAssetId = 1;

        _listingCaseServiceMock
            .Setup(x => x.SetCoverImageByListingCaseIdAsync(listingCaseId, mediaAssetId, userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.SetCoverImageByListingCaseIdAsync(listingCaseId, mediaAssetId);
    
        // Assert
        var requestResult = Assert.IsType<ObjectResult>(result.Result);
        requestResult.StatusCode.Should().Be(204);
        var response = Assert.IsType<PutResponse>(requestResult.Value);
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Updated successfully");

        _listingCaseServiceMock.Verify(x => x.SetCoverImageByListingCaseIdAsync(listingCaseId, mediaAssetId, userId), Times.Once);
    }

    [Fact]
    public async Task GetFinalSelectionByListingCaseIdAsync_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;

        IEnumerable<MediaAssetDto> mediaLists = new List<MediaAssetDto>
        {
            new MediaAssetDto { Id = 1 },
            new MediaAssetDto { Id = 2 }
        };

        _listingCaseServiceMock
            .Setup(x => x.GetFinalSelectionByListingCaseIdAsync(listingCaseId))
            .ReturnsAsync(mediaLists);

        // Act
        var result = await controller.GetFinalSelectionByListingCaseIdAsync(listingCaseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GetResponse<IEnumerable<MediaAssetDto>>>(okResult.Value);
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(mediaLists);

        _listingCaseServiceMock.Verify(x => x.GetFinalSelectionByListingCaseIdAsync(listingCaseId), Times.Once);
    }

    [Fact]
    public async Task SetSelectedMediaByListingCaseIdAsync_WhenNoUserId_ShouldReturnForBid()
    {
        // Arrange
        var user = CreateUser(userId: null, role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var request = new SetSelectedMediaRequestDto();

        // Act
        var result = await controller.SetSelectedMediaByListingCaseIdAsync(listingCaseId, request);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task SetSelectedMediaByListingCaseIdAsync_WhenRequestIsValid_ShouldReturn204()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var request = new SetSelectedMediaRequestDto { MediaIds = new List<int> { 1, 2 } };

        _listingCaseServiceMock
            .Setup(x => x.SetSelectedMediaByListingCaseIdAsync(listingCaseId, request.MediaIds, "1"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.SetSelectedMediaByListingCaseIdAsync(listingCaseId, request);

        // Assert
        var requestResult = Assert.IsType<ObjectResult>(result.Result);
        requestResult.StatusCode.Should().Be(204);
        var response = Assert.IsType<PutResponse>(requestResult.Value);
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Updated successfully");

        _listingCaseServiceMock.Verify(x => x.SetSelectedMediaByListingCaseIdAsync(listingCaseId, request.MediaIds, "1"), Times.Once);
    }

    [Fact]
    public async Task GenerateSharedUrlAsync_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.Agent);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var expectedUrl = "https://shared-url";

        _listingCaseServiceMock
            .Setup(x => x.GenerateSharedUrlAsync(listingCaseId))
            .ReturnsAsync(expectedUrl);

        // Act
        var result = await controller.GenerateSharedUrlAsync(listingCaseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PostResponse<string>>(okResult.Value);
        response.Success.Should().BeTrue();
        response.Data.Should().Be(expectedUrl);
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenNoUserId_ShouldReturnForBid()
    {
        // Arrange
        var user = CreateUser(userId: null, role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var agentId = "15";

        // Act
        var result = await controller.AddAgentToListingCaseAsync(listingCaseId, agentId);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenAllArgumentsAreValid_ShouldReturn204()
    {
        // Arrange
        var user = CreateUser(userId: "1", role: RoleNames.PhotographyCompany);
        var controller = CreateController(user);
        var listingCaseId = 1;
        var agentId = "15";

        _listingCaseServiceMock
            .Setup(x => x.AddAgentToListingCaseAsync(listingCaseId, agentId, "1"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.AddAgentToListingCaseAsync(listingCaseId, agentId);

        // Assert
        var requestResult = Assert.IsType<ObjectResult>(result);
        requestResult.StatusCode.Should().Be(204);
        var response = Assert.IsType<PutResponse>(requestResult.Value);
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Updated successfully");

        _listingCaseServiceMock.Verify(x => x.AddAgentToListingCaseAsync(listingCaseId, agentId, "1"), Times.Once);
    }
}
