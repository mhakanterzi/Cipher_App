using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherApp.Core
{
    /// <summary>
    /// Implements the classic columnar transposition cipher with helper routines for key ordering.
    /// </summary>
    public class TranspositionCipher : ICipher
    {
        public string Name => "Columnar Transposition";
        public string Description => "Writes text into columns whose width is the key length, then reads the columns in key order.";

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
            var k = key?.ToString()?.ToUpperInvariant() ?? string.Empty;
            return $"Columns are reordered alphabetically by the key '{k}' and read top to bottom.";
        }

        private string PrepareKeyOrder(object key, out int[] order)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var k = new string(key.ToString()!.ToUpperInvariant().Where(ch => ch >= 'A' && ch <= 'Z').ToArray());
            if (k.Length == 0) throw new ArgumentException("Key must contain letters only.");

            var indexed = k.Select((ch, i) => (ch, i)).ToList();
            var sorted = indexed.OrderBy(t => t.ch).ThenBy(t => t.i).ToList();
            order = new int[k.Length];
            for (int rank = 0; rank < sorted.Count; rank++)
                order[rank] = sorted[rank].i;
            return k;
        }
    }
}
