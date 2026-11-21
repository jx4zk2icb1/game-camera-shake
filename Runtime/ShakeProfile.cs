using UnityEngine;

namespace Wobble.Shake
{
    /// <summary>
    /// Designer-authored shake definition. The envelope curve drives amplitude over
    /// normalised lifetime, so hits, landings and explosions can share one engine.
    /// </summary>
    [CreateAssetMenu(menuName = "Camera Shake/Profile", fileName = "ShakeProfile")]
    public sealed class ShakeProfile : ScriptableObject
    {
        [SerializeField, Min(0.01f)] private float duration = 0.35f;
        [SerializeField, Min(0.1f)] private float frequency = 22f;
        [SerializeField, Min(0f)] private float maxTranslation = 0.25f;
        [SerializeField, Min(0f)] private float maxRotation = 2f;
        [SerializeField, Range(0f, 1f)] private float weight = 1f;
        [SerializeField] private int seed = 12345;

        [SerializeField]
        private AnimationCurve envelope = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 3.5f),
            new Keyframe(0.12f, 1f, 0f, 0f),
            new Keyframe(1f, 0f, -2f, 0f));

        public float Duration => duration;
        public float Frequency => frequency;
        public float MaxTranslation => maxTranslation;
        public float MaxRotation => maxRotation;
        public float Weight => weight;
        public int Seed => seed;

        /// <summary>Envelope value at normalised lifetime <paramref name="t01"/>.</summary>
        public float SampleEnvelope(float t01) => Mathf.Clamp01(envelope.Evaluate(Mathf.Clamp01(t01)));

        private void OnValidate()
        {
            duration = Mathf.Max(0.01f, duration);
            frequency = Mathf.Max(0.1f, frequency);
        }
    }
}
