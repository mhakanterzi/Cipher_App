using System;

namespace CipherApp.ViewModels
{
    /// <summary>
    /// Represents a numbered explanation step displayed in the educational walkthrough.
    /// </summary>
    public class StepItem
    {
        public int Index { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}

