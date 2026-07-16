using System.Collections;
using CleanRoomArcade.Data;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.Stages;
using UnityEngine;

namespace CleanRoomArcade.Core
{
    public sealed class StageSequenceController : MonoBehaviour
    {
        public const string ActiveStageName = "Barrel Works";
        public static readonly string[] StageOrder = { ActiveStageName };
        public static readonly System.Type ActiveStageType = typeof(BarrelsStage);
        public const bool EscalatesDifficulty = false;

        private AppSettings settings;
        private CameraShakeController shake;
        private readonly DifficultyController difficulty = new DifficultyController();
        private Coroutine loopCoroutine;
        private GameObject activeStageObject;

        public void Initialize(AppSettings appSettings, CameraShakeController shakeController)
        {
            settings = appSettings;
            shake = shakeController;
            if (loopCoroutine != null) StopCoroutine(loopCoroutine);
            DestroyActiveStage();
            loopCoroutine = StartCoroutine(BarrelsLoop());
        }

        private IEnumerator BarrelsLoop()
        {
            while (enabled)
            {
                yield return RunStage<BarrelsStage>();
            }

            loopCoroutine = null;
        }

        private IEnumerator RunStage<T>() where T : StageBase
        {
            activeStageObject = new GameObject("Stage");
            activeStageObject.transform.SetParent(transform, false);
            var stage = activeStageObject.AddComponent<T>();
            activeStageObject.name = $"Stage - {stage.DisplayName}";
            stage.Initialize(settings, difficulty, shake);
            yield return stage.Execute();
            DestroyActiveStage();
            yield return null;
        }

        private void OnDisable()
        {
            if (loopCoroutine != null) StopCoroutine(loopCoroutine);
            loopCoroutine = null;
            DestroyActiveStage();
        }

        private void DestroyActiveStage()
        {
            if (activeStageObject == null) return;
            Destroy(activeStageObject);
            activeStageObject = null;
        }
    }
}
