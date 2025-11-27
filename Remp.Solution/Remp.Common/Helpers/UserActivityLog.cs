using Serilog;

namespace Remp.Service.Services;

public static class UserActivityLog
{
    public static void LogLogin(string? email, string? userId, string description = "User logged in")
    {
        Log.ForContext("LogType", "UserActivity")
            .ForContext("EventType", "Login")
            .ForContext("Email", email)
            .ForContext("UserId", userId)
            .Information(description);
    }

    public static void LogRegister(string? email, string? userId, string description = "User registered")
    {
        Log.ForContext("LogType", "UserActivity")
            .ForContext("EventType", "Register")
            .ForContext("Email", email)
            .ForContext("UserId", userId)
            .Information(description);
    }

    public static void LogCreateAgentAccount(string photographyCompanyId, string? createdAgentId, string createdAgentEmail, string? description = null)
    {
        string details = $"PhotographyCompany {photographyCompanyId} created agent account with id {createdAgentId} and email {createdAgentEmail}";
        Log.ForContext("LogType", "UserActivity")
            .ForContext("EventType", "PhotographyCompanyCreateAgentAccount")
            .ForContext("PhotographyCompanyId", photographyCompanyId)
            .ForContext("CreatedAgentId", createdAgentId)
            .ForContext("CreatedAgentEmail", createdAgentEmail)
            .Information(description ?? details);
    }
}
