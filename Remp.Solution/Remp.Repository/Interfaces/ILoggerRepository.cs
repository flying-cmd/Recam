using Remp.DataAccess.Collections;

namespace Remp.Repository.Interfaces;

public interface ILoggerRepository
{
    Task AddLogCaseHistoryAsync(CaseHistory caseHistory);
    Task AddLogUserActivityAsync(UserActivity userActivity);
}
