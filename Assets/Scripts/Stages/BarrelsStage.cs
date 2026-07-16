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
        private static readonly float[] GirderHeights = { -104f, -68f, -32f, 4f, 40f, 76f };
        private static readonly float[] LadderPositions = { 80f, -80f, 72f, -72f, 64f };

        private readonly List<RollingHazard> hazards = new List<RollingHazard>();
        private SpriteRenderer fireRenderer;
        private SpriteRenderer foremanRenderer;
        private Sprite[] fireFrames;
        private Sprite[] foremanFrames;
        private TextMesh scoreLabel;
        private TextMesh bonusLabel;
        private float spawnClock = .8f;
        private float stompClock = 2.5f;
        private float animationClock;
        private int score;
        private int bonus = 5000;
        private int spawnCount;

        public override string DisplayName => "Barrel Works";

        protected override void BuildStage()
        {
            CreateHud();
            CreateConstructionFrame();
            CreateOilFire();
            CreateForeman();
            CreateWaitingClimber();
            CreateBarrelStack();

            Player = CreatePlayer(new Vector2(-100, -96));
            Player.Initialize(new ScriptedRoute(
                new RoutePoint(new Vector2(-100, -96), .1f),
                new RoutePoint(new Vector2(80, -96), 2.7f, 1),
                new RoutePoint(new Vector2(80, -60), .8f, 2),
                new RoutePoint(new Vector2(-80, -60), 2.5f, 1),
                new RoutePoint(new Vector2(-80, -24), .8f, 2),
                new RoutePoint(new Vector2(72, -24), 2.4f, 1),
                new RoutePoint(new Vector2(72, 12), .8f, 2),
                new RoutePoint(new Vector2(-72, 12), 2.3f, 1),
                new RoutePoint(new Vector2(-72, 48), .8f, 2),
                new RoutePoint(new Vector2(64, 48), 2.2f, 1),
                new RoutePoint(new Vector2(64, 84), .8f, 3),
                new RoutePoint(new Vector2(-18, 84), 1.4f, 4)
            ), Difficulty.SpeedMultiplier, OnRouteCue);
        }

        protected override void Tick(float deltaTime)
        {
            Player.Step(deltaTime);
            animationClock += deltaTime;
            UpdateCharacterAnimation();
            UpdateHud(deltaTime);

            spawnClock -= deltaTime;
            if (spawnClock <= 0f)
            {
                SpawnBarrel();
                spawnClock = 3.25f * Difficulty.SpawnIntervalMultiplier;
            }

            stompClock -= deltaTime;
            if (stompClock <= 0f)
            {
                Shake.Impulse(1.5f, .18f);
                stompClock = 4.2f;
            }

            for (var index = hazards.Count - 1; index >= 0; index--)
            {
                var hazard = hazards[index];
                if (hazard == null)
                {
                    hazards.RemoveAt(index);
                    continue;
                }

                hazard.Step(deltaTime * Difficulty.SpeedMultiplier);
                if (!hazard.IsComplete) continue;
                Shake.Impulse(1.2f, .12f);
                Destroy(hazard.gameObject);
                hazards.RemoveAt(index);
            }

            if (Player.IsComplete) Complete = true;
        }

        private void CreateHud()
        {
            PixelSpriteFactory.Block("HUD Backdrop", transform, new Vector2(0, 110), new Vector2(224, 30), PixelPalette.NearBlack, 40);
            ArcadeHud.Label(transform, "One Up", "1-UP", new Vector2(-79, 120), 7).color = PixelPalette.Red;
            ArcadeHud.Label(transform, "High", "HIGH", new Vector2(0, 120), 7).color = PixelPalette.Red;
            ArcadeHud.Label(transform, "High Score", "028450", new Vector2(0, 110), 7).color = PixelPalette.Cream;
            scoreLabel = ArcadeHud.Label(transform, "Score", "000000", new Vector2(-79, 110), 7);
            scoreLabel.color = PixelPalette.Cream;
            ArcadeHud.Label(transform, "Stage", "BARREL WORKS", new Vector2(70, 120), 6).color = PixelPalette.Cyan;
            bonusLabel = ArcadeHud.Label(transform, "Bonus", "B 5000", new Vector2(73, 109), 6);
            bonusLabel.color = PixelPalette.Yellow;
        }

        private void CreateConstructionFrame()
        {
            for (var row = 0; row < GirderHeights.Length; row++) CreateGirderRow(row, GirderHeights[row]);
            for (var row = 0; row < LadderPositions.Length; row++) CreateLadder($"Route Ladder {row}", LadderPositions[row], GirderHeights[row], GirderHeights[row + 1], false);

            CreateLadder("Broken Ladder A", -24, GirderHeights[0], GirderHeights[1] - 13, true);
            CreateLadder("Broken Ladder B", 22, GirderHeights[2], GirderHeights[3] - 11, true);
            CreateLadder("Broken Ladder C", -12, GirderHeights[4], GirderHeights[5] - 14, true);

            PixelSpriteFactory.Block("Top Platform", transform, new Vector2(-48, 88), new Vector2(92, 4), PixelPalette.Red, 3);
            for (var x = -88; x <= -8; x += 16)
                PixelSpriteFactory.Block("Top Rivet", transform, new Vector2(x, 89), Vector2.one, PixelPalette.Lavender, 4);
        }

        private void CreateGirderRow(int row, float y)
        {
            var direction = row % 2 == 0 ? -1f : 1f;
            for (var segment = 0; segment < 17; segment++)
            {
                var x = -96 + segment * 12;
                var segmentY = y + direction * (segment - 8) * .36f;
                PixelSpriteFactory.Block($"Girder {row}-{segment}", transform, new Vector2(x, segmentY), new Vector2(12, 4), PixelPalette.Red, 2);
                PixelSpriteFactory.Block($"Rivet {row}-{segment}", transform, new Vector2(x - 4, segmentY + 1), Vector2.one, PixelPalette.Lavender, 4);
                if (segment % 2 != 0) continue;
                var brace = PixelSpriteFactory.Block($"Brace {row}-{segment}", transform, new Vector2(x, segmentY - 5), new Vector2(1.5f, 9), PixelPalette.DarkRed, 1);
                brace.transform.localRotation = Quaternion.Euler(0, 0, direction > 0 ? -45 : 45);
            }
        }

        private void CreateLadder(string name, float x, float lowerY, float upperY, bool broken)
        {
            var bottom = lowerY + 3;
            var top = upperY - 3;
            var height = top - bottom;
            PixelSpriteFactory.Block(name + " Left", transform, new Vector2(x - 4, bottom + height * .5f), new Vector2(2, height), PixelPalette.Cyan, 1);
            PixelSpriteFactory.Block(name + " Right", transform, new Vector2(x + 4, bottom + height * .5f), new Vector2(2, height), PixelPalette.Cyan, 1);
            var rungCount = Mathf.FloorToInt(height / 6f);
            for (var rung = 0; rung <= rungCount; rung++)
            {
                if (broken && rung >= rungCount - 1) continue;
                PixelSpriteFactory.Block(name + " Rung", transform, new Vector2(x, bottom + rung * 6), new Vector2(9, 1.5f), PixelPalette.Cyan, 1);
            }
        }

        private void CreateOilFire()
        {
            var drum = PixelSpriteFactory.FromMatrix("oil-drum", new[,]
            {
                { -1,1,1,1,1,1,1,1,1,1,-1 }, { 1,2,2,2,2,2,2,2,2,2,1 },
                { 1,0,0,0,0,0,0,0,0,0,1 }, { 1,0,1,1,1,1,1,1,1,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,1 }, { 1,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,1,1,1,1,1,1,1,0,1 }, { 1,0,0,0,0,0,0,0,0,0,1 },
                { 1,2,2,2,2,2,2,2,2,2,1 }, { -1,1,1,1,1,1,1,1,1,1,-1 }
            }, new[] { PixelPalette.DarkBlue, PixelPalette.Cyan, PixelPalette.Blue });
            PixelSpriteFactory.SpriteObject("Oil Drum", transform, new Vector2(-50, -96), drum, 8);

            fireFrames = new[]
            {
                PixelSpriteFactory.FromMatrix("fire-a", new[,]
                {
                    { -1,-1,1,-1,-1,-1,-1 }, { -1,-1,1,1,-1,0,-1 }, { -1,0,1,1,0,0,-1 },
                    { -1,0,0,2,2,0,-1 }, { 0,0,2,2,2,0,0 }, { -1,0,0,0,0,0,-1 }
                }, new[] { PixelPalette.Orange, PixelPalette.Yellow, PixelPalette.Cream }),
                PixelSpriteFactory.FromMatrix("fire-b", new[,]
                {
                    { -1,-1,-1,1,-1,-1,-1 }, { -1,0,-1,1,1,-1,-1 }, { -1,0,0,1,1,0,-1 },
                    { -1,0,2,2,0,0,-1 }, { 0,0,2,2,2,0,0 }, { -1,0,0,0,0,0,-1 }
                }, new[] { PixelPalette.Orange, PixelPalette.Yellow, PixelPalette.Cream })
            };
            fireRenderer = PixelSpriteFactory.SpriteObject("Oil Fire", transform, new Vector2(-50, -87), fireFrames[0], 9);
        }

        private void CreateForeman()
        {
            var palette = new[] { PixelPalette.Brown, PixelPalette.DarkBrown, PixelPalette.Cream, PixelPalette.Red };
            foremanFrames = new[]
            {
                PixelSpriteFactory.FromMatrix("foreman-a", new[,]
                {
                    { -1,-1,1,1,1,1,1,1,-1,-1,-1,-1,-1,-1 },
                    { -1,1,1,0,0,0,0,1,1,-1,-1,-1,-1,-1 },
                    { 1,1,0,2,0,0,2,0,1,1,-1,-1,-1,-1 },
                    { 1,0,0,0,2,2,0,0,0,1,-1,-1,-1,-1 },
                    { -1,1,0,2,2,2,2,0,1,-1,-1,-1,-1,-1 },
                    { 0,0,0,1,1,1,1,0,0,0,-1,-1,-1,-1 },
                    { 0,0,1,1,1,1,1,1,0,0,0,-1,-1,-1 },
                    { 0,1,1,1,1,1,1,1,1,0,0,0,-1,-1 },
                    { 1,1,1,1,1,1,1,1,1,1,0,0,0,-1 },
                    { 1,1,3,3,3,3,3,3,1,1,-1,0,0,0 },
                    { -1,1,1,1,1,1,1,1,1,-1,-1,-1,0,0 },
                    { -1,1,1,-1,-1,-1,-1,1,1,-1,-1,-1,-1,0 }
                }, palette),
                PixelSpriteFactory.FromMatrix("foreman-b", new[,]
                {
                    { -1,-1,1,1,1,1,1,1,-1,-1,-1,-1,-1,-1 },
                    { -1,1,1,0,0,0,0,1,1,-1,-1,-1,-1,-1 },
                    { 1,1,0,2,0,0,2,0,1,1,-1,-1,-1,-1 },
                    { 1,0,0,0,2,2,0,0,0,1,-1,-1,-1,-1 },
                    { -1,1,0,2,2,2,2,0,1,-1,-1,-1,-1,-1 },
                    { -1,-1,0,1,1,1,1,0,0,0,-1,-1,-1,-1 },
                    { -1,0,0,1,1,1,1,1,0,0,-1,-1,-1,-1 },
                    { 0,0,1,1,1,1,1,1,1,0,-1,-1,-1,-1 },
                    { 0,1,1,1,1,1,1,1,1,1,-1,-1,-1,-1 },
                    { 1,1,3,3,3,3,3,3,1,1,0,0,0,-1 },
                    { -1,1,1,1,1,1,1,1,1,-1,0,0,-1,-1 },
                    { -1,1,1,-1,-1,-1,-1,1,1,-1,0,-1,-1,-1 }
                }, palette)
            };
            foremanRenderer = PixelSpriteFactory.SpriteObject("Construction Foreman", transform, new Vector2(-66, 98), foremanFrames[0], 12);
        }

        private void CreateWaitingClimber()
        {
            var sprite = PixelSpriteFactory.FromMatrix("waiting-climber", new[,]
            {
                { -1,-1,1,1,1,1,-1,-1 }, { -1,1,1,0,0,1,1,-1 },
                { -1,1,0,0,0,0,1,-1 }, { -1,-1,0,0,0,0,-1,-1 },
                { -1,2,2,2,2,2,2,-1 }, { -1,2,1,2,2,1,2,-1 },
                { 1,1,2,2,2,2,1,1 }, { -1,-1,2,2,2,2,-1,-1 },
                { -1,-1,2,2,2,2,-1,-1 }, { -1,-1,3,3,3,3,-1,-1 },
                { -1,3,3,-1,-1,3,3,-1 }, { -1,3,-1,-1,-1,-1,3,-1 }
            }, new[] { PixelPalette.Cream, PixelPalette.Yellow, PixelPalette.Magenta, PixelPalette.Cyan });
            PixelSpriteFactory.SpriteObject("Waiting Climber", transform, new Vector2(20, 98), sprite, 11);
        }

        private void CreateBarrelStack()
        {
            var barrel = BarrelSprite();
            PixelSpriteFactory.SpriteObject("Barrel Stack 1", transform, new Vector2(71, 85), barrel, 10);
            PixelSpriteFactory.SpriteObject("Barrel Stack 2", transform, new Vector2(82, 85), barrel, 10);
            PixelSpriteFactory.SpriteObject("Barrel Stack 3", transform, new Vector2(76, 95), barrel, 10);
        }

        private void SpawnBarrel()
        {
            spawnCount++;
            var renderer = PixelSpriteFactory.SpriteObject($"Rolling Barrel {spawnCount}", transform, Vector2.zero, BarrelSprite(), 15);
            if (spawnCount % 3 == 0) renderer.color = new Color32(255, 187, 72, 255);
            var hazard = renderer.gameObject.AddComponent<RollingHazard>();
            hazard.Initialize(new[]
            {
                new Vector2(-42, 84), new Vector2(-99, 80), new Vector2(-99, 49),
                new Vector2(99, 45), new Vector2(99, 13), new Vector2(-99, 9),
                new Vector2(-99, -23), new Vector2(99, -27), new Vector2(99, -59),
                new Vector2(-107, -63), new Vector2(-107, -96)
            }, 34f);
            hazards.Add(hazard);
        }

        private static Sprite BarrelSprite()
        {
            return PixelSpriteFactory.FromMatrix("arcade-barrel", new[,]
            {
                { -1,-1,1,1,1,1,1,1,-1,-1 }, { -1,1,2,2,0,0,2,2,1,-1 },
                { 1,2,0,0,1,1,0,0,2,1 }, { 1,2,0,1,2,2,1,0,2,1 },
                { 1,0,1,2,2,2,2,1,0,1 }, { 1,0,1,2,2,2,2,1,0,1 },
                { 1,2,0,1,2,2,1,0,2,1 }, { 1,2,0,0,1,1,0,0,2,1 },
                { -1,1,2,2,0,0,2,2,1,-1 }, { -1,-1,1,1,1,1,1,1,-1,-1 }
            }, new[] { PixelPalette.Orange, PixelPalette.DarkBrown, PixelPalette.Cream });
        }

        private void UpdateCharacterAnimation()
        {
            var frame = Mathf.FloorToInt(animationClock / .24f) % 2;
            if (fireRenderer != null) fireRenderer.sprite = fireFrames[frame];
            if (foremanRenderer != null) foremanRenderer.sprite = foremanFrames[frame];
        }

        private void UpdateHud(float deltaTime)
        {
            score += Mathf.RoundToInt(deltaTime * 35f);
            bonus = Mathf.Max(0, bonus - Mathf.RoundToInt(deltaTime * 28f));
            if (scoreLabel != null) scoreLabel.text = score.ToString("D6");
            if (bonusLabel != null) bonusLabel.text = $"B {bonus:D4}";
        }

        private void OnRouteCue(int cue)
        {
            if (cue == 1)
            {
                score += 100;
                Shake.Impulse(.7f, .08f);
            }
            if (cue == 2)
            {
                score += 250;
                Shake.Impulse(1f, .1f);
            }
            if (cue >= 3)
            {
                score += 500;
                Shake.Impulse(1.5f, .16f);
            }
        }

        protected override IEnumerator FinishStage()
        {
            PixelSpriteFactory.Block("Finish Backdrop", transform, new Vector2(0, 12), new Vector2(126, 32), PixelPalette.NearBlack, 44);
            ArcadeHud.Label(transform, "Complete", "TOP REACHED", new Vector2(0, 18), 11).color = PixelPalette.Yellow;
            ArcadeHud.Label(transform, "Bonus Award", $"BONUS {bonus:D4}", new Vector2(0, 4), 7).color = PixelPalette.Cyan;
            Shake.Impulse(2.5f, .3f);
            yield return new WaitForSecondsRealtime(Settings.shortStageMode ? .3f : 1.2f);
        }
    }
}
