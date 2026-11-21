using System.Collections.Generic;
using UnityEngine;

namespace Wobble.Shake.Blending
{
    public enum ShakeMode
    {
        Additive = 0,
        Strongest = 1,
        WeightedAverage = 2,
    }

    /// <summary>
    /// Combines sampled layer offsets. Additive suits impacts that should stack;
    /// Strongest keeps direction coherent; WeightedAverage smooths crowds of
    /// near-equal shakes into one soft push.
    /// </summary>
    public static class ShakeMixer
    {
        public static ShakeLayer.Offset Mix(IReadOnlyList<ShakeLayer.Offset> offsets, ShakeMode mode)
        {
            if (offsets.Count == 0)
            {
                return default;
            }

            switch (mode)
            {
                case ShakeMode.Strongest:
                    return Strongest(offsets);
                case ShakeMode.WeightedAverage:
                    return Weighted(offsets);
                default:
                    return Sum(offsets);
            }
        }

        private static ShakeLayer.Offset Sum(IReadOnlyList<ShakeLayer.Offset> offsets)
        {
            Vector3 translation = Vector3.zero;
            float rotation = 0f;

            for (int i = 0; i < offsets.Count; i++)
            {
                translation += offsets[i].Translation;
                rotation += offsets[i].RotationDegrees;
            }

            return new ShakeLayer.Offset(translation, rotation, 1f);
        }

        private static ShakeLayer.Offset Strongest(IReadOnlyList<ShakeLayer.Offset> offsets)
        {
            ShakeLayer.Offset best = offsets[0];

            for (int i = 1; i < offsets.Count; i++)
            {
                if (offsets[i].Amplitude > best.Amplitude)
                {
                    best = offsets[i];
                }
            }

            return best;
        }

        private static ShakeLayer.Offset Weighted(IReadOnlyList<ShakeLayer.Offset> offsets)
        {
            Vector3 translation = Vector3.zero;
            float rotation = 0f;
            float total = 0f;

            for (int i = 0; i < offsets.Count; i++)
            {
                float weight = Mathf.Max(offsets[i].Amplitude, 0.0001f);
                translation += offsets[i].Translation * weight;
                rotation += offsets[i].RotationDegrees * weight;
                total += weight;
            }

            return new ShakeLayer.Offset(translation / total, rotation / total, 1f);
        }
    }
}
