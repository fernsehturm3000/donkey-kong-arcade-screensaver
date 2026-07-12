using System.Collections;
using System.Collections.Generic;
using CleanRoomArcade.Gameplay;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.UI;
using UnityEngine;

namespace CleanRoomArcade.Stages
{
    public sealed class BarrelsStage : StageBase
    {
        private readonly List<RollingHazard> hazards = new List<RollingHazard>();
        private float spawnClock;
        private float stompClock;
        private int spawnCount;
        public override string DisplayName => "Barrel Works";

        protected override void BuildStage()
        {
            ArcadeHud.Label(transform, "Stage Name", "BARREL WORKS", new Vector2(0, 115), 9).color = PixelPalette.Red;
            ArcadeHud.Label(transform, "Shift", $"SHIFT {Difficulty.Loop}", new Vector2(-102, 104), 6, TextAnchor.MiddleLeft).color = PixelPalette.Cyan;
            for (var row = 0; row < 5; row++)
            {
                var y = -96 + row * 42;
                var girder = PixelSpriteFactory.Block($"Sloped Girder {row}", transform, new Vector2(0, y), new Vector2(200, 5), PixelPalette.Red);
                girder.transform.localRotation = Quaternion.Euler(0, 0, row % 2 == 0 ? -2.5f : 2.5f);
                if (row < 4)
                {
                    var ladderX = row % 2 == 0 ? 72 : -72;
                    PixelSpriteFactory.Block($"Ladder Left {row}", transform, new Vector2(ladderX - 4, y + 21), new Vector2(2, 38), PixelPalette.Cyan);
                    PixelSpriteFactory.Block($"Ladder Right {row}", transform, new Vector2(ladderX + 4, y + 21), new Vector2(2, 38), PixelPalette.Cyan);
                    for (var rung = 0; rung < 5; rung++) PixelSpriteFactory.Block("Ladder Rung", transform, new Vector2(ladderX, y + 7 + rung * 7), new Vector2(9, 1), PixelPalette.Cyan);
                }
            }
            CreateAntagonist();
            CreateCaptive();
            PixelSpriteFactory.Block("Fire Base", transform, new Vector2(-16, -88), new Vector2(11, 8), PixelPalette.Orange, 5);
            PixelSpriteFactory.Block("Fire Tip", transform, new Vector2(-16, -81), new Vector2(5, 7), PixelPalette.Yellow, 5);
            Player = CreatePlayer(new Vector2(-92, -87));
            Player.Initialize(new ScriptedRoute(
                new RoutePoint(new Vector2(-92, -87), .1f), new RoutePoint(new Vector2(72, -87), 2.2f, 1),
                new RoutePoint(new Vector2(72, -48), .8f, 2), new RoutePoint(new Vector2(-72, -45), 2.1f, 1),
                new RoutePoint(new Vector2(-72, -6), .8f, 2), new RoutePoint(new Vector2(72, -3), 2.1f, 1),
                new RoutePoint(new Vector2(72, 36), .8f, 2), new RoutePoint(new Vector2(-72, 39), 2.1f, 1),
                new RoutePoint(new Vector2(-72, 78), .8f, 3), new RoutePoint(new Vector2(62, 81), 2f, 4)
            ), Difficulty.SpeedMultiplier, OnRouteCue);
        }

        protected override void Tick(float deltaTime)
        {
            Player.Step(deltaTime);
            spawnClock -= deltaTime;
            if (spawnClock <= 0f)
            {
                SpawnBarrel();
                spawnClock = 2.2f * Difficulty.SpawnIntervalMultiplier;
            }
            stompClock -= deltaTime;
            if (stompClock <= 0f)
            {
                Shake.Impulse(1.5f, .18f);
                stompClock = 4.5f;
            }
            for (var index = hazards.Count - 1; index >= 0; index--)
            {
                var hazard = hazards[index];
                if (hazard == null) { hazards.RemoveAt(index); continue; }
                hazard.Step(deltaTime * Difficulty.SpeedMultiplier);
                if (hazard.transform.localPosition.x < -112 || hazard.transform.localPosition.y < -125)
                {
                    Shake.Impulse(1.2f, .12f);
                    Destroy(hazard.gameObject);
                    hazards.RemoveAt(index);
                }
            }
            if (Player.IsComplete) Complete = true;
        }

        private void SpawnBarrel()
        {
            spawnCount++;
            var descending = spawnCount % 4 == 0;
            var item = PixelSpriteFactory.Block(descending ? "Falling Barrel" : "Rolling Barrel", transform,
                descending ? new Vector2(72, 78) : new Vector2(86, 84), new Vector2(8, 8), PixelPalette.Orange, 10);
            var hazard = item.AddComponent<RollingHazard>();
            hazard.Initialize(descending ? new Vector2(0, -38) : new Vector2(-35, -1.5f));
            hazards.Add(hazard);
        }

        private void CreateAntagonist()
        {
            var body = PixelSpriteFactory.Block("Foreman Body", transform, new Vector2(-74, 91), new Vector2(25, 19), PixelPalette.Brown, 8);
            PixelSpriteFactory.Block("Foreman Face", body.transform, new Vector2(0, 2), new Vector2(13, 7), PixelPalette.Cream, 9);
            PixelSpriteFactory.Block("Foreman Arm", transform, new Vector2(-91, 91), new Vector2(9, 18), PixelPalette.Brown, 8);
        }

        private void CreateCaptive()
        {
            var body = PixelSpriteFactory.Block("Waiting Climber Body", transform, new Vector2(18, 91), new Vector2(9, 12), PixelPalette.Pink, 8);
            PixelSpriteFactory.Block("Waiting Climber Head", body.transform, new Vector2(0, 8), new Vector2(7, 7), PixelPalette.Cream, 9);
        }

        private void OnRouteCue(int cue)
        {
            if (cue == 1) Shake.Impulse(.7f, .08f);
            if (cue == 2) Shake.Impulse(1f, .1f);
            if (cue >= 3) Shake.Impulse(1.5f, .16f);
        }

        protected override IEnumerator FinishStage()
        {
            ArcadeHud.Label(transform, "Complete", "TOP REACHED", new Vector2(0, 16), 11).color = PixelPalette.Yellow;
            Shake.Impulse(2.5f, .3f);
            yield return new WaitForSecondsRealtime(Settings.shortStageMode ? .2f : 1f);
        }
    }
}
