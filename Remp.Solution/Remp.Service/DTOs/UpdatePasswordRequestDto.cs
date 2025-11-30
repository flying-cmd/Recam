namespace Remp.Service.DTOs;

public class UpdatePasswordRequestDto
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmNewPassword { get; set; } = null!;
}
