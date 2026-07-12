using System.Collections;
using CleanRoomArcade.Data;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.Intermissions;
using CleanRoomArcade.Stages;
using CleanRoomArcade.UI;
using UnityEngine;

namespace CleanRoomArcade.Core
{
    public sealed class StageSequenceController : MonoBehaviour
    {
        public static readonly string[] StageOrder = { "Barrel Works", "Mixer Line", "Lift Junction", "Fastener Deck" };
        private AppSettings settings;
        private CameraShakeController shake;
        private readonly DifficultyController difficulty = new DifficultyController();

        public void Initialize(AppSettings appSettings, CameraShakeController shakeController)
        {
            settings = appSettings;
            shake = shakeController;
            StartCoroutine(CompleteLoop());
        }

        private IEnumerator CompleteLoop()
        {
            while (enabled)
            {
                yield return RunIntermission("Barrel Works", 1);
                yield return RunStage<BarrelsStage>();
                yield return RunIntermission("Mixer Line", 2);
                yield return RunStage<CementStage>();
                yield return RunIntermission("Lift Junction", 3);
                yield return RunStage<ElevatorsStage>();
                yield return RunIntermission("Fastener Deck", 4);
                yield return RunStage<RivetsStage>();
                yield return RunVictory();
                difficulty.AdvanceLoop();
            }
        }

        private IEnumerator RunIntermission(string nextStage, int stageNumber)
        {
            var stateObject = new GameObject($"Intermission - {nextStage}");
            var state = stateObject.AddComponent<HeightIntermission>();
            state.Initialize(settings, difficulty, shake);
            yield return state.Execute(nextStage, stageNumber);
            Destroy(stateObject);
            yield return null;
        }

        private IEnumerator RunStage<T>() where T : StageBase
        {
            var stageObject = new GameObject("Stage");
            var stage = stageObject.AddComponent<T>();
            stageObject.name = $"Stage - {stage.DisplayName}";
            stage.Initialize(settings, difficulty, shake);
            yield return stage.Execute();
            Destroy(stageObject);
            yield return null;
        }

        private IEnumerator RunVictory()
        {
            var root = new GameObject("Shift Complete Transition");
            ArcadeHud.Label(root.transform, "Victory", "SHIFT COMPLETE", new Vector2(0, 18), 13).color = PixelPalette.Yellow;
            ArcadeHud.Label(root.transform, "Next", $"NEXT SHIFT  {difficulty.Loop + 1}", new Vector2(0, -10), 8).color = PixelPalette.Cyan;
            shake.Impulse(3f, .4f);
            yield return new WaitForSecondsRealtime(settings.shortStageMode ? .35f : 1.6f);
            Destroy(root);
            yield return null;
        }
    }
}
