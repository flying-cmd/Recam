using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.DataAccess.Data;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Repository.Repositories;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using Remp.Service.Services;

namespace Remp.UnitTests.Services;

public class UserServiceTests : IDisposable
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly AppDbContext _appDbContext;
    private readonly Mock<ILoggerService> _loggerServiceMock;
    private UserService _userService;

    public UserServiceTests()
    {
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _appDbContext = new AppDbContext(dbOptions);

        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _loggerServiceMock = new Mock<ILoggerService>();
        _userService = new UserService(
            _userRepositoryMock.Object, 
            _mapperMock.Object, 
            _userManagerMock.Object, 
            _appDbContext,
            _loggerServiceMock.Object);
    }

    public void Dispose()
    {
        _appDbContext.Dispose();
    }

    [Fact]
    public async Task AddAgentByIdAsync_WhenPhotographyCompanyDoesNotExist_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var agentId = "1";
        var photographyCompanyId = "2";
        _userRepositoryMock.Setup(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId)).ReturnsAsync((PhotographyCompany?)null);

        // Act
        var act = async () => await _userService.AddAgentByIdAsync(agentId, photographyCompanyId);
    
        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
        _userRepositoryMock.Verify(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId), Times.Once);
    }

    [Fact]
    public async Task AddAgentByIdAsync_WhenAgentDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var agentId = "1";
        var photographyCompanyId = "2";
        var photographyCompany = new PhotographyCompany { Id = photographyCompanyId };
        _userRepositoryMock.Setup(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId)).ReturnsAsync(photographyCompany);
        _userRepositoryMock.Setup(r => r.FindAgentByIdAsync(agentId)).ReturnsAsync((Agent?)null);

        // Act
        var act = async () => await _userService.AddAgentByIdAsync(agentId, photographyCompanyId);
    
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _userRepositoryMock.Verify(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId), Times.Once);
        _userRepositoryMock.Verify(r => r.FindAgentByIdAsync(agentId), Times.Once);
    }

    [Fact]
    public async Task AddAgentByIdAsync_WhenPhotographyHasAddedAgent_ShouldThrowArgumentErrorException()
    {
        // Arrange
        var agentId = "1";
        var photographyCompanyId = "2";
        var photographyCompany = new PhotographyCompany { Id = photographyCompanyId };
        var agent = new Agent { Id = agentId };
        _userRepositoryMock.Setup(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId)).ReturnsAsync(photographyCompany);
        _userRepositoryMock.Setup(r => r.FindAgentByIdAsync(agentId)).ReturnsAsync(agent);
        _userRepositoryMock.Setup(r => r.IsAgentAddedToPhotographyCompanyAsync(agentId, photographyCompanyId)).ReturnsAsync(true);

        // Act
        var act = async () => await _userService.AddAgentByIdAsync(agentId, photographyCompanyId);
    
        // Assert
        await act.Should().ThrowAsync<ArgumentErrorException>();
        _userRepositoryMock.Verify(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId), Times.Once);
        _userRepositoryMock.Verify(r => r.FindAgentByIdAsync(agentId), Times.Once);
        _userRepositoryMock.Verify(r => r.IsAgentAddedToPhotographyCompanyAsync(agentId, photographyCompanyId), Times.Once);
    }

    [Fact]
    public async Task AddAgentByIdAsync_WhenAllArgumentsAreValid_ShouldAddAgentToPhotographyCompany()
    {
        // Arrange
        var agentId = "1";
        var photographyCompanyId = "2";
        var photographyCompany = new PhotographyCompany { Id = photographyCompanyId };
        var agent = new Agent { Id = agentId };
        _userRepositoryMock.Setup(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId)).ReturnsAsync(photographyCompany);
        _userRepositoryMock.Setup(r => r.FindAgentByIdAsync(agentId)).ReturnsAsync(agent);
        _userRepositoryMock.Setup(r => r.IsAgentAddedToPhotographyCompanyAsync(agentId, photographyCompanyId)).ReturnsAsync(false);

        var addedAgent = new Agent 
        { 
            Id = agentId,
            AgentPhotographyCompanies = new List<AgentPhotographyCompany> 
            {
                new AgentPhotographyCompany { AgentId = agentId, PhotographyCompanyId = photographyCompanyId }
            }
        };
        _userRepositoryMock.Setup(r => r.AddAgentToPhotographyCompanyAsync(It.IsAny<AgentPhotographyCompany>())).Returns(Task.CompletedTask);

        // Act
        await _userService.AddAgentByIdAsync(agentId, photographyCompanyId);
    
        // Assert
        _userRepositoryMock.Verify(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId), Times.Once);
        _userRepositoryMock.Verify(r => r.FindAgentByIdAsync(agentId), Times.Once);
        _userRepositoryMock.Verify(r => r.IsAgentAddedToPhotographyCompanyAsync(agentId, photographyCompanyId), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAgentToPhotographyCompanyAsync(It.IsAny<AgentPhotographyCompany>()), Times.Once);
    }

    [Fact]
    public async Task CreateAgentAccountAsync_WhenEmailAlreadyExists_ShouldThrowArgumentErrorException()
    {
        // Arrange
        var newAgent = new User { Email = "test@example.com" };
        var request = new CreateAgentAccountRequestDto { Email = newAgent.Email };
        var photographyCompanyId = "1";
        _userRepositoryMock.Setup(r => r.FindUserByEmailAsync(newAgent.Email)).ReturnsAsync(newAgent);
        _loggerServiceMock
            .Setup(l => l.LogCreateAgentAccount(
                It.IsAny<string>(), 
                null, 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(),
                It.IsAny<string>()
                ))
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _userService.CreateAgentAccountAsync(request, photographyCompanyId);
    
        // Assert
        await act.Should().ThrowAsync<ArgumentErrorException>();
        _userRepositoryMock.Verify(r => r.FindUserByEmailAsync(newAgent.Email), Times.Once);
        _loggerServiceMock.Verify(l => l.LogCreateAgentAccount(It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateAgentAccountAsync_WhenAllStepsAreSucceed_ShouldReturnCreateAgentAccountResponseDto()
    {
        // Arrange
        var newAgent = new User { Email = "test@example.com" };
        var request = new CreateAgentAccountRequestDto { Email = newAgent.Email };
        var photographyCompanyId = "1";
        _userRepositoryMock.Setup(r => r.FindUserByEmailAsync(newAgent.Email)).ReturnsAsync((User?)null);
        
        // Capture the passed user and password
        var newUser = new User();
        string password = "";

        _userManagerMock
            .Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .Callback<User, string>((u, p) => { newUser = u; password = p; })
            .ReturnsAsync(IdentityResult.Success);
        
        _userManagerMock.Setup(u => u.AddToRoleAsync(It.Is<User>(user => user.Email == newAgent.Email), RoleNames.Agent)).ReturnsAsync(IdentityResult.Success);

        _userRepositoryMock.Setup(r => r.AddAgentAsync(It.Is<Agent>(a => a.Id == newUser.Id))).Returns(Task.CompletedTask);

        _loggerServiceMock
            .Setup(l => l.LogCreateAgentAccount(
                It.IsAny<string>(), 
                It.IsAny<string>(),
                newAgent.Email,
                It.IsAny<string>(), 
                It.IsAny<string>(),
                null
                ))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.CreateAgentAccountAsync(request, photographyCompanyId);
    
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreateAgentAccountResponseDto>();
        result.Email.Should().Be(newAgent.Email);
        result.Password.Should().Be(password);

        _userRepositoryMock.Verify(r => r.FindUserByEmailAsync(newAgent.Email), Times.Once);
        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(It.Is<User>(user => user.Email == newAgent.Email), RoleNames.Agent), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAgentAsync(It.Is<Agent>(a => a.Id == newUser.Id)), Times.Once);
        _loggerServiceMock.Verify(l => l.LogCreateAgentAccount(It.IsAny<string>(), It.IsAny<string>(), newAgent.Email, It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task GetAgentByEmailAsync_WhenEmailDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var email = "test@example.com";
        _userRepositoryMock.Setup(r => r.GetAgentByEmailAsync(email)).ReturnsAsync((Agent?)null);
        
        // Act
        var act = async () => await _userService.GetAgentByEmailAsync(email);
    
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _userRepositoryMock.Verify(r => r.GetAgentByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetAgentByEmailAsync_WhenEmailExists_ShouldReturnSearchAgentResponseDto()
    {
        // Arrange
        var email = "test@example.com";
        var agent = new Agent { User = new User { Email = email } };
        _userRepositoryMock.Setup(r => r.GetAgentByEmailAsync(email)).ReturnsAsync(agent);
        
        var expectedResult = new SearchAgentResponseDto { Email = email };
        _mapperMock.Setup(m => m.Map<SearchAgentResponseDto>(agent)).Returns(expectedResult);

        // Act
        var result = await _userService.GetAgentByEmailAsync(email);
    
        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        result.Should().BeOfType<SearchAgentResponseDto>();

        _userRepositoryMock.Verify(r => r.GetAgentByEmailAsync(email), Times.Once);
        _mapperMock.Verify(m => m.Map<SearchAgentResponseDto>(agent), Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(2, 0)]
    [InlineData(2, -1)]
    [InlineData(-1, -1)]
    public async Task GetAgentsAsync_WhenArgumentsAreInvalid_ShouldThrowArgumentErrorException(int pageNumber, int pageSize)
    {
        // Act
        var act = async () => await _userService.GetAgentsAsync(pageNumber, pageSize);
    
        // Assert
        await act.Should().ThrowAsync<ArgumentErrorException>();
    }

    [Fact]
    public async Task GetAgentsAsync_WhenNoAgentsExist_ShouldReturnNotFoundException()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        _userRepositoryMock.Setup(r => r.GetAgentsAsync(pageNumber, pageSize)).ReturnsAsync(new List<Agent>());
        
        // Act
        var act = async () => await _userService.GetAgentsAsync(pageNumber, pageSize);
    
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _userRepositoryMock.Verify(r => r.GetAgentsAsync(pageNumber, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetAgentsAsync_WhenAgentsExist_ShouldReturnPagedResult()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var agents = new List<Agent> 
        { 
            new Agent { Id = "1" },
            new Agent { Id = "2" }
        };
        _userRepositoryMock.Setup(r => r.GetAgentsAsync(pageNumber, pageSize)).ReturnsAsync(agents);
        _userRepositoryMock.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(2);

        var agentsDto = new List<SearchAgentResponseDto> 
        { 
            new SearchAgentResponseDto { Id = "1" },
            new SearchAgentResponseDto { Id = "2" }
        };
        _mapperMock.Setup(m => m.Map<IEnumerable<SearchAgentResponseDto>>(agents)).Returns(agentsDto);

        // Act
        var result = await _userService.GetAgentsAsync(pageNumber, pageSize);
    
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PagedResult<SearchAgentResponseDto>>();
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(2);
        result.Items.Should().BeEquivalentTo(agentsDto);

        _userRepositoryMock.Verify(r => r.GetAgentsAsync(pageNumber, pageSize), Times.Once);
        _userRepositoryMock.Verify(r => r.GetTotalCountAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<SearchAgentResponseDto>>(agents), Times.Once);
    }

    [Fact]
    public async Task GetAgentsUnderPhotographyCompanyAsync_WhenPhotographyCompanyDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var photographyCompanyId = "1";
        _userRepositoryMock.Setup(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId)).ReturnsAsync((PhotographyCompany?)null);
        
        // Act
        var act = async () => await _userService.GetAgentsUnderPhotographyCompanyAsync(photographyCompanyId);
    
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _userRepositoryMock.Verify(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId), Times.Once);
    }

    [Fact]
    public async Task GetAgentsUnderPhotographyCompanyAsync_WhenPhotographyCompanyExists_ShouldReturnSearchAgentResponseDtoLists()
    {
        // Arrange
        var photographyCompanyId = "1";
        var photographyCompany = new PhotographyCompany { Id = photographyCompanyId };
        _userRepositoryMock.Setup(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId)).ReturnsAsync(photographyCompany);
        
        var agents = new List<Agent> 
        { 
            new Agent { Id = "1" },
            new Agent { Id = "2" }
        };
        _userRepositoryMock.Setup(r => r.GetAgentsUnderPhotographyCompanyAsync(photographyCompanyId)).ReturnsAsync(agents);
        
        IEnumerable<SearchAgentResponseDto> agentsDto = new List<SearchAgentResponseDto>
        { 
            new SearchAgentResponseDto { Id = "1" },
            new SearchAgentResponseDto { Id = "2" }
        };
        _mapperMock.Setup(m => m.Map<IEnumerable<SearchAgentResponseDto>>(agents)).Returns(agentsDto);

        // Act
        var result = await _userService.GetAgentsUnderPhotographyCompanyAsync(photographyCompanyId);
    
        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(2);

        _userRepositoryMock.Verify(r => r.FindPhotographyCompanyByIdAsync(photographyCompanyId), Times.Once);
        _userRepositoryMock.Verify(r => r.GetAgentsUnderPhotographyCompanyAsync(photographyCompanyId), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<SearchAgentResponseDto>>(agents), Times.Once);
    }

    [Fact]
    public async Task GetUserListingCaseIdsAsync_WhenUserIsPhotographyCompanyButDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = "1";
        var userRole = RoleNames.PhotographyCompany;
        _userRepositoryMock.Setup(r => r.FindPhotographyCompanyByIdAsync(userId)).ReturnsAsync((PhotographyCompany?)null);
        
        // Act
        var act = async () => await _userService.GetUserListingCaseIdsAsync(userId, userRole);
    
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _userRepositoryMock.Verify(r => r.FindPhotographyCompanyByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserListingCaseIdsAsync_WhenUserIsPhotographyCompanyAndExists_ShouldReturnCaseIds()
    {
        // Arrange
        var userId = "1";
        var userRole = RoleNames.PhotographyCompany;
        var photographyCompany = new PhotographyCompany { Id = userId };
        _userRepositoryMock.Setup(r => r.FindPhotographyCompanyByIdAsync(userId)).ReturnsAsync(photographyCompany);
        
        var listingCaseIds = new List<int> { 1, 2 };
        _userRepositoryMock.Setup(r => r.GetPhotographyCompanyListingCaseIdsAsync(userId)).ReturnsAsync(listingCaseIds);
    
        // Act
        var result = await _userService.GetUserListingCaseIdsAsync(userId, userRole);
    
        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(2);

        _userRepositoryMock.Verify(r => r.FindPhotographyCompanyByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(r => r.GetPhotographyCompanyListingCaseIdsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserListingCaseIdsAsync_WhenUserIsAgentButDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = "1";
        var userRole = RoleNames.Agent;
        _userRepositoryMock.Setup(r => r.FindAgentByIdAsync(userId)).ReturnsAsync((Agent?)null);
        
        // Act
        var act = async () => await _userService.GetUserListingCaseIdsAsync(userId, userRole);
    
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _userRepositoryMock.Verify(r => r.FindAgentByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserListingCaseIdsAsync_WhenUserIsAgentAndExists_ShouldReturnCaseIds()
    {
        // Arrange
        var userId = "1";
        var userRole = RoleNames.Agent;
        var agent = new Agent { Id = userId };
        _userRepositoryMock.Setup(r => r.FindAgentByIdAsync(userId)).ReturnsAsync(agent);
        
        var listingCaseIds = new List<int> { 1, 2 };
        _userRepositoryMock.Setup(r => r.GetAgentListingCaseIdsAsync(userId)).ReturnsAsync(listingCaseIds);
    
        // Act
        var result = await _userService.GetUserListingCaseIdsAsync(userId, userRole);
    
        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(2);

        _userRepositoryMock.Verify(r => r.FindAgentByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(r => r.GetAgentListingCaseIdsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = "1";
        var oldPassword = "oldPassword";
        var newPassword = "newPassword";
        var request = new UpdatePasswordRequestDto 
        { 
            OldPassword = oldPassword, 
            NewPassword = newPassword,
            ConfirmNewPassword = newPassword
        };
        _userManagerMock.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync((User?)null);
        
        // Act
        var act = async () => await _userService.UpdatePasswordAsync(request, userId);
    
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _userManagerMock.Verify(r => r.FindByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenOldPasswordIsIncorrect_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var userId = "1";
        var oldPassword = "oldPassword";
        var newPassword = "newPassword";
        var request = new UpdatePasswordRequestDto 
        { 
            OldPassword = oldPassword, 
            NewPassword = newPassword,
            ConfirmNewPassword = newPassword
        };
        
        var user = new User { Id = userId };
        _userManagerMock.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync(user);
        
        _userManagerMock.Setup(r => r.CheckPasswordAsync(user, oldPassword)).ReturnsAsync(false);

        // Act
        var act = async () => await _userService.UpdatePasswordAsync(request, userId);
    
        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
        _userManagerMock.Verify(r => r.FindByIdAsync(userId), Times.Once);
        _userManagerMock.Verify(r => r.CheckPasswordAsync(user, oldPassword), Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUpdatePasswordFails_ShouldThrowDbErrorException()
    {
        // Arrange
        var userId = "1";
        var oldPassword = "oldPassword";
        var newPassword = "newPassword";
        var request = new UpdatePasswordRequestDto 
        { 
            OldPassword = oldPassword, 
            NewPassword = newPassword,
            ConfirmNewPassword = newPassword
        };
        
        var user = new User { Id = userId };
        _userManagerMock.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync(user);
        
        _userManagerMock.Setup(r => r.CheckPasswordAsync(user, oldPassword)).ReturnsAsync(true);
        
        _userManagerMock.Setup(r => r.ChangePasswordAsync(user, oldPassword, newPassword)).ReturnsAsync(IdentityResult.Failed());

        // Act
        var act = async () => await _userService.UpdatePasswordAsync(request, userId);
    
        // Assert
        await act.Should().ThrowAsync<DbErrorException>();
        _userManagerMock.Verify(r => r.FindByIdAsync(userId), Times.Once);
        _userManagerMock.Verify(r => r.CheckPasswordAsync(user, oldPassword), Times.Once);
        _userManagerMock.Verify(r => r.ChangePasswordAsync(user, oldPassword, newPassword), Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenAllStepsAreSucceed_ShouldReturnUpdatePasswordResponseDto()
    {
        // Arrange
        var userId = "1";
        var oldPassword = "oldPassword";
        var newPassword = "newPassword";
        var request = new UpdatePasswordRequestDto 
        { 
            OldPassword = oldPassword, 
            NewPassword = newPassword,
            ConfirmNewPassword = newPassword
        };
        
        var user = new User { Id = userId };
        _userManagerMock.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync(user);
        
        _userManagerMock.Setup(r => r.CheckPasswordAsync(user, oldPassword)).ReturnsAsync(true);
        
        _userManagerMock.Setup(r => r.ChangePasswordAsync(user, oldPassword, newPassword)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _userService.UpdatePasswordAsync(request, userId);
    
        // Assert
        _userManagerMock.Verify(r => r.FindByIdAsync(userId), Times.Once);
        _userManagerMock.Verify(r => r.CheckPasswordAsync(user, oldPassword), Times.Once);
        _userManagerMock.Verify(r => r.ChangePasswordAsync(user, oldPassword, newPassword), Times.Once);
    }
}
