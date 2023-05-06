using System.Threading.Tasks;
using Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Plutus.Authentication;

[ApiController, Route("api")]
public class AuthenticationController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet, Route("login")]
    public IActionResult Login(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl });
    }
    
    [HttpGet, Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Redirect(Url.Content("~/"));
    }

    public record AuthenticatedResponse(string Username, bool Authenticated);
    
    [AllowAnonymous]
    [HttpGet, Route("authenticated")]
    public ActionResult<AuthenticatedResponse> Authenticated()
    {
        var authenticated = User.Identity?.IsAuthenticated is true;
        var username = authenticated ? User.GetLoggedInUser().Username : string.Empty;
        return Ok(new AuthenticatedResponse(username, authenticated));
    }
}