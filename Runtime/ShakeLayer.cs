using UnityEngine;

namespace Wobble.Shake
{
    /// <summary>
    /// One running shake instance. Owned and ticked by <see cref="CameraShake"/>.
    /// Each layer keeps its own clock so overlapping shakes stay desynchronised.
    /// </summary>
    public sealed class ShakeLayer
    {
        private readonly ShakeProfile profile;
        private readonly float clockOffset;
        private readonly float scale;
        private float elapsed;

        public ShakeLayer(ShakeProfile profile, float startTime, float scale)
        {
            this.profile = profile;
            this.scale = Mathf.Max(0f, scale);
            clockOffset = (startTime + profile.Seed * 0.013f) * Mathf.Max(1f, profile.Frequency);
        }

        public bool Finished => elapsed >= profile.Duration;

        public void Tick(float deltaTime) => elapsed += deltaTime;

        public Offset Sample()
        {
            float lifetime = Mathf.Clamp01(elapsed / profile.Duration);
            float amplitude = profile.SampleEnvelope(lifetime) * profile.Weight * scale;
            float phase = (clockOffset + elapsed * profile.Frequency) * Mathf.PI * 2f;

            return new Offset(
                new Vector3(Noise(phase, 0), Noise(phase, 1), 0f) * (profile.MaxTranslation * amplitude),
                Noise(phase, 2) * profile.MaxRotation * amplitude);
        }

        // Perlin noise is cheap, smooth, and periodic enough for hand-tuned frequencies.
        private static float Noise(float phase, int axis)
            => Mathf.PerlinNoise(phase * 0.137f + axis * 3.17f, axis * 0.71f + 11.3f) * 2f - 1f;

        public readonly struct Offset
        {
            public readonly Vector3 Translation;
            public readonly float RotationDegrees;

            public Offset(Vector3 translation, float rotationDegrees)
            {
                Translation = translation;
                RotationDegrees = rotationDegrees;
            }
        }
    }
}
