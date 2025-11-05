using System;
using System.Linq;
using System.Text;

namespace CipherApp.Core
{
    /// <summary>
    /// Collects shared text utilities for normalization, modular arithmetic, and alphabet conversions.
    /// </summary>
    internal static class TextUtil
    {
        private static readonly char[] Alphabet = Enumerable.Range('A', 26).Select(i => (char)i).ToArray();

        public static string OnlyLetters(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var sb = new StringBuilder(input.Length);
            foreach (var ch in input.ToUpperInvariant())
            {
                if (ch >= 'A' && ch <= 'Z') sb.Append(ch);
            }
            return sb.ToString();
        }

        public static int Mod(int a, int m)
        {
            int r = a % m;
            return r < 0 ? r + m : r;
        }

        public static int CharToIndex(char c) => c - 'A';
        public static char IndexToChar(int i) => (char)('A' + Mod(i, 26));

        public static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                int t = b;
                b = a % b;
                a = t;
            }
            return Math.Abs(a);
        }

        public static int ModInverse(int a, int m)
        {
            int t = 0, newT = 1;
            int r = m, newR = Mod(a, m);
            while (newR != 0)
            {
                int q = r / newR;
                (t, newT) = (newT, t - q * newT);
                (r, newR) = (newR, r - q * newR);
            }
            if (r > 1) throw new InvalidOperationException("Ters mod bulunamad\u0131 (mod\u00FCler ters yok).");
            if (t < 0) t += m;
            return t;
        }
    }
}

