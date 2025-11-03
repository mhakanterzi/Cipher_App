using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace CipherApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, __) => FadeIn();
        CipherListBox.SelectionChanged += (_, __) => FadeIn();
    }

    private void FadeIn()
    {
        var anim = new DoubleAnimation
        {
            From = 0.0,
            To = 1.0,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        ContentPanel.BeginAnimation(OpacityProperty, anim);
    }
}
