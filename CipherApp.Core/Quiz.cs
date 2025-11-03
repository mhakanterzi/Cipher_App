using System;
using System.Collections.Generic;
using System.Linq;

namespace CipherApp.Core
{
    public record CipherQuestion(
        string CipherName,
        string Plaintext,
        string KeyDescription,
        string Ciphertext,
        string Explanation
    );

    public class QuizGenerator
    {
        private readonly Random _rng = new Random();
        private readonly ICipher[] _ciphers;

        private static readonly string[] Words = new[]
        {
            "MERHABA","SIFRE","KRIPTO","ALGORITMA","GUVENLIK","ANAHTAR","MESAJ","COZUM","ORNEK","METIN","TURKIYE","BILGI"
        };

        public QuizGenerator()
        {
            _ciphers = new ICipher[]
            {
                new CaesarCipher(),
                new MonoalphabeticCipher(),
                new PlayfairCipher(),
                new HillCipher(),
                new VigenereCipher(),
                new TranspositionCipher()
            };
        }

        public CipherQuestion Next()
        {
            var cipher = _ciphers[_rng.Next(_ciphers.Length)];
            var pt = GeneratePlaintext();
            (object key, string keyDesc) = GenerateKey(cipher);
            var ct = cipher.Encrypt(pt, key);
            var exp = cipher.Explain(key);
            return new CipherQuestion(cipher.Name, pt, keyDesc, ct, exp);
        }

        private string GeneratePlaintext()
        {
            int count = _rng.Next(2, 4);
            return string.Join(" ", Enumerable.Range(0, count).Select(_ => Words[_rng.Next(Words.Length)]));
        }

        private (object key, string desc) GenerateKey(ICipher cipher)
        {
            return cipher switch
            {
                CaesarCipher => ( _rng.Next(1, 26), "Kaydırma (0-25)"),
                MonoalphabeticCipher => ( RandomPermutation(), "26 harflik permütasyon"),
                PlayfairCipher => ( RandomWord(6), "Anahtar kelime"),
                HillCipher => ( RandomInvertible2x2(), "2x2 matris (mod 26)"),
                VigenereCipher => ( RandomWord(5), "Anahtar kelime"),
                TranspositionCipher => ( RandomWord(6), "Sütun anahtar kelime"),
                _ => (1, "")
            };
        }

        private string RandomPermutation()
        {
            var arr = Enumerable.Range('A', 26).Select(i => (char)i).ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                int j = _rng.Next(i, arr.Length);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
            return new string(arr);
        }

        private string RandomWord(int len)
        {
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Range(0, len).Select(_ => alphabet[_rng.Next(alphabet.Length)]).ToArray());
        }

        private string RandomInvertible2x2()
        {
            while (true)
            {
                int a = _rng.Next(0, 26);
                int b = _rng.Next(0, 26);
                int c = _rng.Next(0, 26);
                int d = _rng.Next(0, 26);
                int det = ((a * d) - (b * c)) % 26;
                if (det < 0) det += 26;
                if (TextUtil.Gcd(det, 26) == 1)
                {
                    return $"{a} {b} {c} {d}";
                }
            }
        }
    }
}

