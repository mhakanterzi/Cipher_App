using System;
using System.Text;

namespace CipherApp.Core
{
    /// <summary>
    /// Implements the classic Caesar shift cipher with modular arithmetic and descriptive helpers.
    /// </summary>
    public class CaesarCipher : ICipher
    {
        public string Name => "Caesar Cipher";
        public string Description => "Each letter is shifted by a fixed key value. Typical attacks include frequency analysis and trying every key.";

        public string Encrypt(string plaintext, object key)
        {
            int k = NormalizeKey(key);
            var input = plaintext?.ToUpperInvariant() ?? string.Empty;
            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                if (ch >= 'A' && ch <= 'Z')
                {
                    sb.Append(TextUtil.IndexToChar(TextUtil.CharToIndex(ch) + k));
                }
                else sb.Append(ch);
            }
            return sb.ToString();
        }

        public string Decrypt(string ciphertext, object key)
        {
            int k = NormalizeKey(key);
            return Encrypt(ciphertext, -k);
        }

        public string Explain(object key)
        {
            int k = NormalizeKey(key);
            return $"The Caesar cipher shifts each letter by {k}. Example: A -> {TextUtil.IndexToChar(k)}.";
        }

        private int NormalizeKey(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (key is int i) return TextUtil.Mod(i, 26);
            if (int.TryParse(key.ToString(), out int v)) return TextUtil.Mod(v, 26);
            throw new ArgumentException("Key must be an integer between 0 and 25.", nameof(key));
        }
    }
}
