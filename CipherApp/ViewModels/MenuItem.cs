namespace CipherApp.ViewModels
{
    /// <summary>
    /// Describes a navigation entry used by the menu list in the WPF shell.
    /// </summary>
    public class MenuItem
    {
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Icon { get; set; } = ""; // emoji or glyph
        public int TabIndex { get; set; } // maps to TabControl index
    }
}

