using System.Security.Cryptography;

namespace HotelApp.Security
{
    internal static class PasswordHasher
    {
        private const string Scheme = "pbkdf2-sha256";
        private const int Iterations = 100000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public static string Hash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

            return $"{Scheme}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            if (password == null || string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            if (!IsHashed(storedHash))
            {
                return password == storedHash;
            }

            string[] parts = storedHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
            {
                return false;
            }

            if (!int.TryParse(parts[1], out int iterations) || iterations <= 0)
            {
                return false;
            }

            byte[] salt;
            byte[] expectedHash;

            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedHash = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        public static bool IsHashed(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && value.StartsWith($"{Scheme}$", StringComparison.Ordinal);
        }
    }
}