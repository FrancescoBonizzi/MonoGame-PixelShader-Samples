using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using System;
using System.IO;

namespace PixelShaderGallery.CircularNormalMapSample
{
    public class CircularNormalMapSampleGame : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        private RenderTarget2D _baseSceneRenderTarget;
        private RenderTarget2D _normalMapRenderTarget;

        private Texture2D _cell;
        private Texture2D _cellNormalMap;
        private Effect _circularNormalMapShader;
        private Vector3 _lightPosition = Vector3.Zero;
        private Vector3 _lightColor = Vector3.One;
        private Vector3 _ambienceColor = new Vector3(0.35f);

        private Matrix _worldMatrix;
        private WpfMouse _mouse;

        private float _lightDirectionZ = 0f;
        private float _lightDistanceSquared = 0f;
        private float _lightOpacity = 1f;

        public event EventHandler GameInitialized;

        protected override void Initialize()
        {
            // Must be initialized. required by Content loading and rendering (will add itself to the Services)
            // note that MonoGame requires this to be initialized in the constructor, while WpfInterop requires it to
            // be called inside Initialize (before base.Initialize())
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);
            _spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);

            _graphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferWidth = 758;
            _graphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferHeight = 729;

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

            _worldMatrix = Matrix.CreateTranslation(new Vector3(-new Vector2(0, 0), 0.0f));
            _mouse = new WpfMouse(this);
        }

        private Matrix GetProjectionMatrix()
        {
            var projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, 0, -1, 0);
            Matrix.Multiply(ref _worldMatrix, ref projection, out projection);
            return projection;
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition - new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                Matrix.Invert(_worldMatrix));
        }


        public void LoadInitializationData()
        {
            Content.RootDirectory = "CircularNormalMapSample/Content/";

            using (var fileStream = new FileStream("CircularNormalMapSample/Content/cell.png", FileMode.Open))
            {
                _cell = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            using (var fileStream = new FileStream("CircularNormalMapSample/Content/cell_normal.png", FileMode.Open))
            {
                _cellNormalMap = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            // It is not possible to load it dynamically
            _circularNormalMapShader = Content.Load<Effect>("circularnormalmap");
        }

        public void SetBaseImage(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                _cell = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }
        }

        public void SetNormalMap(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                _cellNormalMap = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }
        }

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

        public void SetLightDirectionZ(float value)
            => _lightDirectionZ = value;

        public void SetLightDistance(float value)
            => _lightDistanceSquared = value;

        public void SetLightOpacity(float value)
            => _lightOpacity = value;

        protected override void Update(GameTime gameTime)
        {
            var mouse = _mouse.GetState();
            _lightPosition = new Vector3(mouse.X, mouse.Y, _lightDirectionZ);
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

            // Draws the normal map
            GraphicsDevice.SetRenderTarget(_normalMapRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            for (int col = 0; col < GraphicsDevice.Viewport.Width; col += _cell.Width)
                for (int row = 0; row < GraphicsDevice.Viewport.Height; row += _cell.Height)
                {
                    _spriteBatch.Draw(_cellNormalMap, new Vector2(col, row), Color.White);
                }
            _spriteBatch.End();

            // Compose postprocessed image
            GraphicsDevice.SetRenderTarget(defaultRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _circularNormalMapShader.Parameters["LightPosition"].SetValue(_lightPosition);
            _circularNormalMapShader.Parameters["LightColor"].SetValue(_lightColor);
            _circularNormalMapShader.Parameters["LightOpacity"].SetValue(_lightOpacity);
            _circularNormalMapShader.Parameters["AmbientColor"].SetValue(_ambienceColor);
            _circularNormalMapShader.Parameters["World"].SetValue(_worldMatrix);
            _circularNormalMapShader.Parameters["LightDistanceSquared"].SetValue(_lightDistanceSquared);
            _circularNormalMapShader.Parameters["ViewProjection"].SetValue(GetProjectionMatrix());
            _circularNormalMapShader.Parameters["NormalTexture"].SetValue((Texture2D)_normalMapRenderTarget);
            _circularNormalMapShader.CurrentTechnique.Passes[0].Apply();
            
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null, _circularNormalMapShader);
            _spriteBatch.Draw((Texture2D)_baseSceneRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }
    }
}
