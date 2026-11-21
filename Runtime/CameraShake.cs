using System.Collections.Generic;
using UnityEngine;

namespace Wobble.Shake
{
    /// <summary>
    /// Additive shake driver. Layers are summed in <see cref="LateUpdate"/>, so
    /// follow cameras and tweens never fight each other for local transform.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CameraShake : MonoBehaviour
    {
        [SerializeField, Range(0f, 4f)] private float masterScale = 1f;
        [SerializeField, Range(1, 16)] private int maxLayers = 8;
        [SerializeField] private bool useUnscaledTime;

        private readonly List<ShakeLayer> layers = new List<ShakeLayer>(8);
        private Vector3 restPosition;
        private Quaternion restRotation;
        private float sessionClock;

        private void Awake() => RecaptureRest();

        /// <summary>Stores the current transform as the no-shake pose.</summary>
        public void RecaptureRest()
        {
            restPosition = transform.localPosition;
            restRotation = transform.localRotation;
        }

        /// <summary>Starts one shake. Extra layers beyond <c>maxLayers</c> drop oldest-first.</summary>
        public void Add(ShakeProfile profile, float scale = 1f)
        {
            if (profile == null) return;

            while (layers.Count >= Mathf.Max(1, maxLayers))
            {
                layers.RemoveAt(0);
            }

            layers.Add(new ShakeLayer(profile, sessionClock, scale));
        }

        public void StopAll() => layers.Clear();

        private void LateUpdate()
        {
            float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            sessionClock += deltaTime;

            Vector3 position = restPosition;
            float roll = 0f;

            for (int i = layers.Count - 1; i >= 0; i--)
            {
                ShakeLayer layer = layers[i];
                layer.Tick(deltaTime);

                if (layer.Finished)
                {
                    layers.RemoveAt(i);
                    continue;
                }

                ShakeLayer.Offset offset = layer.Sample();
                position += offset.Translation * masterScale;
                roll += offset.RotationDegrees * masterScale;
            }

            transform.localPosition = position;
            transform.localRotation = restRotation * Quaternion.Euler(0f, 0f, roll);
        }
    }
}
