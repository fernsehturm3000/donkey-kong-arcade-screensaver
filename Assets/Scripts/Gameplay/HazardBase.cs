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
        private float phase;
        public override void Step(float deltaTime)
        {
            base.Step(deltaTime);
            phase += deltaTime * 8f;
            transform.localRotation = Quaternion.Euler(0, 0, phase * Mathf.Rad2Deg);
        }
    }
}
