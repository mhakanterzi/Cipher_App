using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherApp.Core
{
    /// <summary>
    /// Implements a monoalphabetic substitution cipher driven by a 26-letter permutation key.
    /// </summary>
    public class MonoalphabeticCipher : ICipher
    {
        public string Name => "Monoalphabetic Substitution";
        public string Description => "Each alphabet letter maps to another letter according to a 26-character permutation. Frequency analysis and word patterns aid decryption.";

        public string Encrypt(string plaintext, object key)
        {
            var map = BuildMapFromKey(key);
            var input = plaintext?.ToUpperInvariant() ?? string.Empty;
            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                if (ch >= 'A' && ch <= 'Z')
                {
                    sb.Append(map[ch]);
                }
                else sb.Append(ch);
            }
            return sb.ToString();
        }

        public string Decrypt(string ciphertext, object key)
        {
            var map = BuildMapFromKey(key);
            var rev = map.ToDictionary(kv => kv.Value, kv => kv.Key);
            var input = ciphertext?.ToUpperInvariant() ?? string.Empty;
            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                if (ch >= 'A' && ch <= 'Z')
                {
                    sb.Append(rev[ch]);
                }
                else sb.Append(ch);
            }
            return sb.ToString();
        }

        public string Explain(object key)
        {
            return "A monoalphabetic cipher replaces each letter with a unique partner (A->?, B->?, ...). Attackers use frequency analysis and common word shapes to break it.";
        }

        private Dictionary<char, char> BuildMapFromKey(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var k = key.ToString()!.ToUpperInvariant();
            // Key must be a permutation of the alphabet (A-Z)
            if (k.Length != 26 || k.Any(c => c < 'A' || c > 'Z') || k.Distinct().Count() != 26)
                throw new ArgumentException("Key must be a 26-character permutation of A-Z.");
            var map = new Dictionary<char, char>(26);
            for (int i = 0; i < 26; i++)
            {
                char src = (char)('A' + i);
                map[src] = k[i];
            }
            return map;
        }
    }
}
