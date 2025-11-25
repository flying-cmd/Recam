namespace Remp.Service.DTOs;

public class UserInfoResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public IEnumerable<int> ListingCaseIds { get; set; } = [];
}
