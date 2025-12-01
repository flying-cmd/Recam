using Serilog;

namespace Remp.Common.Helpers;

public static class CaseHistoryLog
{
    public static void LogCreateListingCase(string? listingCaseId, string userId, string? description = null)
    {
        var details = $"ListingCase {listingCaseId} created by User {userId}";
        Log.ForContext("LogType", "CaseHistory")
            .ForContext("EventType", "Create")
            .ForContext("ListingCaseId", listingCaseId)
            .ForContext("UserId", userId)
            .Information(description ?? details);
    }

    public static void LogUpdateListingCase(string? listingCaseId, string userId, Dictionary<string, FieldChange>? updatedFields, string? description = null)
    {
        var details = $"ListingCase {listingCaseId} updated by User {userId}";
        Log.ForContext("LogType", "CaseHistory")
            .ForContext("EventType", "Update")
            .ForContext("ListingCaseId", listingCaseId)
            .ForContext("UserId", userId)
            .ForContext("updatedFields", updatedFields != null && updatedFields.Count > 0 ? updatedFields : "No fields updated", true)
            .Information(description ?? details);
    }

    public static void LogDeleteListingCase(string? listingCaseId, string userId, string? description = null)
    {
        var details = $"ListingCase {listingCaseId} deleted by User {userId}";
        Log.ForContext("LogType", "CaseHistory")
            .ForContext("EventType", "Delete")
            .ForContext("ListingCaseId", listingCaseId)
            .ForContext("UserId", userId)
            .Information(description ?? details);
    }

    public static void LogUpdateListingCaseStatus(string? listingCaseId, string userId, string? oldStatus, string? newStatus, string? description = null)
    {
        var details = $"ListingCase {listingCaseId} status updated from {oldStatus} to {newStatus} by User {userId}";
        Log.ForContext("LogType", "CaseHistory")
            .ForContext("EventType", "UpdateStatus")
            .ForContext("ListingCaseId", listingCaseId)
            .ForContext("OldStatus", oldStatus)
            .ForContext("NewStatus", newStatus)
            .ForContext("UserId", userId)
            .Information(description ?? details);
    }

    public static void LogDeleteMedia(string? mediaId, string userId, string? description = null)
    {
        var details = $"Media {mediaId} deleted by User {userId}";
        Log.ForContext("LogType", "CaseHistory")
            .ForContext("EventType", "DeleteMedia")
            .ForContext("MediaId", mediaId)
            .ForContext("UserId", userId)
            .Information(description ?? details);
    }

    public static void LogSelectMedia(string? listingCaseId, IEnumerable<int> mediaIds, string userId, string? description = null)
    {
        var details = $"User {userId} selected media {string.Join(", ", mediaIds)} for listing case {listingCaseId}";
        Log.ForContext("LogType", "CaseHistory")
            .ForContext("EventType", "SelectMedia")
            .ForContext("UserId", userId)
            .ForContext("ListingCaseId", listingCaseId)
            .ForContext("MediaId", string.Join(", ", mediaIds))
            .Information(description ?? details);
    }
}
