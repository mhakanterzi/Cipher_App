using System;
using System.IO;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;

namespace CipherApp.Views
{
    public partial class ReactView : UserControl
    {
        public ReactView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var indexPath = Path.Combine(appDir, "wwwroot", "index.html");
            if (!File.Exists(indexPath))
            {
                EmptyState.Visibility = System.Windows.Visibility.Visible;
                Web.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            try
            {
                await Web.EnsureCoreWebView2Async();
                var uri = new Uri(indexPath);
                Web.Source = uri; // file:///...
            }
            catch
            {
                EmptyState.Visibility = System.Windows.Visibility.Visible;
                Web.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}

