using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PixelShaderGallery.FallingSnowSample
{
    public abstract class SnowingManagerBase
    {
        private float _generateTimer;
        private float _swayTimer;
        protected readonly Rectangle _snowingBounds;
        protected readonly Texture2D _snowFlakeParticle;
        protected List<SnowFlake> _particles;

        /// <summary>
        /// How often a particle is produced
        /// </summary>
        public float GenerateSpeed = 1f;

        /// <summary>
        /// How often we apply the "GlobalVelociy" to our particles
        /// </summary>
        public float GlobalVelocitySpeed = 4;

        public int MaxParticles = 5;

        public SnowingManagerBase(Texture2D snowFlakeParticle, Rectangle snowingBounds)
        {
            _snowingBounds = snowingBounds;
            _snowFlakeParticle = snowFlakeParticle;
            _particles = new List<SnowFlake>();
        }

        public void Update(GameTime gameTime)
        {
            _generateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _swayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            AddParticle();

            if (_swayTimer > GlobalVelocitySpeed)
            {
                _swayTimer = 0;

                ApplyGlobalVelocity();
            }

            foreach (var particle in _particles)
            {
                if(!IsOnScreen(particle))
                    particle.IsRemoved = true;

                particle.Update(gameTime);
            }

            RemovedFinishedParticles();
        }

        private bool IsOnScreen(SnowFlake snowFlake)
        {
            return snowFlake.Position.Y <= (_snowingBounds.Y + _snowingBounds.Height + 10);
        }

        private void AddParticle()
        {
            if (_generateTimer > GenerateSpeed)
            {
                _generateTimer = 0;

                if (_particles.Count < MaxParticles)
                {
                    _particles.Add(GenerateParticle());
                }
            }
        }

        protected abstract void ApplyGlobalVelocity();

        private void RemovedFinishedParticles()
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                if (_particles[i].IsRemoved)
                {
                    _particles.RemoveAt(i);
                    i--;
                }
            }
        }

        protected abstract SnowFlake GenerateParticle();

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var particle in _particles)
                particle.Draw(gameTime, spriteBatch);
        }
    }
}
