using System;
using System.IO;
using System.Security.Cryptography;

public class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 20;
    private const int Iterations = 10000;

    public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
    {
        if (string.IsNullOrEmpty(passwordText))
            throw new ArgumentException("Password text cannot be null or empty.", nameof(passwordText));

        if (salt == null || salt.Length != SaltSize)
            throw new ArgumentException($"Salt must be {SaltSize} bytes in length.", nameof(salt));

        using (var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, Iterations))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);

            var hashBytes = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }
    }

    public static byte[] GenerateRandomSalt()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            return salt;
        }
    }

    public static bool VerifyPassword(string enteredPassword, string storedPasswordHash, byte[] storedSalt)
    {
        string enteredPasswordHash = GeneratePasswordHashUsingSalt(enteredPassword, storedSalt);
        return SlowEquals(Convert.FromBase64String(storedPasswordHash), Convert.FromBase64String(enteredPasswordHash));
    }

    private static bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
        {
            diff |= (uint)(a[i] ^ b[i]);
        }
        return diff == 0;
    }
}