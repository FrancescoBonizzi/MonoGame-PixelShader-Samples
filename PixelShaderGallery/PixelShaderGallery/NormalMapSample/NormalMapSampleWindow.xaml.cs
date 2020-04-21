using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PixelShaderGallery.NormalMapSample
{
    public partial class NormalMapSampleWindow : Window
    {
        private readonly NormalMapSampleGame _game;

        public NormalMapSampleWindow()
        {
            InitializeComponent();

            _game = new NormalMapSampleGame();
            GameGrid.Children.Add(_game);
            _game.GameInitialized += _game_GameInitialized;
        }

        private void _game_GameInitialized(object sender, EventArgs e)
        {
            _game.LoadInitializationData();

            LightDirectionX.ValueChanged += (obj, args) => _game.SetLightDirectionX((float)args.NewValue);
            LightDirectionY.ValueChanged += (obj, args) => _game.SetLightDirectionY((float)args.NewValue);
            LightDirectionZ.ValueChanged += (obj, args) => _game.SetLightDirectionZ((float)args.NewValue);

            LightColorX.ValueChanged += (obj, args) => _game.SetLightColorX((float)args.NewValue);
            LightColorY.ValueChanged += (obj, args) => _game.SetLightColorY((float)args.NewValue);
            LightColorZ.ValueChanged += (obj, args) => _game.SetLightColorZ((float)args.NewValue);

            AmbienceColorX.ValueChanged += (obj, args) => _game.SetAmbienceColorX((float)args.NewValue);
            AmbienceColorY.ValueChanged += (obj, args) => _game.SetAmbienceColorY((float)args.NewValue);
            AmbienceColorZ.ValueChanged += (obj, args) => _game.SetAmbienceColorZ((float)args.NewValue);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _game.GameInitialized -= _game_GameInitialized;
            _game.Dispose();
            base.OnClosing(e);
        }

        private void LoadNormalMapButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "PNG image|*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = Path.GetFullPath(openFileDialog.FileName);
                _game.SetNormalMap(filePath);
            }
        }

        private void OpenBaseImage_Click(object sender, RoutedEventArgs e)
        {
            var imageFullPath = Path.GetFullPath("NormalMapSample/Content/cell.png");
            Process.Start(imageFullPath);
        }
    }
}
