using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SGS.TaskTracker.Interfaces;

namespace SSGTaskTracker.Application.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var hashed = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hashed)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = Convert.FromBase64String(parts[1]);

        var computedHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32);

        return computedHash.SequenceEqual(storedHash);
    }
}