using System;
using System.Text;

namespace CipherApp.Core
{
    /// <summary>
    /// Provides Base64 encoding and decoding as a reversible cipher-like transformation without requiring a key.
    /// </summary>
    public class Base64Cipher : ICipher
    {
        /// <inheritdoc />
        public string Name => "Base64 Encoding";

        /// <inheritdoc />
        public string Description => "Plaintext is converted to UTF-8 bytes and encoded with Base64. Decoding reverses the process and requires no key.";

        /// <inheritdoc />
        public string Encrypt(string plaintext, object key)
        {
            // Base64 is an encoding operation; the key parameter is ignored.
            byte[] inputBytes = Encoding.UTF8.GetBytes(plaintext ?? string.Empty);
            return Convert.ToBase64String(inputBytes);
        }

        /// <inheritdoc />
        public string Decrypt(string ciphertext, object key)
        {
            // Convert the Base64 string back to bytes and decode as UTF-8 text.
            try
            {
                byte[] data = Convert.FromBase64String(ciphertext ?? string.Empty);
                return Encoding.UTF8.GetString(data);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Ciphertext must be a valid Base64 string.", nameof(ciphertext), ex);
            }
        }

        /// <inheritdoc />
        public string Explain(object key)
        {
            return "Base64 encoding maps every three bytes to four printable characters using the standard alphabet. No key is required.";
        }
    }
}
