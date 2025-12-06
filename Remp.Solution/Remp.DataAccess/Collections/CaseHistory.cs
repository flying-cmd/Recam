using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Remp.Common.Helpers;

namespace Remp.DataAccess.Collections;

public class CaseHistory : BaseEvent
{
    [BsonIgnoreIfNull]
    public string? ListingCaseId { get; set; }

    [BsonIgnoreIfNull]
    public Dictionary<string, FieldChange>? UpdatedFields { get; set; }

    [BsonIgnoreIfNull]
    public string? OldStatus { get; set; }

    [BsonIgnoreIfNull]
    public string? NewStatus { get; set; }

    [BsonIgnoreIfNull]
    public string? MediaId { get; set; }

    [BsonIgnoreIfNull]
    public IEnumerable<int>? MediaIds { get; set; }

    [BsonIgnoreIfNull]
    public string? AgentId { get; set; }

    public CaseHistory() { }

    public CaseHistory( 
        string eventType, 
        string message,  
        string actionName,
        string? listingCaseId = null,
        string? userId = null,
        Dictionary<string, FieldChange>? updatedFields = null,
        string? oldStatus = null,
        string? newStatus = null,
        string? mediaId = null,
        IEnumerable<int>? mediaIds = null,
        string? agentId = null,
        string? error = null)
        : base(eventType, message, actionName, userId, error)
    {
        if (listingCaseId != null) ListingCaseId = listingCaseId;
        if (userId != null) UserId = userId;
        if (updatedFields != null) UpdatedFields = updatedFields;
        if (oldStatus != null) OldStatus = oldStatus;
        if (newStatus != null) NewStatus = newStatus;
        if (mediaId != null) MediaId = mediaId;
        if (mediaIds != null) MediaIds = mediaIds;
        if (agentId != null) AgentId = agentId;
        if (error != null) Error = error;
    }
}
