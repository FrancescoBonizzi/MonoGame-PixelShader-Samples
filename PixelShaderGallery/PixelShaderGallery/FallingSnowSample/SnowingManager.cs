using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelShaderGallery.FallingSnowSample
{
    public class SnowingManager : SnowingManagerBase
    {
        public SnowingManager(Texture2D snowFlake, Rectangle snowingBounds)
          : base(snowFlake, snowingBounds)
        {

        }

        protected override void ApplyGlobalVelocity()
        {
            var xSway = (float)MathUtilities.Random.Next(-20, 20);
            foreach (var particle in _particles)
                particle.Velocity.X = (xSway * particle.Scale) / 50;
        }

        protected override SnowFlake GenerateParticle()
        {
            var sprite = new SnowFlake(_snowFlakeParticle);

            var xPosition = MathUtilities.Random.Next(_snowingBounds.X, _snowingBounds.Width);
            var ySpeed = MathUtilities.Random.Next(70, 75) / 100f;

            sprite.Position = new Vector2(xPosition, -sprite.Bounds.Height);
            sprite.Opacity = (float)MathUtilities.Random.NextDouble();
      //      sprite.Rotation = MathHelper.ToRadians(MathUtilities.Random.Next(0, 360));
            // sprite.Scale = (float)MathUtilities.Random.NextDouble() + MathUtilities.Random.Next(0, 3);
            sprite.Velocity = new Vector2(0, ySpeed);

            return sprite;
        }
    }
}
