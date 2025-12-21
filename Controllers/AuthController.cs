using Cosmatics.DTOs;
using Cosmatics.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cosmatics.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!await _authService.RegisterAsync(dto))
        {
            return BadRequest(new { message = "Username or Email already exists." });
        }
        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        _logger.LogInformation("Login attempt for User: {Username} / Phone: {Phone}", dto.Username, dto.PhoneNumber);
        
        var (token, user, error) = await _authService.LoginAsync(dto);
        
        if (error != null)
        {
            _logger.LogWarning("Login failed for {Username}/{Phone}: {Error}", dto.Username, dto.PhoneNumber, error);
            if (error.Contains("verified"))
                return StatusCode(403, new { message = error }); // Forbidden (Unverified)
            
            return Unauthorized(new { message = error }); // 401
        }

        if (token == null || user == null)
        {
             _logger.LogWarning("Login failed for {Username}/{Phone}: Invalid credentials (Null return)", dto.Username, dto.PhoneNumber);
             return Unauthorized(new { message = "Invalid credentials." });
        }
        
        _logger.LogInformation("Login successful for User: {Username}", user.Username);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CountryCode = user.CountryCode,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role
        };

        return Ok(new { Token = token, User = userDto });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
    {
        var isValid = await _authService.VerifyOtpAsync(dto);
        if (!isValid)
        {
            return BadRequest(new { message = "Invalid OTP code." });
        }
        return Ok(new { message = "OTP verified successfully." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        await _authService.ForgotPasswordAsync(dto);
        return Ok(new { message = "OTP sent successfully." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var success = await _authService.ResetPasswordAsync(dto);
        if (!success)
        {
            return BadRequest(new { message = "Failed to reset password. Check phone number or passwords match." });
        }
        return Ok(new { message = "Password reset successfully." });
    }

    [HttpPut("profile")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var success = await _authService.UpdateProfileAsync(int.Parse(userIdClaim.Value), dto);
        if(!success) return BadRequest(new { message = "Failed to update profile." });

        return Ok(new { message = "Profile updated successfully." });
    }

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult Logout()
    {
        // Stateless JWT logout is purely client-side (delete token).
        // Sending success message.
        return Ok(new { message = "Logged out successfully." });
    }

    [HttpGet("profile")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var user = await _authService.GetUserByIdAsync(int.Parse(userIdClaim.Value));
        if (user == null) return NotFound();

        return Ok(new
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber,
            CountryCode = user.CountryCode,
            ProfilePhotoUrl = user.ProfilePhotoUrl
        });
    }
}
