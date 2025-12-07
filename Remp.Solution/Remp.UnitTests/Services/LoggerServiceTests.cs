using Moq;
using Remp.Common.Helpers;
using Remp.DataAccess.Collections;
using Remp.Repository.Interfaces;
using Remp.Service.Interfaces;
using Remp.Service.Services;

namespace Remp.UnitTests.Services;

public class LoggerServiceTests
{
    private readonly Mock<ILoggerRepository> _loggerRepositoryMock;
    private readonly ILoggerService _loggerService;

    public LoggerServiceTests()
    {
        _loggerRepositoryMock = new Mock<ILoggerRepository>();
        _loggerService = new LoggerService(_loggerRepositoryMock.Object);
    }

    [Fact]
    public async Task LogLogin_WhenNoError_ShouldLogSuccessUserActivity()
    {
        // Arrange
        var userId = "1";
        var email = "test@example.com";
        var actionName = "Login";
        var message = $"User {userId} logged in successfully";
        var EventType = "Login";
        _loggerRepositoryMock
            .Setup(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == userId &&
                ua.Email == email &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == null &&
                ua.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogLogin(email: email, userId: userId, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == userId &&
                ua.Email == email &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == null &&
                ua.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogLogin_WhenError_ShouldLogErrorUserActivity()
    {
        // Arrange
        var email = "test@example.com";
        var actionName = "Login";
        var error = "An error occurred when logging in";
        var message = $"User failed to login with error: {error}";
        var EventType = "Login";
        _loggerRepositoryMock
            .Setup(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.Email == email &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == error &&
                ua.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogLogin(email: email, userId: null, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.Email == email &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == error &&
                ua.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogRegister_WhenNoError_ShouldLogSuccessUserActivity()
    {
        // Arrange
        var userId = "1";
        var email = "test@example.com";
        var actionName = "Register";
        var message = $"User {userId} registered successfully";
        var EventType = "Register";
        _loggerRepositoryMock
            .Setup(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == userId &&
                ua.Email == email &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == null &&
                ua.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogRegister(email: email, userId: userId, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == userId &&
                ua.Email == email &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == null &&
                ua.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogRegister_WhenError_ShouldLogErrorUserActivity()
    {
        // Arrange
        var email = "test@example.com";
        var actionName = "Register";
        var error = "An error occurred when registering";
        var message = $"User failed to register with error: {error}";
        var EventType = "Register";
        _loggerRepositoryMock
            .Setup(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.Email == email &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == error &&
                ua.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogRegister(email: email, userId: null, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.Email == email &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == error &&
                ua.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogCreateAgentAccount_WhenNoError_ShouldLogSuccessUserActivity()
    {
        // Arrange
        var photographyCompanyId = "1";
        var createdAgentId = "2";
        var createdAgentEmail = "test@example.com";
        var actionName = "CreateAgentAccount";
        var message = $"Agent {createdAgentId} created by PhotographyCompany {photographyCompanyId} successfully";
        var EventType = "Create";
        _loggerRepositoryMock
            .Setup(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == photographyCompanyId &&
                ua.CreatedAgentId == createdAgentId &&
                ua.CreatedAgentEmail == createdAgentEmail &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == null &&
                ua.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogCreateAgentAccount(photographyCompanyId: photographyCompanyId, createdAgentId: createdAgentId, createdAgentEmail: createdAgentEmail, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == photographyCompanyId &&
                ua.CreatedAgentId == createdAgentId &&
                ua.CreatedAgentEmail == createdAgentEmail &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == null &&
                ua.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogCreateAgentAccount_WhenError_ShouldLogErrorUserActivity()
    {
        // Arrange
        var photographyCompanyId = "1";
        var createdAgentId = "2";
        var createdAgentEmail = "test@example.com";
        var actionName = "CreateAgentAccount";
        var error = "An error occurred when creating agent account";
        var message = $"Agent failed to be created by PhotographyCompany {photographyCompanyId} with error: {error}";
        var EventType = "Create";
        _loggerRepositoryMock
            .Setup(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == photographyCompanyId &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == error &&
                ua.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        // Act
        await _loggerService.LogCreateAgentAccount(photographyCompanyId: photographyCompanyId, createdAgentId: createdAgentId, createdAgentEmail: createdAgentEmail, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == photographyCompanyId &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == error &&
                ua.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogUpdatePassword_WhenNoError_ShouldLogSuccessUserActivity()
    {
        // Arrange
        var userId = "1";
        var actionName = "UpdatePassword";
        var message = $"User {userId} updated password successfully";
        var EventType = "UpdatePassword";
        _loggerRepositoryMock
            .Setup(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == userId &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == null &&
                ua.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogUpdatePassword(userId: userId, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == userId &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == null &&
                ua.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogUpdatePassword_WhenError_ShouldLogErrorUserActivity()
    {
        // Arrange
        var userId = "1";
        var actionName = "UpdatePassword";
        var error = "An error occurred when updating password";
        var message = $"User {userId} failed to update password with error: {error}";
        var EventType = "UpdatePassword";
        _loggerRepositoryMock
            .Setup(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == userId &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == error &&
                ua.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogUpdatePassword(userId: userId, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogUserActivityAsync(
                It.Is<UserActivity>(ua =>
                ua.UserId == userId &&
                ua.EventType == EventType &&
                ua.Message == message &&
                ua.Error == error &&
                ua.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogCreateListingCase_WhenNoError_ShouldLogSuccessCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var actionName = "CreateListingCase";
        var message = $"ListingCase {listingCaseId} created by User {userId}";
        var EventType = "Create";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == null &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogCreateListingCase(listingCaseId: listingCaseId.ToString(), userId: userId, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == null &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogCreateListingCase_WhenError_ShouldLogErrorCaseHistory()
    {
        // Arrange
        var userId = "1";
        var actionName = "CreateListingCase";
        var error = "An error occurred when creating listing case";
        var message = $"ListingCase failed to be created by User {userId} with error: {error}";
        var EventType = "Create";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == null &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == error &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogCreateListingCase(listingCaseId: null, userId: userId, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == null &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == error &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogUpdateListingCase_WhenNoErrorAndUpdatedFieldsAreNotNullOrEmpty_ShouldLogSuccessCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var actionName = "UpdateListingCase";
        var message = $"ListingCase {listingCaseId} updated by User {userId} successfully";
        var EventType = "Update";
        var updatedFields = new Dictionary<string, FieldChange>
        {
            { "Title", new FieldChange("Old Value 1", "New Value 1") },
            { "Description", new FieldChange("Old Value 2", "New Value 2") }
        };
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.UpdatedFields == updatedFields &&
                ch.Error == null &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogUpdateListingCase(listingCaseId: listingCaseId.ToString(), userId: userId, updatedFields: updatedFields, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.UpdatedFields == updatedFields &&
                ch.Error == null &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogUpdateListingCase_WhenNoErrorAndUpdatedFieldsAreNull_ShouldLogNoFieldsUpdate()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var actionName = "UpdateListingCase";
        var message = $"No fields need to be updated for ListingCase {listingCaseId} by User {userId}";
        var EventType = "Update";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.UpdatedFields == null &&
                ch.Error == null &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogUpdateListingCase(listingCaseId: listingCaseId.ToString(), userId: userId, updatedFields: null, actionName: actionName, error: null);


        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.UpdatedFields == null &&
                ch.Error == null &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogUpdateListingCase_WhenNoErrorAndUpdatedFieldsAreEmpty_ShouldLogNoFieldsUpdate()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var actionName = "UpdateListingCase";
        var message = $"No fields need to be updated for ListingCase {listingCaseId} by User {userId}";
        var EventType = "Update";
        var updatedFields = new Dictionary<string, FieldChange>();
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == null &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogUpdateListingCase(listingCaseId: listingCaseId.ToString(), userId: userId, updatedFields: updatedFields, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == null &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogUpdateListingCase_WhenError_ShouldLogErrorCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var actionName = "UpdateListingCase";
        var error = "An error occurred when updating listing case";
        var message = $"ListingCase {listingCaseId} failed to be updated by User {userId} with error: {error}";
        var EventType = "Update";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == error &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogUpdateListingCase(listingCaseId: listingCaseId.ToString(), userId: userId, updatedFields: null, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == error &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogDeleteListingCase_WhenNoError_ShouldLogSuccessCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var actionName = "DeleteListingCase";
        var message = $"ListingCase {listingCaseId} deleted by User {userId} successfully";
        var EventType = "Delete";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == null &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogDeleteListingCase(listingCaseId: listingCaseId.ToString(), userId: userId, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == null &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogDeleteListingCase_WhenError_ShouldLogErrorCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var actionName = "DeleteListingCase";
        var error = "An error occurred when deleting listing case";
        var message = $"ListingCase {listingCaseId} failed to be deleted by User {userId} with error: {error}";
        var EventType = "Delete";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == error &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogDeleteListingCase(listingCaseId: listingCaseId.ToString(), userId: userId, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == error &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogUpdateListingCaseStatus_WhenNoError_ShouldLogSuccessCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var actionName = "UpdateListingCaseStatus";
        var oldStatus = "Pending";
        var newStatus = "Delivered";
        var message = $"ListingCase {listingCaseId} status updated from {oldStatus} to {newStatus} by User {userId} successfully"; ;
        var EventType = "UpdateStatus";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.OldStatus == oldStatus &&
                ch.NewStatus == newStatus &&
                ch.Error == null &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogUpdateListingCaseStatus(listingCaseId: listingCaseId.ToString(), userId: userId, oldStatus: oldStatus, newStatus: newStatus, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.OldStatus == oldStatus &&
                ch.NewStatus == newStatus &&
                ch.Error == null &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogUpdateListingCaseStatus_WhenError_ShouldLogErrorCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        var userId = "1";
        var actionName = "UpdateListingCaseStatus";
        var oldStatus = "Created";
        var newStatus = "Delivered";
        var error = "An error occurred when updating listing case status";
        var message = $"ListingCase {listingCaseId} status failed to be updated from {oldStatus} to {newStatus} by User {userId} with error: {error}";
        var EventType = "UpdateStatus";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.OldStatus == oldStatus &&
                ch.NewStatus == newStatus &&
                ch.Error == error &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogUpdateListingCaseStatus(listingCaseId: listingCaseId.ToString(), userId: userId, oldStatus: oldStatus, newStatus: newStatus, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.OldStatus == oldStatus &&
                ch.NewStatus == newStatus &&
                ch.Error == error &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogDeleteMedia_WhenNoError_ShouldLogSuccessCaseHistory()
    {
        // Arrange
        var mediaId = 1;
        var userId = "1";
        var actionName = "DeleteMedia";
        var message = $"Media {mediaId} deleted by User {userId} successfully";
        var EventType = "Delete";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.MediaId == mediaId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == null &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogDeleteMedia(mediaId: mediaId.ToString(), userId: userId, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.MediaId == mediaId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == null &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogDeleteMedia_WhenError_ShouldLogErrorCaseHistory()
    {
        // Arrange
        var mediaId = 1;
        var userId = "1";
        var actionName = "DeleteMedia";
        var error = "An error occurred when deleting media";
        var message = $"Media {mediaId} failed to be deleted by User {userId} with error: {error}";
        var EventType = "Delete";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.MediaId == mediaId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == error &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogDeleteMedia(mediaId: mediaId.ToString(), userId: userId, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.MediaId == mediaId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.Error == error &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogSelectMedia_WhenNoError_ShouldLogSuccessCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        IEnumerable<int> mediaIds = [1, 2, 3, 4];
        var userId = "1";
        var actionName = "SelectMedia";
        var message = $"User {userId} successfully selected media {string.Join(", ", mediaIds)} for listing case {listingCaseId}";
        var EventType = "SelectMedia";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.MediaIds == mediaIds &&
                ch.Error == null &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogSelectMedia(listingCaseId: listingCaseId.ToString(), mediaIds: mediaIds, userId: userId, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.MediaIds == mediaIds &&
                ch.Error == null &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogSelectMedia_WhenError_ShouldLogErrorCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        IEnumerable<int> mediaIds = [1, 2, 3, 4];
        var userId = "1";
        var actionName = "SelectMedia";
        var error = "An error occurred when selecting media";
        var message = $"User {userId} failed to select media {string.Join(", ", mediaIds)} for listing case {listingCaseId} with error: {error}";
        var EventType = "SelectMedia";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.MediaIds == mediaIds &&
                ch.Error == error &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogSelectMedia(listingCaseId: listingCaseId.ToString(), mediaIds: mediaIds, userId: userId, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == userId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.MediaIds == mediaIds &&
                ch.Error == error &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogAddAgentToListingCase_WhenNoError_ShouldLogSuccessCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        var agentId = "1";
        var photographyCompanyId = "5";
        var actionName = "AddAgentToListingCase";
        var message = $"Photography company {photographyCompanyId} successfully added agent {agentId} to listing case {listingCaseId}";
        var EventType = "AddAgentToListingCase";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == photographyCompanyId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.AgentId == agentId &&
                ch.Error == null &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogAddAgentToListingCase(listingCaseId: listingCaseId.ToString(), agentId: agentId, photographyCompanyId: photographyCompanyId, actionName: actionName, error: null);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == photographyCompanyId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.AgentId == agentId &&
                ch.Error == null &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }

    [Fact]
    public async Task LogAddAgentToListingCase_WhenError_ShouldLogErrorCaseHistory()
    {
        // Arrange
        var listingCaseId = 1;
        var agentId = "1";
        var photographyCompanyId = "5";
        var actionName = "AddAgentToListingCase";
        var error = "An error occurred when adding agent to listing case";
        var message = $"Photography company {photographyCompanyId} failed to add agent {agentId} to listing case {listingCaseId} with error: {error}";
        var EventType = "AddAgentToListingCase";
        _loggerRepositoryMock
            .Setup(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == photographyCompanyId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.AgentId == agentId &&
                ch.Error == error &&
                ch.ActionName == actionName
                )))
            .Returns(Task.CompletedTask);

        // Act
        await _loggerService.LogAddAgentToListingCase(listingCaseId: listingCaseId.ToString(), agentId: agentId, photographyCompanyId: photographyCompanyId, actionName: actionName, error: error);

        // Assert
        _loggerRepositoryMock
            .Verify(r => r.AddLogCaseHistoryAsync(
                It.Is<CaseHistory>(ch =>
                ch.ListingCaseId == listingCaseId.ToString() &&
                ch.UserId == photographyCompanyId &&
                ch.EventType == EventType &&
                ch.Message == message &&
                ch.AgentId == agentId &&
                ch.Error == error &&
                ch.ActionName == actionName
                )),
                Times.Once);
    }
}
