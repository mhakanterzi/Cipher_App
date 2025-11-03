namespace CipherApp.ViewModels
{
    public class MenuItem
    {
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Icon { get; set; } = ""; // emoji or glyph
        public int TabIndex { get; set; } // maps to TabControl index
    }
}

