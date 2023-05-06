using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using LiveLines.Api.Spotify;
using LiveLines.Api.Users;

namespace LiveLines.Spotify;

public class SpotifyService : ISpotifyService
{
    private static readonly HttpClient HttpClient = new ();
    
    private readonly ISpotifyCredentialsStore _spotifyCredentialsStore;
    private readonly string _spotifyClientId;
    private readonly string _spotifyClientSecret;
    
    public SpotifyService(IConfiguration configuration, ISpotifyCredentialsStore spotifyCredentialsStore)
    {
        _spotifyClientId = configuration.GetValue<string>("SPOTIFY_CLIENT_ID");
        _spotifyClientSecret = configuration.GetValue<string>("SPOTIFY_CLIENT_SECRET");
        _spotifyCredentialsStore = spotifyCredentialsStore;
    }

    public async Task<SpotifyCredentials?> GetSpotifyCredentials(LoggedInUser user)
    {
        var spotifyCredentials = await _spotifyCredentialsStore.GetCredentialsForUser(user);

        if (spotifyCredentials == null)
        {
            return null;
        }
        
        if (spotifyCredentials.ExpiresAt < DateTime.UtcNow)
        {
            var refreshedSpotifyCredentials = await RefreshAccessToken(spotifyCredentials.RefreshToken);
            await _spotifyCredentialsStore.UpsertCredentialsForUser(user, refreshedSpotifyCredentials);
            
            return refreshedSpotifyCredentials;
        }

        return spotifyCredentials;
    }

    private async Task<SpotifyCredentials> RefreshAccessToken(string refreshTokenRequest)
    {
        var data = new Dictionary<string, string>
        {
            {"refresh_token", refreshTokenRequest},
            {"grant_type", "refresh_token"},
        };

        var credentialsResponse = await GetSpotifyCredentialsFromOptions(data);

        return GetCredentialsFromResponse(credentialsResponse);
    }

    public async Task UpsertSpotifyCredentials(LoggedInUser user, string code, string redirectUrl)
    {
        var data = new Dictionary<string, string>
        {
            {"code", code},
            {"grant_type", "authorization_code"},
            {"redirect_uri", redirectUrl},
        };

        var credentialsResponse = await GetSpotifyCredentialsFromOptions(data);
        var spotifyCredentials = GetCredentialsFromResponse(credentialsResponse);

        await _spotifyCredentialsStore.UpsertCredentialsForUser(user, spotifyCredentials);
    }
    
    private SpotifyCredentials GetCredentialsFromResponse(SpotifyCredentialsResponse credentialsResponse)
    {
        var (accessToken, tokenType, scope, expiresIn, refreshToken) = credentialsResponse;
        
        // give 5 minutes of leeway
        var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 60 * 5);
        return new SpotifyCredentials(accessToken, tokenType, scope, expiresAt, refreshToken);
    }

    private async Task<SpotifyCredentialsResponse> GetSpotifyCredentialsFromOptions(Dictionary<string, string> data)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        request.Content = new FormUrlEncodedContent(data);

        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_spotifyClientId}:{_spotifyClientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        var response = await HttpClient.SendAsync(request);
        var spotifyTokenInfo = await response.Content.ReadFromJsonAsync<SpotifyCredentialsResponse>();

        if (spotifyTokenInfo == null)
        {
            throw new ArgumentNullException($"Spotify token info not deserialised correctly from Spotify response");
        }

        return spotifyTokenInfo;
    }

    // From the spotify docs https://developer.spotify.com/documentation/general/guides/authorization/code-flow/
    private record SpotifyCredentialsResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("token_type")] string TokenType,
        [property: JsonPropertyName("scope")] string Scope,
        [property: JsonPropertyName("expires_in")] int ExpiresIn,
        [property: JsonPropertyName("refresh_token")] string RefreshToken);
}