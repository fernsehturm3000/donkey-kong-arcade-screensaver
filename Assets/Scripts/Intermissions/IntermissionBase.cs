using System.Collections;
using CleanRoomArcade.Core;
using CleanRoomArcade.Data;
using CleanRoomArcade.Rendering;
using UnityEngine;

namespace CleanRoomArcade.Intermissions
{
    public abstract class IntermissionBase : MonoBehaviour
    {
        protected AppSettings Settings { get; private set; }
        protected DifficultyController Difficulty { get; private set; }
        protected CameraShakeController Shake { get; private set; }

        public void Initialize(AppSettings settings, DifficultyController difficulty, CameraShakeController shake)
        {
            Settings = settings;
            Difficulty = difficulty;
            Shake = shake;
        }

        public abstract IEnumerator Execute(string nextStage, int stageNumber);
    }
}
