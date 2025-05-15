using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

public class CustomPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly string _secretKey;

    public CustomPasswordHasher(string secretKey)
    {
        _secretKey = secretKey;
    }

    public string HashPassword(TUser user, string password)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_secretKey);
        using var hmac = new HMACSHA256(keyBytes);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = hmac.ComputeHash(passwordBytes);
        return Convert.ToBase64String(hashBytes);
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        var providedHashed = HashPassword(user, providedPassword);

        return hashedPassword == providedHashed
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }
}
