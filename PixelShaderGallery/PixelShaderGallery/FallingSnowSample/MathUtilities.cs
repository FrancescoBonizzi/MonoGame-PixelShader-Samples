using System;

namespace PixelShaderGallery.FallingSnowSample
{
    public static class MathUtilities
    {
        public static Random Random { get; } = new Random();

        public static float RandomBetweenFloats(float min, float max)
            => min + (float)Random.NextDouble() * (max - min);
    }
}
