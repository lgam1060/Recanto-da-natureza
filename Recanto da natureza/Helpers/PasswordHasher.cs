using System;
using System.Security.Cryptography;

namespace Recanto_da_natureza.Helpers;

public static class PasswordHasher
{
    // PBKDF2 simple implementation
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        var result = new byte[1 + salt.Length + hash.Length];
        result[0] = 0; // version
        Buffer.BlockCopy(salt, 0, result, 1, salt.Length);
        Buffer.BlockCopy(hash, 0, result, 1 + salt.Length, hash.Length);
        return Convert.ToBase64String(result);
    }

    public static bool Verify(string password, string storedHash)
    {
        try
        {
            var bytes = Convert.FromBase64String(storedHash);
            if (bytes.Length < 1 + 16 + 32) return false;
            var salt = new byte[16];
            Buffer.BlockCopy(bytes, 1, salt, 0, salt.Length);
            var hash = new byte[32];
            Buffer.BlockCopy(bytes, 1 + salt.Length, hash, 0, hash.Length);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
        catch
        {
            return false;
        }
    }
}
