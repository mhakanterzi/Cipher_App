using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CipherApp.Core;

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
            }
            catch
            {
                Explanation = string.Empty;
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
    }
}

