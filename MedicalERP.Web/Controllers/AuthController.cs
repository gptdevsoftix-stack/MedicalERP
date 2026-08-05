using MedicalERP.Application.Abstractions.Security;
using MedicalERP.Application.Interfaces;
using MedicalERP.Application.Common;
using MedicalERP.Application.Identity.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalERP.Web.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IIdentityService identity, IRefreshTokenService refreshTokens) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> Login(LoginRequest request, CancellationToken ct) => Ok(ApiResponse<TokenResponse>.Ok(await identity.LoginAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString(), ct)));
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> Refresh(RefreshTokenRequest request, CancellationToken ct) => Ok(ApiResponse<TokenResponse>.Ok(await refreshTokens.RefreshAsync(request.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString(), ct)));
    [Authorize]
    [HttpPost("revoke")]
    public async Task<ActionResult<ApiResponse<object>>> Revoke(RevokeRefreshTokenRequest request, CancellationToken ct) { await refreshTokens.RevokeAsync(request.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString(), request.Reason, ct); return Ok(ApiResponse<object>.Ok(new { })); }
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<string>>> ForgotPassword(ForgotPasswordRequest request, CancellationToken ct) => Ok(ApiResponse<string>.Ok(await identity.GeneratePasswordResetTokenAsync(request, ct), "Use email delivery in production."));
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword(ResetPasswordRequest request, CancellationToken ct) { await identity.ResetPasswordAsync(request, ct); return Ok(ApiResponse<object>.Ok(new { })); }
}


