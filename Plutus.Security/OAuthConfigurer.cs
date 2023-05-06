
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Plutus.Api.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Plutus.Security;

public class OAuthConfigurer
{
    private readonly string _gitHubClientId;
    private readonly string _gitHubClientSecret;

    public OAuthConfigurer(IConfiguration configuration)
    {
        _gitHubClientId = configuration.GetValue<string?>("GITHUB_CLIENT_ID") ??
                          throw new InvalidOperationException("Could not get GITHUB_CLIENT_ID");
        _gitHubClientSecret = configuration.GetValue<string?>("GITHUB_CLIENT_SECRET") ??
                              throw new InvalidOperationException("Could not get GITHUB_CLIENT_SECRET");
    }

    public void GitHub(OAuthOptions options)
    {
        options.ClientId = _gitHubClientId;
        options.ClientSecret = _gitHubClientSecret;
        options.CallbackPath = new PathString("/api/signin-github");
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;

        options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        options.TokenEndpoint = "https://github.com/login/oauth/access_token";
        options.UserInformationEndpoint = "https://api.github.com/user";

        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey("urn:github:login", "login");
        options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
        options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var request =
                    new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", context.AccessToken);

                var response = await context.Backchannel.SendAsync(request,
                    HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var userJson = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                context.RunClaimActions(userJson.RootElement);

                // Dammit operator since we should definitely have the identity at this point
                var gitHubUsername = context.Identity!.Claims.Single(x => x.Type == "urn:github:login").Value;

                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                
                var user = await userService.GetUser("GitHub", gitHubUsername) 
                           ?? await userService.CreateUser("GitHub", gitHubUsername);
                
                context.Identity.AddClaim(new Claim(LoggedInClaims.InternalUserId, user.InternalId.ToString()));
                context.Identity.AddClaim(new Claim(LoggedInClaims.Username, user.Username));
                
                context.Properties.AllowRefresh = true;
                context.Properties.IsPersistent = true;
                context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);
            }
        };
    }
}