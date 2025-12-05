using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Remp.DataAccess.Collections;
using Remp.Models.Settings;

namespace Remp.DataAccess.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSetting> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<CaseHistory> CaseHistory => _database.GetCollection<CaseHistory>("CaseHistory");
    public IMongoCollection<UserActivity> UserActivity => _database.GetCollection<UserActivity>("UserActivity");
}
