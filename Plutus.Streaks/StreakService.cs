using LiveLines.Api.Lines;
using LiveLines.Api.Streaks;
using LiveLines.Api.Users;
using Microsoft.Extensions.Caching.Memory;

namespace LiveLines.Streaks;

public class StreakService : IStreakService
{
    private readonly ILinesService _linesService;
    private readonly IMemoryCache _expiringStreakCache;

    private const string StreakCachePrefix = "Streak_";

    public StreakService(ILinesService linesService, IMemoryCache expiringStreakCache)
    {
        _linesService = linesService;
        _expiringStreakCache = expiringStreakCache;
    }

    private string GetStreakCacheKey(LoggedInUser user) => StreakCachePrefix + user.InternalId;

    // We want it to expire at midnight the day after tomorrow, midnight makes this confusing.
    private DateTimeOffset GenerateStreakExpiry() => DateTime.UtcNow.AddDays(2).Date;
    
    public async Task<int> UpdateStreakForNewLine(LoggedInUser user, bool forYesterday)
    {
        var streakCacheKey = GetStreakCacheKey(user);
        if (!forYesterday && _expiringStreakCache.TryGetValue(streakCacheKey, out int currentStreak))
        {
            return _expiringStreakCache.Set(streakCacheKey, currentStreak + 1, GenerateStreakExpiry());
        }

        var streak = await GetStreakCount(user);
        return _expiringStreakCache.Set(streakCacheKey, streak, GenerateStreakExpiry());
    }

    public async Task<int> GetOrCreateStreak(LoggedInUser user)
    {
        return await _expiringStreakCache.GetOrCreateAsync(
            GetStreakCacheKey(user),
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpiration = GenerateStreakExpiry();
                return await GetStreakCount(user);
            });
    }

    private async Task<int> GetStreakCount(LoggedInUser loggedInUser)
    {
        // could do better than this, perhaps there's a fancy query we can do
        // that counts db-side consecutive line dates and short circuits. This will do for now.
        var orderedLines = (await _linesService.GetLines(loggedInUser))
            .OrderByDescending(x => x.DateFor);

        var previous = DateTime.Now;
        var count = 0;

        foreach (var line in orderedLines)
        {
            if ((previous.Date - line.DateFor.Date).TotalDays > 1)
            {
                break;
            }

            previous = line.DateFor;
            count++;
        }

        return count;
    }
}
