using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using System;
using System.IO;

namespace PixelShaderGallery.HighlightLightsSample
{
    public class HighlightLightsSampleGame : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        private RenderTarget2D _baseSceneRenderTarget;
        private RenderTarget2D _highlightMapRenderTarget;
        private WpfMouse _mouse;
        private Texture2D _cell;
        private Texture2D _cellHightlightMap;
        private Texture2D _circularLight;
        private Effect _highlightMapShader;
        private Vector2 _lightPosition;

        public event EventHandler GameInitialized;

        protected override void Initialize()
        {
            // Must be initialized. required by Content loading and rendering (will add itself to the Services)
            // note that MonoGame requires this to be initialized in the constructor, while WpfInterop requires it to
            // be called inside Initialize (before base.Initialize())
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);
            _spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);

            // Must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();
            GameInitialized?.Invoke(this, EventArgs.Empty);

            _baseSceneRenderTarget = new RenderTarget2D(
               GraphicsDevice,
               GraphicsDevice.PresentationParameters.BackBufferWidth,
               GraphicsDevice.PresentationParameters.BackBufferHeight);

            _highlightMapRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            _mouse = new WpfMouse(this);
        }

        public void LoadInitializationData()
        {
            Content.RootDirectory = "HighlightLightsSample/Content/";

            _circularLight = Content.Load<Texture2D>("lightmask");

            using (var fileStream = new FileStream("HighlightLightsSample/Content/cell.png", FileMode.Open))
            {
                _cell = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            using (var fileStream = new FileStream("HighlightLightsSample/Content/cellspec.png", FileMode.Open))
            {
                _cellHightlightMap = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            // It is not possible to load it dynamically
            _highlightMapShader = Content.Load<Effect>("highlightlights");
        }

        public void SetBaseTexture(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                _cell = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }
        }

        public void SetHighlightMap(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                _cellHightlightMap = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var mouse = _mouse.GetState();
            _lightPosition = new Vector2(mouse.X, mouse.Y);
        }

        protected override void Draw(GameTime time)
        {
            var defaultRenderTarget = (RenderTarget2D)GraphicsDevice.GetRenderTargets()[0].RenderTarget;

            // Draws everything normal
            GraphicsDevice.SetRenderTarget(_baseSceneRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            for (int col = 0; col < GraphicsDevice.Viewport.Width; col += _cell.Width)
                for (int row = 0; row < GraphicsDevice.Viewport.Height; row += _cell.Height)
                {
                    _spriteBatch.Draw(_cell, new Vector2(col, row), Color.White);
                }
            _spriteBatch.End();

            // Draws the highlight map
            GraphicsDevice.SetRenderTarget(_highlightMapRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            for (int col = 0; col < GraphicsDevice.Viewport.Width; col += _cell.Width)
                for (int row = 0; row < GraphicsDevice.Viewport.Height; row += _cell.Height)
                {
                    _spriteBatch.Draw(_cellHightlightMap, new Vector2(col, row), Color.White);
                }
         //   _spriteBatch.Draw(_circularLight, _lightPosition, Color.White);
            _spriteBatch.End();

            // Compose postprocessed image
            GraphicsDevice.SetRenderTarget(defaultRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _highlightMapShader.Parameters["lightMask"].SetValue(_highlightMapRenderTarget);
            _highlightMapShader.CurrentTechnique.Passes[0].Apply();
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, _highlightMapShader);
            _spriteBatch.Draw(_baseSceneRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }
    }
}
