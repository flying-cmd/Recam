using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Remp.DataAccess.Collections;

public class UserActivity : BaseEvent
{
    [BsonIgnoreIfNull]
    public string? Email { get; set; }

    [BsonIgnoreIfNull]
    public string? CreatedAgentId { get; set; }

    [BsonIgnoreIfNull]
    public string? CreatedAgentEmail { get; set; }

    public UserActivity() { }

    public UserActivity(
        string eventType,
        string message,
        string actionName,
        string? userId = null,
        string? email = null,
        string? createdAgentId = null,
        string? createdAgentEmail = null,
        string? error = null)
        : base(eventType, message, actionName, userId, error)
    {
        if (email != null) Email = email;
        if (createdAgentId != null) CreatedAgentId = createdAgentId;
        if (createdAgentEmail != null) CreatedAgentEmail = createdAgentEmail;
    }
}
