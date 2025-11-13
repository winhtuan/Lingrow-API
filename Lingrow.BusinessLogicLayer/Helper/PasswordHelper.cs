using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Plantpedia.BusinessLogicLayer.Helper;

public static class PasswordHelper
{
    public static (byte[] Hash, byte[] Salt) GeneratePasswordHash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        // Tạo salt ngẫu nhiên (128-bit = 16 bytes)
        byte[] saltBytes = new byte[16];
        RandomNumberGenerator.Fill(saltBytes);

        // Hash password với salt
        byte[] hashBytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32
        );

        return (hashBytes, saltBytes);
    }

    // Kiểm tra password (so sánh byte[])
    public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));
        if (storedHash == null || storedSalt == null)
            throw new ArgumentNullException("Stored hash or salt cannot be null.");

        // Hash lại password người dùng nhập vào
        byte[] computedHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: storedSalt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32
        );

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}
