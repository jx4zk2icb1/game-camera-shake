using UnityEngine;

namespace Wobble.Shake.Demo
{
    /// <summary>Keyboard driver for a sample scene: Space fires, 1-3 pick weight, Esc stops.</summary>
    public sealed class LandingDemo : MonoBehaviour
    {
        [SerializeField] private CameraShake shake;
        [SerializeField] private ShakeProfile[] profiles = new ShakeProfile[3];

        private int selected;

        private void Update()
        {
            if (shake == null || profiles.Length == 0)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                shake.Add(profiles[Mathf.Clamp(selected, 0, profiles.Length - 1)]);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) selected = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) selected = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) selected = 2;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                shake.StopAll();
            }
        }
    }
}
