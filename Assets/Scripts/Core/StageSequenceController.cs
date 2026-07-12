using System.Collections;
using CleanRoomArcade.Data;
using CleanRoomArcade.Rendering;
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
            StartCoroutine(FoundationLoop());
        }

        private IEnumerator FoundationLoop()
        {
            while (enabled)
            {
                var root = new GameObject("Foundation State");
                PixelSpriteFactory.Block("Test Girder", root.transform, new Vector2(0, -70), new Vector2(190, 5), PixelPalette.Red);
                PixelSpriteFactory.Block("Test Player", root.transform, new Vector2(-70, -60), new Vector2(8, 12), PixelPalette.Cyan, 2);
                yield return new WaitForSeconds(settings.shortStageMode ? 1f : 3f);
                shake.Impulse(2f, .18f);
                Destroy(root);
                yield return null;
                difficulty.AdvanceLoop();
            }
        }
    }
}
