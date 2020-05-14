using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PixelShaderGallery.SpecularMapSample
{
    public partial class SpecularMapSampleWindow : Window
    {
        private readonly SpecularMapSampleGame _game;

        public SpecularMapSampleWindow()
        {
            InitializeComponent();

            _game = new SpecularMapSampleGame();
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

        private void LoadSpecularMapButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "PNG image|*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = Path.GetFullPath(openFileDialog.FileName);
                _game.SetSpecularMap(filePath);
            }
        }

        private void LoadBaseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "PNG image|*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = Path.GetFullPath(openFileDialog.FileName);
                _game.SetBaseImage(filePath);
            }
        }

        private void OpenBaseImage_Click(object sender, RoutedEventArgs e)
        {
            var imageFullPath = Path.GetFullPath("SpecularMapSample/Content/cell.png");
            Process.Start(imageFullPath);
        }

        private void OpenSpecularImage_Click(object sender, RoutedEventArgs e)
        {
            var imageFullPath = Path.GetFullPath("SpecularMapSample/Content/cell_specular.png");
            Process.Start(imageFullPath);
        }

    }
}
