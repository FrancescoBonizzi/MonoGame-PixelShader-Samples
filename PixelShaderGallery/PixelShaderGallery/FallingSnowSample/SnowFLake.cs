using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelShaderGallery.FallingSnowSample
{
    public class SnowFlake
    {
        protected Texture2D _texture;

        public float Opacity { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; } = 0;// MathHelper.ToRadians(MathUtilities.Random.Next(0, 360));
        public float Scale { get; set; } = 1f;

        public Rectangle Bounds => _texture.Bounds;

        public Vector2 Position;
        public Vector2 Velocity;

        public bool IsRemoved { get; set; }

        public SnowFlake(Texture2D texture)
        {
            _texture = texture;
            Opacity = 1f;
            Origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: _texture,
                position: Position,
                sourceRectangle: null,
                color: Color.White * Opacity,
                rotation: Rotation,
                origin: Origin,
                scale: Scale,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}
