using System.ComponentModel.DataAnnotations;

namespace Cosmatics.DTOs;

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer"; // Default to Customer
}

public class LoginDto
{
    public string? Username { get; set; } // Added for Dashboard/Admin login
    public string? CountryCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
}

public class VerifyOtpDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}

public class ForgotPasswordDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
}

public class UpdateProfileDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
}
