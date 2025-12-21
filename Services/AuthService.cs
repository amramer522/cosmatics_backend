using Cosmatics.DTOs;
using Cosmatics.Models;
using Cosmatics.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Cosmatics.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepo;
    private readonly IConfiguration _configuration;

    public AuthService(IRepository<User> userRepo, IConfiguration configuration)
    {
        _userRepo = userRepo;
        _configuration = configuration;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var existingUsers = await _userRepo.FindAsync(u => u.PhoneNumber == dto.PhoneNumber);
        if (existingUsers.Any())
            return false;

        CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email ?? "",
            CountryCode = dto.CountryCode,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = Convert.ToBase64String(passwordHash) + ":" + Convert.ToBase64String(passwordSalt),
            Role = "Customer", // Always default to Customer for public registration
            IsVerified = false // Always require verification
        };

        await _userRepo.AddAsync(user);
        return true;
    }

    public async Task<(string? Token, User? User, string? Error)> LoginAsync(LoginDto dto)
    {
        IEnumerable<User> users;
        
        if (!string.IsNullOrEmpty(dto.Username))
        {
             // Login by Username
             users = await _userRepo.FindAsync(u => u.Username == dto.Username);
        }
        else
        {
             // Login by Phone
             users = await _userRepo.FindAsync(u => u.PhoneNumber == dto.PhoneNumber);
        }

        var user = users.FirstOrDefault();

        if (user == null)
            return (null, null, "Invalid credentials.");

        // CHECK VERIFICATION
        if (!user.IsVerified)
            return (null, null, "Account not verified. Please verify your phone number first.");

        if (!VerifyPasswordHash(dto.Password, user.PasswordHash))
            return (null, null, "Invalid credentials.");

        var token = CreateToken(user);
        return (token, user, null);
    }

    public Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        // In a real app, generate OTP and send SMS here.
        // For now, we simulate success so user can use the OTP "1111"
        return Task.FromResult(true);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return false;

        var users = await _userRepo.FindAsync(u => u.PhoneNumber == dto.PhoneNumber);
        var user = users.FirstOrDefault();
        if (user == null) return false;

        CreatePasswordHash(dto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
        
        user.PasswordHash = Convert.ToBase64String(passwordHash) + ":" + Convert.ToBase64String(passwordSalt);
        
        await _userRepo.UpdateAsync(user);
        return true;
    }

    public async Task<bool> VerifyOtpAsync(VerifyOtpDto dto)
    {
        // Static check for now as requested
        if (dto.OtpCode == "1111")
        {
            var users = await _userRepo.FindAsync(u => u.PhoneNumber == dto.PhoneNumber);
            var user = users.FirstOrDefault();
            if (user != null)
            {
                user.IsVerified = true;
                await _userRepo.UpdateAsync(user);
                return true;
            }
        }

        return false;
    }

    public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return false;

        user.Username = dto.Username;
        user.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.ProfilePhotoUrl))
        {
            user.ProfilePhotoUrl = dto.ProfilePhotoUrl;
        }

        await _userRepo.UpdateAsync(user);
        return true;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepo.GetByIdAsync(id);
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 2) return false;

        var passwordHash = Convert.FromBase64String(parts[0]);
        var passwordSalt = Convert.FromBase64String(parts[1]);

        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
