using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CipherApp.Core;
using System.Linq;

namespace CipherApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ICipher> CipherList { get; } = new();

        private ICipher? _selectedCipher;
        public ICipher? SelectedCipher
        {
            get => _selectedCipher;
            set { _selectedCipher = value; OnPropertyChanged(); UpdateExplanation(); }
        }

        private string _keyInput = string.Empty;
        public string KeyInput
        {
            get => _keyInput;
            set { _keyInput = value; OnPropertyChanged(); }
        }

        private string _plaintext = string.Empty;
        public string Plaintext
        {
            get => _plaintext;
            set { _plaintext = value; OnPropertyChanged(); }
        }

        private string _output = string.Empty;
        public string Output
        {
            get => _output;
            set { _output = value; OnPropertyChanged(); }
        }

        private string _explanation = string.Empty;
        public string Explanation
        {
            get => _explanation;
            set { _explanation = value; OnPropertyChanged(); }
        }

        private string _keyHint = string.Empty;
        public string KeyHint
        {
            get => _keyHint;
            set { _keyHint = value; OnPropertyChanged(); }
        }

        // Quiz
        private readonly QuizGenerator _quiz = new();
        private CipherQuestion? _currentQuestion;
        public CipherQuestion? CurrentQuestion
        {
            get => _currentQuestion;
            set { _currentQuestion = value; OnPropertyChanged(); }
        }

        public ICommand EncryptCommand { get; }
        public ICommand DecryptCommand { get; }
        public ICommand NewQuizCommand { get; }
        public ICommand ShowAnswerCommand { get; }

        // Step-by-step
        public ObservableCollection<StepItem> Steps { get; } = new();
        private int _currentStepIndex = -1;
        public int CurrentStepIndex
        {
            get => _currentStepIndex;
            set { _currentStepIndex = value; OnPropertyChanged(); }
        }
        public ICommand GenerateEncryptStepsCommand { get; }
        public ICommand GenerateDecryptStepsCommand { get; }
        public ICommand NextStepCommand { get; }
        public ICommand ResetStepsCommand { get; }

        public MainViewModel()
        {
            CipherList.Add(new CaesarCipher());
            CipherList.Add(new MonoalphabeticCipher());
            CipherList.Add(new PlayfairCipher());
            CipherList.Add(new HillCipher());
            CipherList.Add(new VigenereCipher());
            CipherList.Add(new TranspositionCipher());
            SelectedCipher = CipherList[0];

            EncryptCommand = new RelayCommand(DoEncrypt, () => SelectedCipher != null);
            DecryptCommand = new RelayCommand(DoDecrypt, () => SelectedCipher != null);
            NewQuizCommand = new RelayCommand(DoNewQuiz);
            ShowAnswerCommand = new RelayCommand(DoShowAnswer);

            GenerateEncryptStepsCommand = new RelayCommand(() => GenerateSteps(encrypt: true));
            GenerateDecryptStepsCommand = new RelayCommand(() => GenerateSteps(encrypt: false));
            NextStepCommand = new RelayCommand(AdvanceStep);
            ResetStepsCommand = new RelayCommand(ResetSteps);
        }

        private void DoEncrypt()
        {
            if (SelectedCipher == null) return;
            try
            {
                Output = SelectedCipher.Encrypt(Plaintext ?? string.Empty, KeyInput);
            }
            catch (Exception ex)
            {
                Output = $"Hata: {ex.Message}";
            }
        }

        private void DoDecrypt()
        {
            if (SelectedCipher == null) return;
            try
            {
                Output = SelectedCipher.Decrypt(Plaintext ?? string.Empty, KeyInput);
            }
            catch (Exception ex)
            {
                Output = $"Hata: {ex.Message}";
            }
        }

        private void UpdateExplanation()
        {
            if (SelectedCipher == null) { Explanation = string.Empty; return; }
            try
            {
                Explanation = SelectedCipher.Description;
                KeyHint = SelectedCipher switch
                {
                    CaesarCipher => "Anahtar: sayı (ör. 3)",
                    VigenereCipher => "Anahtar: kelime (ör. ANAHTAR)",
                    MonoalphabeticCipher => "Anahtar: 26 harflik permütasyon",
                    PlayfairCipher => "Anahtar: kelime (I/J birleşik)",
                    HillCipher => "Anahtar: 'a b c d' (ör. 3 3 2 5)",
                    TranspositionCipher => "Anahtar: kelime (kolon)",
                    _ => string.Empty
                };
            }
            catch
            {
                Explanation = string.Empty;
                KeyHint = string.Empty;
            }
        }

        private void DoNewQuiz()
        {
            CurrentQuestion = _quiz.Next();
        }

        private void DoShowAnswer()
        {
            if (CurrentQuestion == null) return;
            // Show the decryption of the ciphertext using the cipher name match
            Explanation = $"Soru Çözümü:\n{CurrentQuestion.Explanation}\n\nDüz metin: {CurrentQuestion.Plaintext}\nŞifreli: {CurrentQuestion.Ciphertext}\nŞifre: {CurrentQuestion.CipherName}\nAnahtar: {CurrentQuestion.KeyDescription}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Step generation
        private void GenerateSteps(bool encrypt)
        {
            Steps.Clear();
            CurrentStepIndex = -1;
            if (SelectedCipher == null) return;

            var text = Plaintext ?? string.Empty;
            var key = KeyInput ?? string.Empty;

            try
            {
                if (SelectedCipher is CaesarCipher)
                {
                    if (!int.TryParse(key, out var k)) throw new("Anahtar sayı olmalı (örn: 3)");
                    var input = text.ToUpperInvariant();
                    int idx = 1;
                    Steps.Add(new StepItem { Index = idx++, Text = encrypt ? $"Her harfi +{k} kaydır." : $"Her harfi -{k} kaydır." });
                    foreach (var ch in input)
                    {
                        if (ch < 'A' || ch > 'Z') { Steps.Add(new StepItem { Index = idx++, Text = $"{ch} → aynen bırak" }); continue; }
                        int pos = ch - 'A';
                        int pos2 = ((pos + (encrypt ? k : -k)) % 26 + 26) % 26;
                        char outc = (char)('A' + pos2);
                        Steps.Add(new StepItem { Index = idx++, Text = $"{ch}({pos}) {(encrypt ? "+" : "-")} {Math.Abs(k)} = {pos2} → {outc}" });
                    }
                }
                else if (SelectedCipher is VigenereCipher)
                {
                    var kstr = new string(key.ToUpperInvariant().Where(c => c >= 'A' && c <= 'Z').ToArray());
                    if (kstr.Length == 0) throw new("Anahtar kelime olmalı");
                    var input = text.ToUpperInvariant();
                    Steps.Add(new StepItem { Index = 1, Text = $"Anahtar tekrarı: {RepeatKey(kstr, input.Length)}" });
                    int idx = 2; int ki = 0;
                    foreach (var ch in input)
                    {
                        if (ch < 'A' || ch > 'Z') { Steps.Add(new StepItem { Index = idx++, Text = $"{ch} → aynen bırak" }); continue; }
                        int p = ch - 'A'; int k = kstr[ki % kstr.Length] - 'A';
                        int r = ((encrypt ? p + k : p - k) % 26 + 26) % 26; char outc = (char)('A' + r);
                        Steps.Add(new StepItem { Index = idx++, Text = $"{ch}({p}) {(encrypt ? "+" : "-")} {kstr[ki%kstr.Length]}({k}) = {r} → {outc}" });
                        ki++;
                    }
                }
                else if (SelectedCipher is PlayfairCipher)
                {
                    // Build grid and digraphs, then describe rule used
                    var (grid, pos) = BuildPlayfair(key);
                    Steps.Add(new StepItem { Index = 1, Text = "5x5 tablo hazırlandı (I/J birleşik)." });
                    Steps.Add(new StepItem { Index = 2, Text = GridToText(grid) });
                    var pairs = ToDigraphs(text);
                    int idx = 3;
                    foreach (var (a, b) in pairs)
                    {
                        var (ra, ca) = pos[a]; var (rb, cb) = pos[b];
                        if (ra == rb)
                            Steps.Add(new StepItem { Index = idx++, Text = $"{a}{b}: Aynı satır → sağa kaydır" });
                        else if (ca == cb)
                            Steps.Add(new StepItem { Index = idx++, Text = $"{a}{b}: Aynı sütun → aşağı kaydır" });
                        else
                            Steps.Add(new StepItem { Index = idx++, Text = $"{a}{b}: Dikdörtgen → köşe değişimi" });
                    }
                }
                else
                {
                    Steps.Add(new StepItem { Index = 1, Text = "Girdi normalleştir (harfler, büyük harf)." });
                    Steps.Add(new StepItem { Index = 2, Text = "Algoritma kurallarını sırayla uygula." });
                    Steps.Add(new StepItem { Index = 3, Text = "Sonucu derle ve doğrula." });
                }
            }
            catch (System.Exception ex)
            {
                Steps.Clear();
                Steps.Add(new StepItem { Index = 1, Text = $"Hata: {ex.Message}" });
            }
        }

        private void AdvanceStep()
        {
            if (Steps.Count == 0) return;
            CurrentStepIndex = (CurrentStepIndex + 1) % Steps.Count;
        }

        private void ResetSteps()
        {
            CurrentStepIndex = -1;
        }

        private static string RepeatKey(string key, int len)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            return new string(Enumerable.Range(0, len).Select(i => key[i % key.Length]).ToArray());
        }

        // Minimal Playfair helpers for steps
        private static (char[,], System.Collections.Generic.Dictionary<char, (int r, int c)>) BuildPlayfair(object key)
        {
            var seen = new System.Collections.Generic.HashSet<char>();
            var seq = new System.Collections.Generic.List<char>();
            foreach (var ch in (key?.ToString() ?? string.Empty).ToUpperInvariant())
            {
                if (ch < 'A' || ch > 'Z') continue; var c = ch == 'J' ? 'I' : ch; if (seen.Add(c)) seq.Add(c);
            }
            for (char c = 'A'; c <= 'Z'; c++) { if (c == 'J') continue; if (seen.Add(c)) seq.Add(c); }
            var grid = new char[5, 5]; var pos = new System.Collections.Generic.Dictionary<char, (int,int)>();
            for (int i = 0; i < 25; i++) { int r = i / 5, col = i % 5; grid[r, col] = seq[i]; pos[seq[i]] = (r, col); }
            return (grid, pos);
        }

        private static (char,char)[] ToDigraphs(string text)
        {
            var input = new string((text ?? string.Empty).ToUpperInvariant().Where(c => c >= 'A' && c <= 'Z').ToArray()).Replace('J', 'I');
            var list = new System.Collections.Generic.List<(char,char)>(); int i = 0; while (i < input.Length)
            { char a = input[i++]; char b = i < input.Length ? input[i] : 'X'; if (a == b) { b = 'X'; } else { i++; } list.Add((a,b)); }
            if (list.Count > 0 && list[^1].Item2 == '\0') { var last = list[^1]; list[^1] = (last.Item1, 'X'); }
            return list.ToArray();
        }

        private static string GridToText(char[,] grid)
        {
            var lines = new System.Collections.Generic.List<string>();
            for (int r = 0; r < 5; r++)
            {
                var row = new char[5]; for (int c = 0; c < 5; c++) row[c] = grid[r, c];
                lines.Add(string.Join(" ", row));
            }
            return string.Join(" | ", lines);
        }
    }
}

