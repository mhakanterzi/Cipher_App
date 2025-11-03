using System;
using System.Text;

namespace CipherApp.Core
{
    public class CaesarCipher : ICipher
    {
        public string Name => "Sezar (Caesar) Şifrelemesi";
        public string Description => "Her harf sabit bir anahtar kadar kaydırılır. Çözüm için frekans analizi ve tüm anahtarları deneme kullanılabilir.";

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
            return $"Sezar şifresi, her harfi alfabede {k} kadar kaydırır. Örn: A→{TextUtil.IndexToChar(k)}.";
        }

        private int NormalizeKey(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (key is int i) return TextUtil.Mod(i, 26);
            if (int.TryParse(key.ToString(), out int v)) return TextUtil.Mod(v, 26);
            throw new ArgumentException("Anahtar bir tamsayı olmalıdır (0-25).", nameof(key));
        }
    }
}

