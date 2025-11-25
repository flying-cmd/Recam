namespace Remp.Service.DTOs;

public class DeleteListingCaseResponseDto
{
    public string Message { get; set; } = null!;

    public DeleteListingCaseResponseDto(string message = "Listing case deleted successfully.")
    {
        Message = message;
    }
}
