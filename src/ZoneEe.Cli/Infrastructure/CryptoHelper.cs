using System.Security.Cryptography;
using System.Text;

namespace ZoneEe.Cli.Infrastructure;

internal static class CryptoHelper
{
    private const int SaltSize = 16;
    private const int IvSize = 16;
    private const int KeySize = 32; // AES-256
    private const int Iterations = 100_000;

    /// <summary>
    /// Encrypts plaintext using AES-256-CBC with a key derived from PIN via PBKDF2.
    /// Output format: [salt:16][iv:16][ciphertext]
    /// </summary>
    public static byte[] Encrypt(string plaintext, string pin)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = DeriveKey(pin, salt);
        var iv = RandomNumberGenerator.GetBytes(IvSize);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        var result = new byte[SaltSize + IvSize + ciphertext.Length];
        salt.CopyTo(result, 0);
        iv.CopyTo(result, SaltSize);
        ciphertext.CopyTo(result, SaltSize + IvSize);
        return result;
    }

    /// <summary>
    /// Decrypts data previously encrypted with <see cref="Encrypt"/>.
    /// Returns null if the PIN is wrong.
    /// </summary>
    public static string? Decrypt(byte[] data, string pin)
    {
        if (data.Length < SaltSize + IvSize + 1)
            return null;

        try
        {
            var salt = data[..SaltSize];
            var iv = data[SaltSize..(SaltSize + IvSize)];
            var ciphertext = data[(SaltSize + IvSize)..];

            var key = DeriveKey(pin, salt);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var plaintext = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
            return Encoding.UTF8.GetString(plaintext);
        }
        catch (CryptographicException)
        {
            return null;
        }
    }

    private static byte[] DeriveKey(string pin, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(pin),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);
    }
}
