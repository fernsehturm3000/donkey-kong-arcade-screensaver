using System.Collections;
using CleanRoomArcade.Gameplay;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.UI;
using UnityEngine;

namespace CleanRoomArcade.Stages
{
    public sealed class ElevatorsStage : StageBase
    {
        private readonly Transform[] lifts = new Transform[4];
        private readonly float[] previousY = new float[4];
        private readonly Transform[] springs = new Transform[3];
        private float clock;
        public override string DisplayName => "Lift Junction";

        protected override void BuildStage()
        {
            ArcadeHud.Label(transform, "Stage Name", "LIFT JUNCTION", new Vector2(0, 115), 9).color = PixelPalette.Cyan;
            PixelSpriteFactory.Block("Lower Deck", transform, new Vector2(0, -98), new Vector2(205, 6), PixelPalette.Red);
            PixelSpriteFactory.Block("Upper Deck", transform, new Vector2(0, 88), new Vector2(205, 6), PixelPalette.Red);
            for (var index = 0; index < lifts.Length; index++)
            {
                var x = -72 + index * 48;
                lifts[index] = PixelSpriteFactory.Block($"Elevator {index}", transform, new Vector2(x, -65 + index * 28), new Vector2(34, 5), PixelPalette.Steel, 5).transform;
                PixelSpriteFactory.Block("Lift Cable", transform, new Vector2(x, 0), new Vector2(2, 178), PixelPalette.Blue);
                previousY[index] = lifts[index].localPosition.y;
            }
            for (var index = 0; index < springs.Length; index++)
                springs[index] = PixelSpriteFactory.Block($"Spring {index}", transform, new Vector2(-54 + index * 54, -88), new Vector2(8, 12), PixelPalette.Yellow, 8).transform;
            Player = CreatePlayer(new Vector2(-100, -87));
            Player.Initialize(new ScriptedRoute(
                new RoutePoint(new Vector2(-100, -87), .1f), new RoutePoint(new Vector2(-72, -67), .8f, 1),
                new RoutePoint(new Vector2(-72, -12), 1.2f), new RoutePoint(new Vector2(-24, 14), 1f, 2),
                new RoutePoint(new Vector2(-24, 52), 1f), new RoutePoint(new Vector2(24, 24), 1f, 2),
                new RoutePoint(new Vector2(24, 70), 1f), new RoutePoint(new Vector2(72, 40), 1f, 2),
                new RoutePoint(new Vector2(72, 79), 1f), new RoutePoint(new Vector2(100, 98), .8f, 3)
            ), Difficulty.SpeedMultiplier, cue => Shake.Impulse(cue >= 3 ? 2f : 1f, cue >= 3 ? .22f : .1f));
        }

        protected override void Tick(float deltaTime)
        {
            clock += deltaTime * Difficulty.SpeedMultiplier;
            Player.Step(deltaTime);
            for (var index = 0; index < lifts.Length; index++)
            {
                var y = Mathf.Lerp(-70, 70, (Mathf.Sin(clock * (1.05f + index * .08f) + index * 1.7f) + 1f) * .5f);
                var directionChanged = Mathf.Sign(y - lifts[index].localPosition.y) != Mathf.Sign(lifts[index].localPosition.y - previousY[index]);
                previousY[index] = lifts[index].localPosition.y;
                lifts[index].localPosition = new Vector2(lifts[index].localPosition.x, Mathf.Round(y));
                if (directionChanged) Shake.Impulse(.55f, .08f);
            }
            for (var index = 0; index < springs.Length; index++)
            {
                var bounce = Mathf.Max(0f, Mathf.Sin(clock * 2.8f + index * 1.4f));
                springs[index].localPosition = new Vector2(springs[index].localPosition.x, -88 + Mathf.Round(bounce * 44));
            }
            if (Player.IsComplete) Complete = true;
        }

        protected override IEnumerator FinishStage()
        {
            ArcadeHud.Label(transform, "Complete", "UPPER DECK", new Vector2(0, 2), 11).color = PixelPalette.Yellow;
            Shake.Impulse(2.7f, .3f);
            yield return new WaitForSecondsRealtime(Settings.shortStageMode ? .2f : .9f);
        }
    }
}
