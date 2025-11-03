using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherApp.Core
{
    public class MonoalphabeticCipher : ICipher
    {
        public string Name => "Monoalfabetik (Basit Yerine Koyma)";
        public string Description => "Alfabedeki her harf, bir permütasyonla başka bir harfe eşlenir. Çözüm için frekans analizi ve kelime kalıpları kullanılır.";

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
            return "Monoalfabetik şifre, A→?, B→?, ... Z→? şeklinde tekil bir eşleme kullanır. Çözümde frekans analizi (E, A, R, L gibi) ve kelime kalıpları (TEK, VE) önemlidir.";
        }

        private Dictionary<char, char> BuildMapFromKey(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var k = key.ToString()!.ToUpperInvariant();
            // Anahtar 26 harflik bir permütasyon olmalı (A-Z)
            if (k.Length != 26 || k.Any(c => c < 'A' || c > 'Z') || k.Distinct().Count() != 26)
                throw new ArgumentException("Anahtar 26 harften oluşan bir permütasyon olmalıdır.");
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
