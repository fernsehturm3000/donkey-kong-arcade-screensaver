using UnityEngine;

namespace CleanRoomArcade.Rendering
{
    public sealed class CameraShakeController : MonoBehaviour
    {
        private Transform target;
        private Vector3 origin;
        private float remaining;
        private float duration;
        private float magnitude;
        private float sampleClock;
        private int seed;
        public float GlobalMultiplier { get; set; } = 1f;

        public void Initialize(Transform cameraTransform)
        {
            target = cameraTransform;
            origin = target.localPosition;
        }

        public void Impulse(float intensityPixels, float impulseDuration)
        {
            if (target == null || impulseDuration <= 0f) return;
            magnitude = Mathf.Max(magnitude, intensityPixels);
            duration = remaining = Mathf.Max(remaining, impulseDuration);
            seed++;
        }

        private void LateUpdate() => Advance(Time.unscaledDeltaTime);

        public void Advance(float deltaTime)
        {
            if (target == null) return;
            if (remaining <= 0f)
            {
                target.localPosition = origin;
                duration = 0f;
                magnitude = 0f;
                return;
            }
            sampleClock += Mathf.Max(0f, deltaTime);
            remaining = Mathf.Max(0f, remaining - Mathf.Max(0f, deltaTime));
            var damping = duration <= 0f ? 0f : remaining / duration;
            var strength = magnitude * GlobalMultiplier * damping;
            var tick = Mathf.FloorToInt(sampleClock * 60f) + seed * 101;
            var x = Mathf.Round(Mathf.Sin(tick * 12.9898f) * strength);
            var y = Mathf.Round(Mathf.Cos(tick * 78.233f) * strength);
            target.localPosition = origin + new Vector3(x, y, 0f);
            if (remaining <= 0f)
            {
                target.localPosition = origin;
                duration = 0f;
                magnitude = 0f;
            }
        }

        public void ResetShake()
        {
            remaining = 0f;
            duration = 0f;
            magnitude = 0f;
            if (target != null) target.localPosition = origin;
        }
    }
}
