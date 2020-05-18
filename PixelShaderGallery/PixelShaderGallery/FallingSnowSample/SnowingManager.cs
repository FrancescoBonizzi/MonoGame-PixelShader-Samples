using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PixelShaderGallery.FallingSnowSample
{
    public class SnowingManager
    {
        private readonly Rectangle _snowingBounds;
        private readonly Texture2D _snowFlakeParticle;

        private TimeSpan _currentParticleGenerationElapsed;
        private TimeSpan _currentWindElapsed;
        
        private readonly List<SnowFlake> _particles;

        public TimeSpan ParticleGenerationInterval { get; } = TimeSpan.FromSeconds(1f);

        /// <summary>
        /// How often we apply the wind to our particles
        /// </summary>
        public TimeSpan WindInterval { get; } = TimeSpan.FromSeconds(4);

        public int MaxParticles { get; } = 5;

        public SnowingManager(Texture2D snowFlakeParticle, Rectangle snowingBounds)
        {
            _snowingBounds = snowingBounds;
            _snowFlakeParticle = snowFlakeParticle;
            _particles = new List<SnowFlake>();
        }

        private void ApplyWindVelocityToAllParticles()
        {
            var xSway = (float)MathUtilities.Random.Next(-20, 20);
            foreach (var particle in _particles)
                particle.Velocity.X = (xSway * particle.Scale) / 50;
        }

        private SnowFlake GenerateParticle()
        {
            var sprite = new SnowFlake(_snowFlakeParticle);

            var xPosition = MathUtilities.Random.Next(_snowingBounds.X, _snowingBounds.Width);
            var ySpeed = MathUtilities.Random.Next(70, 75) / 100f;

            sprite.Position = new Vector2(xPosition, -sprite.Bounds.Height);
            sprite.Opacity = (float)MathUtilities.Random.NextDouble();
            sprite.Velocity = new Vector2(0, ySpeed);

            return sprite;
        }

        public void Update(TimeSpan elapsed)
        {
            _currentParticleGenerationElapsed += elapsed;
            _currentWindElapsed += elapsed;

            AddParticle();

            if (_currentWindElapsed > WindInterval)
            {
                _currentWindElapsed = TimeSpan.Zero;
                ApplyWindVelocityToAllParticles();
            }

            foreach (var particle in _particles)
            {
                if (!IsOnScreen(particle))
                    particle.IsRemoved = true;

                particle.Update(elapsed);
            }

            RemovedFinishedParticles();
        }

        private void RemovedFinishedParticles()
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                if (_particles[i].IsRemoved)
                {
                    _particles.RemoveAt(i);
                    --i;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var particle in _particles)
                particle.Draw(gameTime, spriteBatch);
        }

        private bool IsOnScreen(SnowFlake snowFlake)
        {
            return snowFlake.Position.Y <= (_snowingBounds.Y + _snowingBounds.Height + 10);
        }

        private void AddParticle()
        {
            if (_currentParticleGenerationElapsed > ParticleGenerationInterval)
            {
                _currentParticleGenerationElapsed = TimeSpan.Zero;

                if (_particles.Count < MaxParticles)
                {
                    _particles.Add(GenerateParticle());
                }
            }
        }
    }
}
