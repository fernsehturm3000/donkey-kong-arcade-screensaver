using System.Collections;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.UI;
using UnityEngine;

namespace CleanRoomArcade.Intermissions
{
    public sealed class HeightIntermission : IntermissionBase
    {
        public override IEnumerator Execute(string nextStage, int stageNumber)
        {
            ArcadeHud.Label(transform, "Progress Title", "CLIMB REPORT", new Vector2(0, 82), 12).color = PixelPalette.Yellow;
            ArcadeHud.Label(transform, "Stage", $"ZONE {stageNumber}  {nextStage.ToUpperInvariant()}", new Vector2(0, 58), 8).color = PixelPalette.Cyan;
            ArcadeHud.Label(transform, "Loop", $"SHIFT {Difficulty.Loop}", new Vector2(0, 42), 8).color = PixelPalette.Pink;
            for (var row = 0; row < 5; row++)
                PixelSpriteFactory.Block("Progress Rail", transform, new Vector2(0, -70 + row * 24), new Vector2(100 - row * 10, 3), PixelPalette.Red);
            var climber = PixelSpriteFactory.Block("Climber", transform, new Vector2(-42, -61), new Vector2(7, 10), PixelPalette.Cream, 5);
            var duration = Settings.shortStageMode ? 1.1f : 3f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                climber.transform.localPosition = new Vector2(Mathf.Lerp(-42, 25, t), Mathf.Lerp(-61, 32, t));
                yield return null;
            }
            Shake.Impulse(1f, .12f);
            yield return new WaitForSecondsRealtime(Settings.shortStageMode ? .15f : .5f);
        }
    }
}
