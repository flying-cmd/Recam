namespace Remp.Service.DTOs;

public class DeleteMediaResponseDto
{
    public string Message { get; set; } = null!;

    public DeleteMediaResponseDto(string message = "Media deleted successfully.")
    {
        Message = message;
    }
}
