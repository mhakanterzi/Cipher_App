using System;
using System.Linq;
using System.Text;

namespace CipherApp.Core
{
    /// <summary>
    /// Implements a classic XOR stream cipher by combining UTF-8 bytes with a repeating key and returning the result as an uppercase hex string.
    /// </summary>
    public class XorCipher : ICipher
    {
        /// <inheritdoc />
        public string Name => "XOR Cipher";

        /// <inheritdoc />
        public string Description => "Plaintext bytes are XORed with repeating key bytes and emitted as hex. XOR is symmetric, so the same key decrypts the message.";

        /// <inheritdoc />
        public string Encrypt(string plaintext, object key)
        {
            // Convert inputs to byte arrays so we can apply XOR at the byte level.
            byte[] keyBytes = NormalizeKey(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(plaintext ?? string.Empty);

            // XOR the bytes with a repeating key sequence.
            byte[] cipherBytes = new byte[inputBytes.Length];
            for (int i = 0; i < inputBytes.Length; i++)
            {
                cipherBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            // Return the ciphertext as an uppercase hexadecimal string for readability.
            return Convert.ToHexString(cipherBytes);
        }

        /// <inheritdoc />
        public string Decrypt(string ciphertext, object key)
        {
            // Parse the hex representation back into raw bytes before applying XOR.
            byte[] keyBytes = NormalizeKey(key);
            byte[] cipherBytes = ParseHex(ciphertext);

            // XORing twice with the same key recovers the original plaintext bytes.
            byte[] plainBytes = new byte[cipherBytes.Length];
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                plainBytes[i] = (byte)(cipherBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }

        /// <inheritdoc />
        public string Explain(object key)
        {
            string keyPreview = key?.ToString() ?? string.Empty;
            if (keyPreview.Length > 8)
            {
                keyPreview = keyPreview[..8] + "...";
            }

            return $"The XOR cipher combines each byte with the repeating key using XOR. Key preview: '{keyPreview}'.";
        }

        /// <summary>
        /// Ensures the provided key is a non-empty UTF-8 string and returns its bytes for XOR operations.
        /// </summary>
        private static byte[] NormalizeKey(object key)
        {
            string keyText = key?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(keyText))
            {
                throw new ArgumentException("XOR encryption requires a non-empty key string.", nameof(key));
            }

            return Encoding.UTF8.GetBytes(keyText);
        }

        /// <summary>
        /// Converts an incoming hex string into the corresponding byte array, ignoring whitespace between pairs.
        /// </summary>
        private static byte[] ParseHex(string? ciphertext)
        {
            string cleaned = new string((ciphertext ?? string.Empty).Where(c => !char.IsWhiteSpace(c)).ToArray());
            if (cleaned.Length % 2 != 0)
            {
                throw new ArgumentException("Ciphertext must contain an even number of hex digits.", nameof(ciphertext));
            }

            try
            {
                return Convert.FromHexString(cleaned);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Ciphertext may only contain hexadecimal characters 0-9 and A-F.", nameof(ciphertext), ex);
            }
        }
    }
}
