using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherApp.Core
{
    // Kolonlu yer değiştirme (Columnar Transposition)
    public class TranspositionCipher : ICipher
    {
        public string Name => "Transpozisyon (Kolonlu)";
        public string Description => "Metin, anahtar kelime uzunluğunda sütunlara yazılır; sütunlar anahtarın sırasına göre okunur.";

        public string Encrypt(string plaintext, object key)
        {
            var k = PrepareKeyOrder(key, out var order);
            var input = TextUtil.OnlyLetters(plaintext);
            int cols = k.Length;
            int rows = (int)Math.Ceiling(input.Length / (double)cols);
            char[,] grid = new char[rows, cols];
            int idx = 0;
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    grid[r, c] = idx < input.Length ? input[idx++] : 'X';

            var sb = new StringBuilder();
            for (int rank = 0; rank < cols; rank++)
            {
                int col = order[rank];
                for (int r = 0; r < rows; r++)
                    sb.Append(grid[r, col]);
            }
            return sb.ToString();
        }

        public string Decrypt(string ciphertext, object key)
        {
            var k = PrepareKeyOrder(key, out var order);
            int cols = k.Length;
            var input = TextUtil.OnlyLetters(ciphertext);
            int rows = (int)Math.Ceiling(input.Length / (double)cols);
            char[,] grid = new char[rows, cols];

            int idx = 0;
            for (int rank = 0; rank < cols; rank++)
            {
                int col = order[rank];
                for (int r = 0; r < rows && idx < input.Length; r++)
                    grid[r, col] = input[idx++];
            }

            var sb = new StringBuilder();
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    sb.Append(grid[r, c]);
            return sb.ToString().TrimEnd('X');
        }

        public string Explain(object key)
        {
            var k = key?.ToString()?.ToUpperInvariant() ?? "";
            return $"Anahtar '{k}' harflerinin alfabetik sırasına göre sütunlar yeniden düzenlenir ve okunur.";
        }

        private string PrepareKeyOrder(object key, out int[] order)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var k = new string(key.ToString()!.ToUpperInvariant().Where(ch => ch >= 'A' && ch <= 'Z').ToArray());
            if (k.Length == 0) throw new ArgumentException("Anahtar yalnızca harflerden oluşmalıdır.");

            var indexed = k.Select((ch, i) => (ch, i)).ToList();
            var sorted = indexed.OrderBy(t => t.ch).ThenBy(t => t.i).ToList();
            order = new int[k.Length];
            for (int rank = 0; rank < sorted.Count; rank++)
                order[rank] = sorted[rank].i;
            return k;
        }
    }
}

