﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using System;
using System.IO;

namespace PixelShaderGallery.SpecularMapSample
{
    public class SpecularMapSampleGame : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        private RenderTarget2D _baseSceneRenderTarget;
        private RenderTarget2D _specularMapRenderTarget;
        private RenderTarget2D _specularMapLightMaskRenderTarget;
        private RenderTarget2D _lightsRenderTarget;

        private WpfMouse _mouse;
        private Texture2D _cell;
        private Texture2D _cellSpecularMap;
        private Texture2D _lightTexture;
        private Effect _specularMapShader;

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

            _specularMapRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            _lightsRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            _specularMapLightMaskRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            _mouse = new WpfMouse(this);
        }

        public void LoadInitializationData()
        {
            Content.RootDirectory = "SpecularMapSample/Content/";

            using (var fileStream = new FileStream("SpecularMapSample/Content/cell.png", FileMode.Open))
            {
                _cell = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            using (var fileStream = new FileStream("SpecularMapSample/Content/cell_spec.png", FileMode.Open))
            {
                _cellSpecularMap = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            using (var fileStream = new FileStream("SpecularMapSample/Content/light.png", FileMode.Open))
            {
                _lightTexture = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }

            // It is not possible to load it dynamically
            _specularMapShader = Content.Load<Effect>("specularmap");
        }

        public void SetBaseImage(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                _cell = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }
        }

        public void SetSpecularMap(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                _cellSpecularMap = Texture2D.FromStream(_graphicsDeviceManager.GraphicsDevice, fileStream);
            }
        }


        protected override void Draw(GameTime time)
        {
            var mousePosition = _mouse.GetState();
            var defaultRenderTarget = (RenderTarget2D)GraphicsDevice.GetRenderTargets()[0].RenderTarget;

            // Draws everything normal
            GraphicsDevice.SetRenderTarget(_baseSceneRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
            for (int col = 0; col < GraphicsDevice.Viewport.Width; col += _cell.Width)
                for (int row = 0; row < GraphicsDevice.Viewport.Height; row += _cell.Height)
                {
                    _spriteBatch.Draw(_cell, new Vector2(col, row), Color.White);
                }
            _spriteBatch.End();

            // Draws the specular map
            GraphicsDevice.SetRenderTarget(_specularMapRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            for (int col = 0; col < GraphicsDevice.Viewport.Width; col += _cell.Width)
                for (int row = 0; row < GraphicsDevice.Viewport.Height; row += _cell.Height)
                {
                    _spriteBatch.Draw(_cellSpecularMap, new Vector2(col, row), Color.White);
                }
            _spriteBatch.End();

            // Draws the lightmask for the specular
            GraphicsDevice.SetRenderTarget(_specularMapLightMaskRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
            _spriteBatch.Draw(
               texture: _lightTexture,
               position: new Vector2(mousePosition.X, mousePosition.Y),
               color: Color.White,
               origin: new Vector2(_lightTexture.Width / 2, _lightTexture.Height / 2),
               scale: new Vector2(2f));
            _spriteBatch.End();

            // Applies the lightmask in specular
            GraphicsDevice.SetRenderTarget(_lightsRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _specularMapShader.Parameters["SpecularTexture"].SetValue((Texture2D)_specularMapLightMaskRenderTarget);
            _specularMapShader.CurrentTechnique.Passes[0].Apply();
            _spriteBatch.Begin(effect: _specularMapShader);
            _spriteBatch.Draw(_specularMapRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            // Compose postprocessed image
            GraphicsDevice.SetRenderTarget(defaultRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(blendState: BlendState.Additive);
            _spriteBatch.Draw((Texture2D)_baseSceneRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.Draw((Texture2D)_lightsRenderTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }
    }
}
