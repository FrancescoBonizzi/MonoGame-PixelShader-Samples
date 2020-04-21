using System;
using System.ComponentModel;
using System.Windows;

namespace PixelShaderGallery.CircularNormalMapSample
{
    public partial class CircularNormalMapSampleWindow : Window
    {
        private readonly CircularNormalMapSampleGame _game;

        public CircularNormalMapSampleWindow()
        {
            InitializeComponent();

            _game = new CircularNormalMapSampleGame();
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
    }
}
