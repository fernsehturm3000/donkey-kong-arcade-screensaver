using UnityEngine;

namespace CleanRoomArcade.Core
{
    public sealed class DifficultyController
    {
        public int Loop { get; private set; } = 1;
        public float SpeedMultiplier => Mathf.Min(1.75f, 1f + (Loop - 1) * 0.08f);
        public float SpawnIntervalMultiplier => Mathf.Max(0.55f, 1f - (Loop - 1) * 0.05f);
        public void AdvanceLoop() => Loop++;
    }
}
