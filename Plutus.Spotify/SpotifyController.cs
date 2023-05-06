using Extensions;
using LiveLines.Api.Spotify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LiveLines.Spotify;

[Authorize]
[ApiController, Route("api")]
public class SpotifyController : ControllerBase
{
    private readonly ISpotifyService _spotifyService;
    private readonly string _spotifyClientId;
    private readonly string _spotifyCallbackUrl;
    private readonly string _profileCallbackUrl;

    public SpotifyController(IConfiguration configuration, ISpotifyService spotifyService)
    {
        _spotifyClientId = configuration.GetValue<string>("SPOTIFY_CLIENT_ID");
        _spotifyService = spotifyService;
        
        var hostName = configuration.GetValue<string>("HOST_NAME");
        _spotifyCallbackUrl = hostName + "/api/spotify/callback";
        _profileCallbackUrl = hostName + "/profile";
    }

    [Route("spotify/login")]
    public IActionResult Login()
    {
        const string scope = "user-read-private user-read-email";

        var query = new QueryBuilder
        {
            {"response_type", "code"},
            {"client_id", _spotifyClientId},
            {"scope", scope},
            {"redirect_uri", _spotifyCallbackUrl},
        };

        return Redirect("https://accounts.spotify.com/authorize" + query);
    }
    
    [Route("spotify/callback")]
    public async Task<IActionResult> Callback()
    {
        if (!Request.Query.TryGetValue("code", out var code))
        {
            throw new ArgumentNullException($"Spotify login callback is missing the code.");
        }

        await _spotifyService.UpsertSpotifyCredentials(User.GetLoggedInUser(), code.First(), _spotifyCallbackUrl);
        
        return Redirect(_profileCallbackUrl);
    }

}