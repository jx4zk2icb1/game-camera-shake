using System.Collections.Generic;
using UnityEngine;

namespace Wobble.Shake.Falloff
{
    /// <summary>
    /// Converts world position into a shake scale: full strength inside
    /// <see cref="innerRadius"/>, fading to zero at <see cref="radius"/>.
    /// Place one on an explosion or a stamping boss.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public sealed class FalloffVolume : MonoBehaviour
    {
        private static readonly List<FalloffVolume> Active = new List<FalloffVolume>();

        [SerializeField, Min(0f)] private float innerRadius = 3f;
        [SerializeField, Min(0.01f)] private float radius = 9f;

        private void OnEnable() => Active.Add(this);
        private void OnDisable() => Active.Remove(this);

        /// <summary>Strongest attenuation of any enabled volume at <paramref name="worldPosition"/>.</summary>
        public static float Attenuation(Vector3 worldPosition)
        {
            float strongest = 1f;

            for (int i = 0; i < Active.Count; i++)
            {
                strongest = Mathf.Min(strongest, Active[i].ScaleAt(worldPosition));
            }

            return strongest;
        }

        public float ScaleAt(Vector3 worldPosition)
        {
            float distance = Vector3.Distance(worldPosition, transform.position);
            float t = Mathf.InverseLerp(innerRadius, Mathf.Max(innerRadius + 0.01f, radius), distance);
            return Mathf.SmoothStep(1f, 0f, t);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.88f, 0.38f, 0.23f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, innerRadius);
            Gizmos.color = new Color(0.88f, 0.38f, 0.23f, 0.12f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
