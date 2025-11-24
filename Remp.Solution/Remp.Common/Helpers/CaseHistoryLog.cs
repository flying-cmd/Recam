using Serilog;

namespace Remp.Common.Helpers;

public static class CaseHistoryLog
{
    public static void LogCreateListingCase(string? listingCaseId, string userId, string? description = null)
    {
        var details = $"ListingCase {listingCaseId} created by User {userId}";
        Log.ForContext("LogType", "CaseHistory")
            .ForContext("ListingCaseId", listingCaseId)
            .ForContext("UserId", userId)
            .Information(description ?? details);
    }
}
