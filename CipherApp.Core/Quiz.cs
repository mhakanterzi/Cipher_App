using System;
using System.Collections.Generic;
using System.Linq;

namespace CipherApp.Core
{
    /// <summary>
    /// Represents a generated quiz prompt with all data required to display and grade the challenge.
    /// </summary>
    public record CipherQuestion(
        string CipherName,
        string Plaintext,
        string KeyDescription,
        string Ciphertext,
        string Explanation,
        string KeyValue
    );

    /// <summary>
    /// Produces randomized cipher questions by sampling plaintexts, keys, and algorithm descriptions.
    /// </summary>
    public class QuizGenerator
    {
        private readonly Random _rng = new Random();
        private readonly ICipher[] _ciphers;

        private static readonly string[] Words = new[]
        {
            "HELLO","CIPHER","CRYPTO","ALGORITHM","SECURITY","KEY","MESSAGE","EXAMPLE","KNOWLEDGE","PUZZLE","ENCODE","DECODE"
        };

        public QuizGenerator()
        {
            // Catalogue of cipher implementations that the quiz can randomly choose from.
            _ciphers = new ICipher[]
            {
                new CaesarCipher(),
                new MonoalphabeticCipher(),
                new PlayfairCipher(),
                new HillCipher(),
                new VigenereCipher(),
                new TranspositionCipher(),
                new XorCipher(),
                new Base64Cipher()
            };
        }

        public CipherQuestion Next()
        {
            var cipher = _ciphers[_rng.Next(_ciphers.Length)];
            var pt = GeneratePlaintext();
            (object key, string keyDesc) = GenerateKey(cipher);
            var ct = cipher.Encrypt(pt, key);
            var exp = cipher.Explain(key);
            var keyVal = key?.ToString() ?? string.Empty;
            return new CipherQuestion(cipher.Name, pt, keyDesc, ct, exp, keyVal);
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
                CaesarCipher => (_rng.Next(1, 26), "Shift value (0-25)"),
                MonoalphabeticCipher => (RandomPermutation(), "26-letter permutation"),
                PlayfairCipher => (RandomWord(6), "Keyword"),
                HillCipher => (RandomInvertible2x2(), "2x2 matrix (mod 26)"),
                VigenereCipher => (RandomWord(5), "Keyword"),
                TranspositionCipher => (RandomWord(6), "Column keyword"),
                XorCipher => (RandomWord(6), "XOR key text"),
                Base64Cipher => (string.Empty, "No key required"), // Encoding needs no key
                _ => (1, string.Empty)
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
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
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
