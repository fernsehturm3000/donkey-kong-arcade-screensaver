using System.Collections;
using CleanRoomArcade.Data;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.Intermissions;
using CleanRoomArcade.Stages;
using UnityEngine;

namespace CleanRoomArcade.Core
{
    public sealed class StageSequenceController : MonoBehaviour
    {
        private AppSettings settings;
        private CameraShakeController shake;
        private readonly DifficultyController difficulty = new DifficultyController();

        public void Initialize(AppSettings appSettings, CameraShakeController shakeController)
        {
            settings = appSettings;
            shake = shakeController;
            StartCoroutine(VerticalSliceLoop());
        }

        private IEnumerator VerticalSliceLoop()
        {
            while (enabled)
            {
                var intermissionObject = new GameObject("Intermission - Barrel Works");
                var intermission = intermissionObject.AddComponent<HeightIntermission>();
                intermission.Initialize(settings, difficulty, shake);
                yield return intermission.Execute("Barrel Works", 1);
                Destroy(intermissionObject);
                yield return null;

                var stageObject = new GameObject("Stage - Barrel Works");
                var stage = stageObject.AddComponent<BarrelsStage>();
                stage.Initialize(settings, difficulty, shake);
                yield return stage.Execute();
                Destroy(stageObject);
                yield return null;
                difficulty.AdvanceLoop();
            }
        }
    }
}
