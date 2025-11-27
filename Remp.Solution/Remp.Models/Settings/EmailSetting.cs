namespace Remp.Models.Settings;

public class EmailSetting
{
    public string SmtpServer { get; set; } = null!;
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}
