using System;
using System.Text;

namespace CipherApp.Core
{
    /// <summary>
    /// Provides a Vigenere cipher implementation with key normalization and explanatory helpers.
    /// </summary>
    public class VigenereCipher : ICipher
    {
        public string Name => "Vigenere Cipher";
        public string Description => "A polyalphabetic shift where the keyword defines the shift for each letter. Classic attacks include Kasiski and Friedman tests.";

        public string Encrypt(string plaintext, object key)
        {
            var k = PrepareKey(key);
            var input = plaintext?.ToUpperInvariant() ?? string.Empty;
            var sb = new StringBuilder(input.Length);
            int ki = 0;
            foreach (var ch in input)
            {
                if (ch >= 'A' && ch <= 'Z')
                {
                    int shift = TextUtil.CharToIndex(k[ki % k.Length]);
                    sb.Append(TextUtil.IndexToChar(TextUtil.CharToIndex(ch) + shift));
                    ki++;
                }
                else sb.Append(ch);
            }
            return sb.ToString();
        }

        public string Decrypt(string ciphertext, object key)
        {
            var k = PrepareKey(key);
            var input = ciphertext?.ToUpperInvariant() ?? string.Empty;
            var sb = new StringBuilder(input.Length);
            int ki = 0;
            foreach (var ch in input)
            {
                if (ch >= 'A' && ch <= 'Z')
                {
                    int shift = TextUtil.CharToIndex(k[ki % k.Length]);
                    sb.Append(TextUtil.IndexToChar(TextUtil.CharToIndex(ch) - shift));
                    ki++;
                }
                else sb.Append(ch);
            }
            return sb.ToString();
        }

        public string Explain(object key)
        {
            var k = PrepareKey(key);
            int firstShift = TextUtil.CharToIndex(k[0]);
            return $"The Vigenere cipher shifts each letter by the matching keyword letter. Example: A + {k[0]} -> {TextUtil.IndexToChar(TextUtil.CharToIndex('A') + firstShift)}.";
        }

        private string PrepareKey(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var k = new StringBuilder();
            foreach (var c in key.ToString()!.ToUpperInvariant())
            {
                if (c >= 'A' && c <= 'Z') k.Append(c);
            }
            if (k.Length == 0) throw new ArgumentException("Key must contain letters only.");
            return k.ToString();
        }
    }
}
