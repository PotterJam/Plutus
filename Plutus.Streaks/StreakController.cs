using Extensions;
using LiveLines.Api.Streaks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveLines.Streaks;

[Authorize]
[ApiController, Route("api")]
public class StreakController : ControllerBase
{
    private readonly IStreakService _streakService;

    public StreakController(IStreakService streakService)
    {
        _streakService = streakService;
    }

    public record StreakResponse(int StreakCount);

    [HttpGet, Route("streak")]
    public async Task<StreakResponse> GetStreak()
    {
        var streak = await _streakService.GetOrCreateStreak(User.GetLoggedInUser());
        return new StreakResponse(streak);
    }
}