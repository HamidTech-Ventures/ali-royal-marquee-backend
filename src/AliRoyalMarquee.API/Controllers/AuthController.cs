using AliRoyalMarquee.Application.Auth.Commands.ChangePassword;
using AliRoyalMarquee.Application.Auth.Commands.Login;
using AliRoyalMarquee.Application.Auth.Commands.Logout;
using AliRoyalMarquee.Application.Auth.Commands.Refresh;
using AliRoyalMarquee.Application.Auth.Queries.Me;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _mediator.Send(new LoginCommand(request.Email, request.Password, userAgent, ipAddress));
            
            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt);

            return Ok(new
            {
                accessToken = result.AccessToken,
                expiresAt = result.ExpiresAt,
                user = result.User
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "No refresh token provided." });
            }

            var userAgent = Request.Headers["User-Agent"].ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _mediator.Send(new RefreshCommand(refreshToken, userAgent, ipAddress));
            
            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt);

            return Ok(new
            {
                accessToken = result.AccessToken,
                expiresAt = result.ExpiresAt,
                user = result.User
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _mediator.Send(new LogoutCommand(refreshToken));
        }

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true // True for production proxy
        });

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized(new { message = "Invalid user id." });
        }

        var result = await _mediator.Send(new MeQuery(userId));
        return Ok(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            await _mediator.Send(new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword));
            
            // Log out user on change password
            Response.Cookies.Delete("refreshToken");
            
            return Ok(new { message = "Password changed successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    private void SetRefreshTokenCookie(string token, DateTimeOffset expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expiresAt,
            SameSite = SameSiteMode.None,
            Secure = true // We require HTTPS/Proxy for this
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}

public record LoginRequest(string Email, string Password);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
