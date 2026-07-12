using System.Collections;
using System.Collections.Generic;
using CleanRoomArcade.Gameplay;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.UI;
using UnityEngine;

namespace CleanRoomArcade.Stages
{
    public sealed class RivetsStage : StageBase
    {
        private readonly List<Transform> rivets = new List<Transform>();
        private readonly List<Transform> patrols = new List<Transform>();
        private float patrolClock;
        public override string DisplayName => "Fastener Deck";

        protected override void BuildStage()
        {
            ArcadeHud.Label(transform, "Stage Name", "FASTENER DECK", new Vector2(0, 115), 9).color = PixelPalette.Pink;
            for (var row = 0; row < 4; row++)
            {
                var y = -82 + row * 48;
                PixelSpriteFactory.Block($"Deck {row}", transform, new Vector2(0, y), new Vector2(200, 5), PixelPalette.Red);
                for (var column = 0; column < 2; column++)
                {
                    var x = column == 0 ? -72 : 72;
                    var rivet = PixelSpriteFactory.Block("Removable Fastener", transform, new Vector2(x, y + 6), new Vector2(7, 8), PixelPalette.Yellow, 6);
                    rivets.Add(rivet.transform);
                }
                var patrol = PixelSpriteFactory.Block("Deck Patrol", transform, new Vector2(0, y + 10), new Vector2(9, 10), row % 2 == 0 ? PixelPalette.Orange : PixelPalette.Cyan, 9);
                patrols.Add(patrol.transform);
            }
            Player = CreatePlayer(new Vector2(-98, -72));
            Player.Initialize(new ScriptedRoute(
                new RoutePoint(new Vector2(-98, -72), .1f), new RoutePoint(new Vector2(-72, -72), .5f), new RoutePoint(new Vector2(72, -72), 1.7f, 1),
                new RoutePoint(new Vector2(94, -72), .4f), new RoutePoint(new Vector2(72, -24), .8f), new RoutePoint(new Vector2(-72, -24), 1.7f, 1),
                new RoutePoint(new Vector2(-94, -24), .4f), new RoutePoint(new Vector2(-72, 24), .8f), new RoutePoint(new Vector2(72, 24), 1.7f, 1),
                new RoutePoint(new Vector2(94, 24), .4f), new RoutePoint(new Vector2(72, 72), .8f), new RoutePoint(new Vector2(-72, 72), 1.7f, 3)
            ), Difficulty.SpeedMultiplier);
        }

        protected override void Tick(float deltaTime)
        {
            patrolClock += deltaTime * Difficulty.SpeedMultiplier;
            Player.Step(deltaTime);
            for (var index = patrols.Count - 1; index >= 0; index--)
            {
                var patrol = patrols[index];
                var direction = index % 2 == 0 ? 1f : -1f;
                patrol.localPosition = new Vector2(Mathf.Sin(patrolClock * 1.2f + index) * 83f * direction, patrol.localPosition.y);
            }
            for (var index = rivets.Count - 1; index >= 0; index--)
            {
                var rivet = rivets[index];
                if (rivet == null) { rivets.RemoveAt(index); continue; }
                if (Vector2.Distance(Player.transform.localPosition, rivet.localPosition) < 13f)
                {
                    Destroy(rivet.gameObject);
                    rivets.RemoveAt(index);
                    Shake.Impulse(1.15f, .12f);
                }
            }
            if (rivets.Count == 0 && Player.IsComplete) Complete = true;
        }

        protected override void RecoverRoute()
        {
            base.RecoverRoute();
            // Removed fasteners stay removed during route recovery; a second pass finishes the remainder.
        }

        protected override IEnumerator FinishStage()
        {
            ArcadeHud.Label(transform, "Complete", "DECK RELEASED", new Vector2(0, 5), 11).color = PixelPalette.Yellow;
            var decks = new List<Transform>();
            for (var index = 0; index < transform.childCount; index++)
            {
                var child = transform.GetChild(index);
                if (child.name.StartsWith("Deck ")) decks.Add(child);
            }
            var duration = Settings.shortStageMode ? .35f : 1.3f;
            var elapsed = 0f;
            Shake.Impulse(4f, .65f);
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                for (var index = 0; index < decks.Count; index++)
                    decks[index].localPosition += new Vector3((index % 2 == 0 ? -1 : 1) * 7f * Time.unscaledDeltaTime, -30f * Time.unscaledDeltaTime, 0);
                yield return null;
            }
        }
    }
}
