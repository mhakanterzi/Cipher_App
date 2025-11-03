using System;
using System.Text;

namespace CipherApp.Core
{
    public class VigenereCipher : ICipher
    {
        public string Name => "Vigenère Şifresi";
        public string Description => "Çokalfabeli kaydırma: anahtar kelime harf harf kaydırma miktarlarını belirler. Çözüm için Kasiski ve Friedman testleri kullanılır.";

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
            return $"Vigenère, her harfi anahtarın ilgili harfi kadar kaydırır. Örn: A + {k[0]} → {TextUtil.IndexToChar(TextUtil.CharToIndex('A') + TextUtil.CharToIndex(k[0]))}.";
        }

        private string PrepareKey(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var k = new StringBuilder();
            foreach (var c in key.ToString()!.ToUpperInvariant())
            {
                if (c >= 'A' && c <= 'Z') k.Append(c);
            }
            if (k.Length == 0) throw new ArgumentException("Anahtar yalnızca harflerden oluşmalıdır.");
            return k.ToString();
        }
    }
}

