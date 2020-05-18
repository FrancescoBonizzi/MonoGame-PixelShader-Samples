using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;
using System.IO;

namespace PixelShaderGallery.FallingSnowSample
{
    public class FallingSnowSampleGame : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        private SnowingManager _snowingManager0;
        private SnowingManager _snowingManager1;
        private SnowingManager _snowingManager2;

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
        }

        public void LoadInitializationData()
        {
            Content.RootDirectory = "FallingSnowSample/Content/";

            Texture2D snowFlakeTexture;
            using (var fileStream = new FileStream("FallingSnowSample/Content/snowflake.png", FileMode.Open))
            {
                snowFlakeTexture = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            _snowingManager0 = new SnowingManager(
                snowFlakeTexture, 
                new Rectangle(0, 0, _graphicsDeviceManager.GraphicsDevice.Viewport.Bounds.Width, _graphicsDeviceManager.GraphicsDevice.Viewport.Bounds.Height));
            _snowingManager1 = new SnowingManager(
                snowFlakeTexture,
                new Rectangle(0, 0, _graphicsDeviceManager.GraphicsDevice.Viewport.Bounds.Width, _graphicsDeviceManager.GraphicsDevice.Viewport.Bounds.Height - 150));
            _snowingManager2 = new SnowingManager(
                snowFlakeTexture,
                new Rectangle(0, 0, _graphicsDeviceManager.GraphicsDevice.Viewport.Bounds.Width, _graphicsDeviceManager.GraphicsDevice.Viewport.Bounds.Height - 250));
        }

        protected override void Update(GameTime gameTime)
        {
            _snowingManager0.Update(gameTime);
            _snowingManager1.Update(gameTime);
            _snowingManager2.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MidnightBlue);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _snowingManager0.Draw(gameTime, _spriteBatch);
            _snowingManager1.Draw(gameTime, _spriteBatch);
            _snowingManager2.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
        }

    }
}
