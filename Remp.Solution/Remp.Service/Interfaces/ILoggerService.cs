using Remp.Common.Helpers;
using Remp.DataAccess.Collections;
using System.Runtime.CompilerServices;

namespace Remp.Service.Interfaces;

public interface ILoggerService
{
    Task LogLogin(string? email, string? userId, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
    Task LogRegister(string? email, string? userId, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
    Task LogCreateAgentAccount(string photographyCompanyId, string? createdAgentId, string createdAgentEmail, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
    Task LogUpdatePassword(string userId, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
    Task LogCreateListingCase(string? listingCaseId, string userId, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
    Task LogUpdateListingCase(string? listingCaseId, string userId, Dictionary<string, FieldChange>? updatedFields, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
    Task LogDeleteListingCase(string? listingCaseId, string userId, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
    Task LogUpdateListingCaseStatus(string? listingCaseId, string userId, string? oldStatus, string? newStatus, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
    Task LogDeleteMedia(string? mediaId, string userId, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
    Task LogSelectMedia(string? listingCaseId, IEnumerable<int> mediaIds, string userId, [CallerMemberName] string actionName = "", string? message = null, string? error = null);
}
