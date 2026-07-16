using UnityEngine;

namespace CleanRoomArcade.Gameplay
{
    public abstract class HazardBase : MonoBehaviour
    {
        protected Vector2 Velocity;
        public virtual void Initialize(Vector2 velocity) => Velocity = velocity;
        public virtual void Step(float deltaTime) => transform.localPosition += (Vector3)(Velocity * deltaTime);
    }

    public sealed class RollingHazard : HazardBase
    {
        private Vector2[] route;
        private int targetIndex;
        private float routeSpeed;
        public bool IsComplete { get; private set; }

        public void Initialize(Vector2[] points, float speed)
        {
            route = points;
            routeSpeed = Mathf.Max(1f, speed);
            targetIndex = 1;
            IsComplete = route == null || route.Length < 2;
            if (!IsComplete) transform.localPosition = route[0];
        }

        public override void Step(float deltaTime)
        {
            if (IsComplete) return;
            var remaining = routeSpeed * deltaTime;
            while (remaining > 0f && targetIndex < route.Length)
            {
                var current = (Vector2)transform.localPosition;
                var target = route[targetIndex];
                var distance = Vector2.Distance(current, target);
                if (distance <= remaining)
                {
                    transform.localPosition = target;
                    remaining -= distance;
                    targetIndex++;
                    continue;
                }

                var motion = (target - current).normalized * remaining;
                transform.localPosition = current + motion;
                if (Mathf.Abs(motion.x) > .001f)
                    transform.Rotate(0f, 0f, -Mathf.Sign(motion.x) * remaining * 22f);
                remaining = 0f;
            }

            IsComplete = targetIndex >= route.Length;
        }
    }
}
