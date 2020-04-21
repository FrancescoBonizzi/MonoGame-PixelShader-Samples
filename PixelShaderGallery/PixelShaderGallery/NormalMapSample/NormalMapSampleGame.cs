using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;
using System.IO;
using System.Windows.Input;

namespace PixelShaderGallery.NormalMapSample
{
    public class NormalMapSampleGame : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        private RenderTarget2D _baseSceneRenderTarget;
        private RenderTarget2D _normalMapRenderTarget;

        private Texture2D _cell;
        private Texture2D _cellNormalMap;
        private Effect _normalMapShader;
        private Vector3 _lightDirection = Vector3.Zero;
        private Vector3 _lightColor = Vector3.One;
        private Vector3 _ambienceColor = new Vector3(0.35f);

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

            _normalMapRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

        }

        public void LoadInitializationData()
        {
            Content.RootDirectory = "NormalMapSample/Content/";

            using (var fileStream = new FileStream("NormalMapSample/Content/cell.png", FileMode.Open))
            {
                _cell = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            using (var fileStream = new FileStream("NormalMapSample/Content/cell_normal.png", FileMode.Open))
            {
                _cellNormalMap = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            // It is not possible to load it dynamically
            _normalMapShader = Content.Load<Effect>("normalmap");
        }

        public void SetNormalMap(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                _cellNormalMap = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }
        }

        public void SetLightDirectionX(float value)
            => _lightDirection.X = value;

        public void SetLightDirectionY(float value)
            => _lightDirection.Y = value;

        public void SetLightDirectionZ(float value)
            => _lightDirection.Z = value;

        public void SetLightColorX(float value)
            => _lightColor.X = value;

        public void SetLightColorY(float value)
            => _lightColor.Y = value;

        public void SetLightColorZ(float value)
            => _lightColor.Z = value;

        public void SetAmbienceColorX(float value)
            => _ambienceColor.X = value;

        public void SetAmbienceColorY(float value)
            => _ambienceColor.Y = value;

        public void SetAmbienceColorZ(float value)
            => _ambienceColor.Z = value;

        protected override void Update(GameTime gameTime)
        {

        }

        protected override void Draw(GameTime time)
        {
            //GraphicsDevice.Clear(Color.Black);
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

            //// Draws the normal map
            GraphicsDevice.SetRenderTarget(_normalMapRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            for (int col = 0; col < GraphicsDevice.Viewport.Width; col += _cell.Width)
                for (int row = 0; row < GraphicsDevice.Viewport.Height; row += _cell.Height)
                {
                    _spriteBatch.Draw(_cellNormalMap, new Vector2(col, row), Color.White);
                }
            _spriteBatch.End();

            //// Compose postprocessed image
            GraphicsDevice.SetRenderTarget(defaultRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _normalMapShader.Parameters["LightDirection"].SetValue(_lightDirection);
            _normalMapShader.Parameters["LightColor"].SetValue(_lightColor);
            _normalMapShader.Parameters["AmbientColor"].SetValue(_ambienceColor);
            _normalMapShader.Parameters["NormalTexture"].SetValue((Texture2D)_normalMapRenderTarget);
            _normalMapShader.CurrentTechnique.Passes[0].Apply();

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null, _normalMapShader);
            _spriteBatch.Draw((Texture2D)_baseSceneRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }
    }
}
