using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherApp.Core
{
    public class PlayfairCipher : ICipher
    {
        public string Name => "Playfair Şifresi";
        public string Description => "5x5 tablo ve çift harf (digr af) ile şifreleme. I/J birleşik. Çözümde digraf frekansları ve tablo tahmini kullanılır.";

        public string Encrypt(string plaintext, object key)
        {
            var (grid, pos) = BuildGrid(key);
            var pairs = ToDigraphs(plaintext);
            var sb = new StringBuilder();
            foreach (var (a, b) in pairs)
            {
                var (ra, ca) = pos[a];
                var (rb, cb) = pos[b];
                if (ra == rb)
                {
                    sb.Append(grid[ra, (ca + 1) % 5]);
                    sb.Append(grid[rb, (cb + 1) % 5]);
                }
                else if (ca == cb)
                {
                    sb.Append(grid[(ra + 1) % 5, ca]);
                    sb.Append(grid[(rb + 1) % 5, cb]);
                }
                else
                {
                    sb.Append(grid[ra, cb]);
                    sb.Append(grid[rb, ca]);
                }
            }
            return sb.ToString();
        }

        public string Decrypt(string ciphertext, object key)
        {
            var (grid, pos) = BuildGrid(key);
            var input = TextUtil.OnlyLetters(ciphertext).Replace('J', 'I');
            if (input.Length % 2 == 1) input += 'X';
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i += 2)
            {
                char a = input[i], b = input[i + 1];
                var (ra, ca) = pos[a];
                var (rb, cb) = pos[b];
                if (ra == rb)
                {
                    sb.Append(grid[ra, (ca + 4) % 5]);
                    sb.Append(grid[rb, (cb + 4) % 5]);
                }
                else if (ca == cb)
                {
                    sb.Append(grid[(ra + 4) % 5, ca]);
                    sb.Append(grid[(rb + 4) % 5, cb]);
                }
                else
                {
                    sb.Append(grid[ra, cb]);
                    sb.Append(grid[rb, ca]);
                }
            }
            return sb.ToString();
        }

        public string Explain(object key)
        {
            return "Playfair, 5x5 tablo ile harf çiftlerini işler. Aynı satır/kolon için kaydırma, farklıysa dikdörtgen köşeleri kullanılır (I/J birleştirilir).";
        }

        private static (char[,], Dictionary<char, (int r, int c)>) BuildGrid(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var seen = new HashSet<char>();
            var seq = new List<char>();
            foreach (var ch in (key.ToString() ?? string.Empty).ToUpperInvariant())
            {
                if (ch < 'A' || ch > 'Z') continue;
                var c = ch == 'J' ? 'I' : ch;
                if (seen.Add(c)) seq.Add(c);
            }
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (c == 'J') continue;
                if (seen.Add(c)) seq.Add(c);
            }
            var grid = new char[5, 5];
            var pos = new Dictionary<char, (int, int)>();
            for (int i = 0; i < 25; i++)
            {
                int r = i / 5, col = i % 5;
                grid[r, col] = seq[i];
                pos[seq[i]] = (r, col);
            }
            return (grid, pos);
        }

        private static (char, char)[] ToDigraphs(string text)
        {
            var input = TextUtil.OnlyLetters(text).Replace('J', 'I');
            var list = new List<(char, char)>();
            int i = 0;
            while (i < input.Length)
            {
                char a = input[i++];
                char b = i < input.Length ? input[i] : 'X';
                if (a == b)
                {
                    b = 'X';
                }
                else
                {
                    i++;
                }
                list.Add((a, b));
            }
            if (list.Count > 0 && list[^1].Item2 == '\0')
            {
                var last = list[^1];
                list[^1] = (last.Item1, 'X');
            }
            return list.ToArray();
        }
    }
}

