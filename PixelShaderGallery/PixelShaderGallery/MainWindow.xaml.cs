using System.Windows;

namespace PixelShaderGallery
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NormalMapButton_Click(object sender, RoutedEventArgs e)
        {
            var gallery = new NormalMapSample.NormalMapSampleWindow();
            gallery.ShowDialog();
        }
    }
}
