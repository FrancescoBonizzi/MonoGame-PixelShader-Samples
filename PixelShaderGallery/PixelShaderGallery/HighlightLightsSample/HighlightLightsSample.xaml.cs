using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PixelShaderGallery.HighlightLightsSample
{
    public partial class HighlightLightsSampleWindow : Window
    {
        private readonly HighlightLightsSampleGame _game;

        public HighlightLightsSampleWindow()
        {
            InitializeComponent();

            _game = new HighlightLightsSampleGame();
            GameGrid.Children.Add(_game);
            _game.GameInitialized += _game_GameInitialized;
        }

        private void _game_GameInitialized(object sender, EventArgs e)
        {
            _game.LoadInitializationData();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _game.GameInitialized -= _game_GameInitialized;
            _game.Dispose();
            base.OnClosing(e);
        }

        private void LoadNormalMapButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void OpenBaseImage_Click(object sender, RoutedEventArgs e)
        {
            var imageFullPath = Path.GetFullPath("HighlightLightsSample/Content/cell.png");
            Process.Start(imageFullPath);
        }

        private void OpenHighlightImage_Click(object sender, RoutedEventArgs e)
        {
            var imageFullPath = Path.GetFullPath("HighlightLightsSample/Content/cellspec.png");
            Process.Start(imageFullPath);
        }

        private void LoadHighlightMapButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "PNG image|*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = Path.GetFullPath(openFileDialog.FileName);
                _game.SetHighlightMap(filePath);
            }
        }

        private void LoadBaseTextureButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "PNG image|*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = Path.GetFullPath(openFileDialog.FileName);
                _game.SetBaseTexture(filePath);
            }
        }
    }
}
