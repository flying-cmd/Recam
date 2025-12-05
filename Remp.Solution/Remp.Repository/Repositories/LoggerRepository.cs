using Remp.DataAccess.Collections;
using Remp.DataAccess.Data;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class LoggerRepository : ILoggerRepository
{
    private readonly MongoDbContext _dbContext;

    public LoggerRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddLogCaseHistoryAsync(CaseHistory caseHistory)
    {
        await _dbContext.CaseHistory.InsertOneAsync(caseHistory);
    }

    public async Task AddLogUserActivityAsync(UserActivity userActivity)
    {
        await _dbContext.UserActivity.InsertOneAsync(userActivity);
    }
}
