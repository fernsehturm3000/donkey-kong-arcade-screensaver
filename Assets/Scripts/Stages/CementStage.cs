using System.Collections;
using System.Collections.Generic;
using CleanRoomArcade.Gameplay;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.UI;
using UnityEngine;

namespace CleanRoomArcade.Stages
{
    public sealed class CementStage : StageBase
    {
        private sealed class Tray
        {
            public Transform Transform;
            public int Row;
            public float Speed;
        }

        private readonly List<Tray> trays = new List<Tray>();
        private float spawnClock;
        private float directionClock = 4f;
        private int direction = 1;
        public override string DisplayName => "Mixer Line";

        protected override void BuildStage()
        {
            ArcadeHud.Label(transform, "Stage Name", "MIXER LINE", new Vector2(0, 115), 9).color = PixelPalette.Orange;
            for (var row = 0; row < 4; row++)
            {
                var y = -86 + row * 50;
                PixelSpriteFactory.Block($"Conveyor {row}", transform, new Vector2(0, y), new Vector2(200, 6), row % 2 == 0 ? PixelPalette.Steel : PixelPalette.Blue);
                for (var wheel = -88; wheel <= 88; wheel += 16)
                    PixelSpriteFactory.Block("Conveyor Roller", transform, new Vector2(wheel, y - 5), new Vector2(6, 3), PixelPalette.Cream);
                if (row < 3)
                {
                    var ladderX = row % 2 == 0 ? 80 : -80;
                    PixelSpriteFactory.Block("Connector", transform, new Vector2(ladderX, y + 25), new Vector2(8, 45), PixelPalette.Cyan);
                }
            }
            Player = CreatePlayer(new Vector2(-96, -76));
            Player.Initialize(new ScriptedRoute(
                new RoutePoint(new Vector2(-96, -76), .1f), new RoutePoint(new Vector2(-22, -76), 1.4f, 1),
                new RoutePoint(new Vector2(-22, -76), .7f), new RoutePoint(new Vector2(80, -76), 1.6f, 2),
                new RoutePoint(new Vector2(80, -36), .8f), new RoutePoint(new Vector2(28, -26), 1f, 1),
                new RoutePoint(new Vector2(28, -26), .6f), new RoutePoint(new Vector2(-80, -26), 1.7f, 2),
                new RoutePoint(new Vector2(-80, 14), .8f), new RoutePoint(new Vector2(80, 24), 2.2f, 1),
                new RoutePoint(new Vector2(80, 64), .8f), new RoutePoint(new Vector2(-82, 74), 2.3f, 3)
            ), Difficulty.SpeedMultiplier, cue => { if (cue > 0) Shake.Impulse(cue == 2 ? 1.1f : .6f, .1f); });
        }

        protected override void Tick(float deltaTime)
        {
            Player.Step(deltaTime);
            directionClock -= deltaTime;
            if (directionClock <= 0f)
            {
                direction *= -1;
                directionClock = 4f / Difficulty.SpeedMultiplier;
                Shake.Impulse(.8f, .1f);
            }
            spawnClock -= deltaTime;
            if (spawnClock <= 0f)
            {
                SpawnTray(trays.Count % 4);
                spawnClock = 1.25f * Difficulty.SpawnIntervalMultiplier;
            }
            for (var index = trays.Count - 1; index >= 0; index--)
            {
                var tray = trays[index];
                if (tray.Transform == null) { trays.RemoveAt(index); continue; }
                var rowDirection = tray.Row % 2 == 0 ? direction : -direction;
                tray.Transform.localPosition += Vector3.right * (tray.Speed * rowDirection * Difficulty.SpeedMultiplier * deltaTime);
                if (Mathf.Abs(tray.Transform.localPosition.x) > 112)
                {
                    Destroy(tray.Transform.gameObject);
                    trays.RemoveAt(index);
                }
            }
            if (Player.IsComplete) Complete = true;
        }

        private void SpawnTray(int row)
        {
            var rowDirection = row % 2 == 0 ? direction : -direction;
            var position = new Vector2(rowDirection > 0 ? -108 : 108, -78 + row * 50);
            var item = PixelSpriteFactory.Block("Moving Mixing Tray", transform, position, new Vector2(15, 6), PixelPalette.Yellow, 10);
            PixelSpriteFactory.Block("Tray Load", item.transform, new Vector2(0, 1), new Vector2(10, 5), PixelPalette.Cream, 11);
            trays.Add(new Tray { Transform = item.transform, Row = row, Speed = 28f });
        }

        protected override IEnumerator FinishStage()
        {
            ArcadeHud.Label(transform, "Complete", "LINE CLEARED", new Vector2(0, 10), 11).color = PixelPalette.Cyan;
            Shake.Impulse(1.8f, .2f);
            yield return new WaitForSecondsRealtime(Settings.shortStageMode ? .2f : .8f);
        }
    }
}
