using System;
using System.Text;

namespace CipherApp.Core
{
    /// <summary>
    /// Provides a 2x2 Hill cipher implementation that leverages matrix algebra over mod 26 arithmetic.
    /// </summary>
    public class HillCipher : ICipher
    {
        public string Name => "Hill Cipher (2x2)";
        public string Description => "Encrypts two-letter blocks with a 2x2 matrix over mod 26. The key must be invertible modulo 26.";

        public string Encrypt(string plaintext, object key)
        {
            var K = ParseKey(key);
            var input = TextUtil.OnlyLetters(plaintext);
            if (input.Length % 2 == 1) input += "X";
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i += 2)
            {
                int a = TextUtil.CharToIndex(input[i]);
                int b = TextUtil.CharToIndex(input[i + 1]);
                int c0 = TextUtil.Mod(K[0,0] * a + K[0,1] * b, 26);
                int c1 = TextUtil.Mod(K[1,0] * a + K[1,1] * b, 26);
                sb.Append(TextUtil.IndexToChar(c0));
                sb.Append(TextUtil.IndexToChar(c1));
            }
            return sb.ToString();
        }

        public string Decrypt(string ciphertext, object key)
        {
            var K = ParseKey(key);
            var det = TextUtil.Mod(K[0,0] * K[1,1] - K[0,1] * K[1,0], 26);
            int invDet = TextUtil.ModInverse(det, 26);
            int[,] inv = new int[2,2];
            inv[0,0] = TextUtil.Mod(invDet * K[1,1], 26);
            inv[0,1] = TextUtil.Mod(invDet * -K[0,1], 26);
            inv[1,0] = TextUtil.Mod(invDet * -K[1,0], 26);
            inv[1,1] = TextUtil.Mod(invDet * K[0,0], 26);

            var input = TextUtil.OnlyLetters(ciphertext);
            if (input.Length % 2 == 1) input += "X";
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i += 2)
            {
                int a = TextUtil.CharToIndex(input[i]);
                int b = TextUtil.CharToIndex(input[i + 1]);
                int p0 = TextUtil.Mod(inv[0,0] * a + inv[0,1] * b, 26);
                int p1 = TextUtil.Mod(inv[1,0] * a + inv[1,1] * b, 26);
                sb.Append(TextUtil.IndexToChar(p0));
                sb.Append(TextUtil.IndexToChar(p1));
            }
            return sb.ToString();
        }

        public string Explain(object key)
        {
            return "The Hill cipher groups text into digraphs and multiplies each vector by the key matrix modulo 26. Decryption requires the modular inverse of the matrix.";
        }

        private int[,] ParseKey(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            // Expected formats include "a b c d" or "a,b;c,d".
            var s = key.ToString()!.Replace(",", " ").Replace(";", " ");
            var parts = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4) throw new ArgumentException("Key must contain four integers (2x2 matrix). Example: '3 3 2 5'.");
            var K = new int[2, 2];
            for (int i = 0; i < 4; i++)
            {
                if (!int.TryParse(parts[i], out int v)) throw new ArgumentException("Key must contain only integers.");
                K[i / 2, i % 2] = TextUtil.Mod(v, 26);
            }
            int det = TextUtil.Mod(K[0,0] * K[1,1] - K[0,1] * K[1,0], 26);
            if (TextUtil.Gcd(det, 26) != 1) throw new ArgumentException("The matrix must be invertible mod 26 (determinant coprime to 26).");
            return K;
        }
    }
}
