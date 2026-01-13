using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Remp.DataAccess.Data;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Repository.Repositories;

namespace Remp.UnitTests.Repository;

public class AuthRepositoryTest
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly AuthRepository _authRepository;

    public AuthRepositoryTest()
    {
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "AuthRepositoryTestDb")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings
            .Options;

        _dbContext = new AppDbContext(dbOptions);
        
        var store = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

        _authRepository = new AuthRepository(_dbContext, _userManagerMock.Object);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task CheckPasswordAsync_WhenPasswordIsCorrect_ShouldReturnTrue()
    {
        // Arrange
        var user = new User { UserName = "testuser", PasswordHash = "hashedPassword" };
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "hashedPassword")).ReturnsAsync(true);

        // Act
        var result = await _authRepository.CheckPasswordAsync(user, "hashedPassword");
        
        // Assert
        result.Should().BeTrue();
        _userManagerMock.Verify(u => u.CheckPasswordAsync(user, "hashedPassword"), Times.Once);
    }

    [Fact]
    public async Task CheckPasswordAsync_WhenPasswordIsIncorrect_ShouldReturnFalse()
    {
        // Arrange
        var user = new User { UserName = "test@exmaple.com", PasswordHash = "hashedPassword" };
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "incorrectPassword")).ReturnsAsync(false);

        // Act
        var result = await _authRepository.CheckPasswordAsync(user, "incorrectPassword");
        
        // Assert
        result.Should().BeFalse();
        _userManagerMock.Verify(u => u.CheckPasswordAsync(user, "incorrectPassword"), Times.Once);
    }

    [Fact]
    public async Task CheckPasswordAsync_WhenUserDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var user = new User { UserName = "test@exmaple.com", PasswordHash = "hashedPassword" };
        _userManagerMock.Setup(u => u.CheckPasswordAsync(null, "hashedPassword")).ReturnsAsync(false);

        // Act
        var result = await _authRepository.CheckPasswordAsync(null, "hashedPassword");
        
        // Assert
        result.Should().BeFalse();
        _userManagerMock.Verify(u => u.CheckPasswordAsync(null, "hashedPassword"), Times.Once);
    }

    [Fact]
    public async Task CreateAgentAsync_WhenAgentIsCreated_ShouldSaveToDatabase()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };
        var photographyCompany = new PhotographyCompany
        {
            Id = Guid.NewGuid().ToString(),
            PhotographyCompanyName = "Test Photography Company",
        };
        var password = "password";

        _userManagerMock.Setup(u => u.CreateAsync(user, password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(u => u.AddToRoleAsync(user, RoleNames.PhotographyCompany)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _authRepository.CreateAgentAsync(user, photographyCompany, password, RoleNames.PhotographyCompany);
        
        // Assert
        var createdAgent = await _dbContext.PhotographyCompanies.FirstOrDefaultAsync(pc => pc.Id == photographyCompany.Id);
        createdAgent.Should().BeEquivalentTo(photographyCompany);
        _userManagerMock.Verify(u => u.CreateAsync(user, password), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(user, RoleNames.PhotographyCompany), Times.Once);
    }

    [Fact]
    public async Task CreateAgentAsync_WhenSaveUserFails_ShouldThrowException()
    {
        // Arrange
        var user = new User { Email = "test@example.com", UserName = "test@example.com" };
        var photographyCompany = new PhotographyCompany
        {
            Id = Guid.NewGuid().ToString(),
            PhotographyCompanyName = "Test Photography Company",
        };
        var password = "password";

        _userManagerMock.Setup(u => u.CreateAsync(user, password)).ReturnsAsync(IdentityResult.Failed());
        _userManagerMock.Setup(u => u.AddToRoleAsync(user, RoleNames.PhotographyCompany)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = async() => await _authRepository.CreateAgentAsync(user, photographyCompany, password, RoleNames.PhotographyCompany);
        
        // Assert
        await result.Should().ThrowAsync<Exception>();
        _userManagerMock.Verify(u => u.CreateAsync(user, password), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(user, RoleNames.PhotographyCompany), Times.Never);
    }

    [Fact]
    public async Task CreateAgentAsync_WhenSaveRoleFails_ShouldThrowException()
    {
        // Arrange
        var user = new User { Email = "test@example.com", UserName = "test@example.com" };
        var photographyCompany = new PhotographyCompany
        {
            Id = Guid.NewGuid().ToString(),
            PhotographyCompanyName = "Test Photography Company",
        };
        var password = "password";

        _userManagerMock.Setup(u => u.CreateAsync(user, password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(u => u.AddToRoleAsync(user, RoleNames.PhotographyCompany)).ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = async() => await _authRepository.CreateAgentAsync(user, photographyCompany, password, RoleNames.PhotographyCompany);
        
        // Assert
        await result.Should().ThrowAsync<Exception>();
        _userManagerMock.Verify(u => u.CreateAsync(user, password), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(user, RoleNames.PhotographyCompany), Times.Once);
    }

    [Fact]
    public async Task FindByEmailAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Email = "test@example.com", UserName = "test@example.com" };
        _userManagerMock.Setup(u => u.FindByEmailAsync("test@example.com")).ReturnsAsync(user);

        // Act
        var result = await _authRepository.FindByEmailAsync("test@example.com");
        
        // Assert
        result.Should().BeEquivalentTo(user);
        _userManagerMock.Verify(u => u.FindByEmailAsync("test@example.com"), Times.Once);
    }

    [Fact]
    public async Task FindByEmailAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByEmailAsync("test@example.com")).ReturnsAsync((User?)null);

        // Act
        var result = await _authRepository.FindByEmailAsync("test@example.com");
        
        // Assert
        result.Should().BeNull();
        _userManagerMock.Verify(u => u.FindByEmailAsync("test@example.com"), Times.Once);
    }
}
