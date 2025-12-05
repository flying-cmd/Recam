using Remp.Common.Helpers;
using Remp.DataAccess.Collections;
using Remp.Repository.Interfaces;
using Remp.Service.Interfaces;
using System.Runtime.CompilerServices;

namespace Remp.Service.Services;

public class LoggerService : ILoggerService
{
    private readonly ILoggerRepository _loggerRepository;

    public LoggerService(ILoggerRepository loggerRepository)
    {
        _loggerRepository = loggerRepository;
    }

    public async Task LogLogin(string? email, string? userId, string actionName, string? message = null, string? error = null)
    {
        var userActivity = new UserActivity();
        string details;

        if (error == null)
        {
            details = $"User {userId} logged in successfully";
            userActivity = new UserActivity(
                userId: userId,
                email: email,
                eventType: "Login",
                message: message ?? details,
                actionName: actionName
                );
        }
        else
        {
            details = $"User failed to login with error: {error}";
            userActivity = new UserActivity(
                email: email,
                eventType: "Login",
                message: message ?? details,
                error: error!,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogUserActivityAsync(userActivity);
    }

    public async Task LogRegister(string? email, string? userId, string actionName, string? message = null, string? error = null)
    {
        var userActivity = new UserActivity();
        string details;

        if (error == null)
        {
            details = $"User {userId} registered successfully";
            userActivity = new UserActivity(
                userId: userId,
                email: email,
                eventType: "Register",
                message: message ?? details,
                actionName: actionName
                );
        }
        else
        {
            details = $"User failed to register with error: {error}";
            userActivity = new UserActivity(
                email: email,
                eventType: "Register",
                message: message ?? details,
                error: error!,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogUserActivityAsync(userActivity);
    }

    public async Task LogCreateAgentAccount(string photographyCompanyId, string? createdAgentId, string createdAgentEmail, string actionName, string? message = null, string? error = null)
    {
        var userActivity = new UserActivity();
        string details;

        if (error == null)
        {
            details = $"Agent {createdAgentId} created by PhotographyCompany {photographyCompanyId} successfully";
            userActivity = new UserActivity(
                userId: photographyCompanyId,
                createdAgentId: createdAgentId,
                createdAgentEmail: createdAgentEmail,
                eventType: "Create",
                message: message ?? details,
                actionName: actionName
                );
        }
        else
        {
            details = $"Agent failed to be created by PhotographyCompany {photographyCompanyId} with error: {error}";
            userActivity = new UserActivity(
                userId: photographyCompanyId,
                eventType: "Create",
                message: message ?? details,
                error: error!,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogUserActivityAsync(userActivity);
    }

    public async Task LogUpdatePassword(string userId, string actionName, string? message = null, string? error = null)
    {
        var userActivity = new UserActivity();
        string details;

        if (error == null)
        {
            details = $"User {userId} updated password successfully";
            userActivity = new UserActivity(
                userId: userId,
                eventType: "UpdatePassword",
                message: message ?? details,
                actionName: actionName
                );
        }
        else
        {
            details = $"User {userId} failed to update password with error: {error}";
            userActivity = new UserActivity(
                userId: userId,
                eventType: "UpdatePassword",
                message: message ?? details,
                error: error!,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogUserActivityAsync(userActivity);
    }

    public async Task LogCreateListingCase(string? listingCaseId, string userId, string actionName,string? message = null, string? error = null)
    {
        var caseHistory = new CaseHistory();
        string details;

        if (error == null)
        {
            details = $"ListingCase {listingCaseId} created by User {userId}";
            caseHistory = new CaseHistory(
                listingCaseId: listingCaseId,
                userId: userId,
                eventType: "Create",
                message: message ?? details,
                actionName: actionName
                );
        }
        else
        {
            details = $"ListingCase failed to be created by User {userId} with error: {error}";
            caseHistory = new CaseHistory(
                userId: userId,
                eventType: "Create",
                message: message ?? details,
                error: error!,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogCaseHistoryAsync(caseHistory);
    }

    public async Task LogUpdateListingCase(string? listingCaseId, string userId, Dictionary<string, FieldChange>? updatedFields, string actionName, string? message = null, string? error = null)
    {
        var caseHistory = new CaseHistory();
        string details;

        if (error == null)
        {
            if (updatedFields != null && updatedFields.Count > 0)
            {
                details = $"ListingCase {listingCaseId} updated by User {userId} successfully";
                caseHistory = new CaseHistory(
                    listingCaseId: listingCaseId,
                    userId: userId,
                    eventType: "Update",
                    message: message ?? details,
                    updatedFields: updatedFields,
                    actionName: actionName
                    );
            }
            else
            {
                details = $"No fields need to be updated for ListingCase {listingCaseId} by User {userId}";
                caseHistory = new CaseHistory(
                    listingCaseId: listingCaseId,
                    userId: userId,
                    eventType: "Update",
                    message: message ?? details,
                    actionName: actionName
                    );
            }
        }
        else
        {
            details = $"ListingCase {listingCaseId} failed to be updated by User {userId} with error: {error}";
            caseHistory = new CaseHistory(
                listingCaseId: listingCaseId,
                userId: userId,
                eventType: "Update",
                message: message ?? details,
                error: error!,
                updatedFields: updatedFields,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogCaseHistoryAsync(caseHistory);
    }

    public async Task LogDeleteListingCase(string? listingCaseId, string userId, string actionName, string? message = null, string? error = null)
    {
        var caseHistory = new CaseHistory();
        string details;

        if (error == null)
        {
            details = $"ListingCase {listingCaseId} deleted by User {userId} successfully";
            caseHistory = new CaseHistory(
                listingCaseId: listingCaseId,
                userId: userId,
                eventType: "Delete",
                message: message ?? details,
                actionName: actionName
                );
        }
        else
        {
            details = $"ListingCase {listingCaseId} failed to be deleted by User {userId} with error: {error}";
            caseHistory = new CaseHistory(
                listingCaseId: listingCaseId,
                userId: userId,
                eventType: "Delete",
                message: message ?? details,
                error: error!,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogCaseHistoryAsync(caseHistory);
    }

    public async Task LogUpdateListingCaseStatus(string? listingCaseId, string userId, string? oldStatus, string? newStatus, string actionName, string? message = null, string? error = null)
    {
        var caseHistory = new CaseHistory();
        string details;

        if (error == null)
        {
            details = $"ListingCase {listingCaseId} status updated from {oldStatus} to {newStatus} by User {userId} successfully";
            caseHistory = new CaseHistory(
                listingCaseId: listingCaseId,
                userId: userId,
                eventType: "UpdateStatus",
                message: message ?? details,
                oldStatus: oldStatus,
                newStatus: newStatus,
                actionName: actionName
                );
        }
        else
        {
            details = $"ListingCase {listingCaseId} status failed to be updated from {oldStatus} to {newStatus} by User {userId} with error: {error}";
            caseHistory = new CaseHistory(
                listingCaseId: listingCaseId,
                userId: userId,
                eventType: "UpdateStatus",
                message: message ?? details,
                error: error!,
                oldStatus: oldStatus,
                newStatus: newStatus,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogCaseHistoryAsync(caseHistory);
    }

    public async Task LogDeleteMedia(string? mediaId, string userId, [CallerMemberName] string actionName = "", string? message = null, string? error = null)
    {
        var caseHistory = new CaseHistory();
        string details;

        if (error == null)
        {
            details = $"Media {mediaId} deleted by User {userId} successfully";
            caseHistory = new CaseHistory(
                mediaId: mediaId,
                userId: userId,
                eventType: "Delete",
                message: message ?? details,
                actionName: actionName
                );
        }
        else
        {
            details = $"Media {mediaId} failed to be deleted by User {userId} with error: {error}";
            caseHistory = new CaseHistory(
                mediaId: mediaId,
                userId: userId,
                eventType: "Delete",
                message: message ?? details,
                error: error!,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogCaseHistoryAsync(caseHistory);
    }

    public async Task LogSelectMedia(string? listingCaseId, IEnumerable<int> mediaIds, string userId, string actionName, string? message = null, string? error = null)
    {
        var caseHistory = new CaseHistory();
        string details;

        if (error == null)
        {
            details = $"User {userId} successfully selected media {string.Join(", ", mediaIds)} for listing case {listingCaseId}";
            caseHistory = new CaseHistory(
                listingCaseId: listingCaseId,
                userId: userId,
                eventType: "SelectMedia",
                message: message ?? details,
                mediaIds: mediaIds,
                actionName: actionName
                );
        }
        else
        {
            details = $"User {userId} failed to select media {string.Join(", ", mediaIds)} for listing case {listingCaseId} with error: {error}";
            caseHistory = new CaseHistory(
                listingCaseId: listingCaseId,
                userId: userId,
                eventType: "SelectMedia",
                message: message ?? details,
                error: error!,
                mediaIds: mediaIds,
                actionName: actionName
                );
        }

        await _loggerRepository.AddLogCaseHistoryAsync(caseHistory);
    }
}
