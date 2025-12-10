using Remp.Common.Exceptions;
using Remp.Models.Enums;

namespace Remp.Common.Utilities;

public static class ListingCaseStatusExtensions
{
    public static ListingCaseStatus NextStatus(this ListingCaseStatus currentStatus) => currentStatus switch
    {
        ListingCaseStatus.Created => ListingCaseStatus.Pending,
        ListingCaseStatus.Pending => ListingCaseStatus.Delivered,
        ListingCaseStatus.Delivered => throw new ArgumentErrorException(
            message: $"Listing case status cannot be updated because it is already delivered",
            title: "Listing case status cannot be updated because it is already delivered"),
        _ => throw new ArgumentOutOfRangeException(nameof(currentStatus), currentStatus, "Unknown listing case status.")
    };
}
