using System;
using System.ComponentModel;
using System.Windows;

namespace PixelShaderGallery.FallingSnowSample
{
    public partial class FallingSnowSampleWindow : Window
    {
        private readonly FallingSnowSampleGame _game;

        public FallingSnowSampleWindow()
        {
            InitializeComponent();

            _game = new FallingSnowSampleGame();
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
