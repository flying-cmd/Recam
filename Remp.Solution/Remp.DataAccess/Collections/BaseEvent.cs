using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Remp.DataAccess.Collections;

public class BaseEvent
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonIgnoreIfNull]
    public string? UserId { get; set; }

    public string EventType { get; set; } = null!;

    public string Message { get; set; } = null!;

    [BsonIgnoreIfNull]
    public string? Error { get; set; }

    public string ActionName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public BaseEvent() { }

    public BaseEvent(
        string eventType,
        string message,
        string actionName,
        string? userId = null,
        string? error = null)
    {
        if (userId != null) UserId = userId;
        EventType = eventType;
        Message = message;
        if (error != null) Error = error;
        ActionName = actionName;
        CreatedAt = DateTime.UtcNow;
    }
}
