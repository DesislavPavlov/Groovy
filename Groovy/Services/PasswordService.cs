using System.Security.Cryptography;
using System.Text;

namespace Groovy.Services
{
    public class PasswordService
    {
        public string HashPassword(string password)
        {
            // Generate salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Combine salt & password
            byte[] saltedPassword = CombinePasswordSalt(password, salt);

            // Hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);

                byte[] hashWithSalt = new byte[salt.Length + hashBytes.Length];
                Array.Copy(salt, 0, hashWithSalt, 0, salt.Length);
                Array.Copy(hashBytes, 0, hashWithSalt, salt.Length, hashBytes.Length);

                return Convert.ToBase64String(hashWithSalt);
            }
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashWithSalt = Convert.FromBase64String(storedHash);

            // Extract salt
            byte[] salt = new byte[16];
            Array.Copy(hashWithSalt, 0, salt, 0, salt.Length);

            // Combine password & salt
            byte[] saltedPassword = CombinePasswordSalt(enteredPassword, salt);

            // Hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);

                // Compare new and old hash
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    if (hashWithSalt[i + salt.Length] != hashBytes[i])
                        return false;
                }
            }

            return true;
        }

        private byte[] CombinePasswordSalt(string password, byte[] salt)
        {
            byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(password);

            byte[] saltedPassword = new byte[salt.Length + enteredPasswordBytes.Length];
            Array.Copy(salt, 0, saltedPassword, 0, salt.Length);
            Array.Copy(enteredPasswordBytes, 0, saltedPassword, salt.Length, enteredPasswordBytes.Length);

            return saltedPassword;
        }
    }
}
