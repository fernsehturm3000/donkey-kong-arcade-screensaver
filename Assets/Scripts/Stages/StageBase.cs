using System.Collections;
using CleanRoomArcade.Core;
using CleanRoomArcade.Data;
using CleanRoomArcade.Gameplay;
using CleanRoomArcade.Rendering;
using UnityEngine;

namespace CleanRoomArcade.Stages
{
    public abstract class StageBase : MonoBehaviour
    {
        protected AppSettings Settings { get; private set; }
        protected DifficultyController Difficulty { get; private set; }
        protected CameraShakeController Shake { get; private set; }
        protected AutoplayerController Player { get; set; }
        protected bool Complete { get; set; }
        protected virtual float TimeoutSeconds => Settings.shortStageMode ? 8f : 32f;
        public abstract string DisplayName { get; }

        public void Initialize(AppSettings settings, DifficultyController difficulty, CameraShakeController shake)
        {
            Settings = settings;
            Difficulty = difficulty;
            Shake = shake;
        }

        public IEnumerator Execute()
        {
            BuildStage();
            for (var attempt = 0; attempt < 2 && !Complete; attempt++)
            {
                var elapsed = 0f;
                if (attempt > 0) RecoverRoute();
                while (!Complete && elapsed < TimeoutSeconds)
                {
                    var delta = Time.unscaledDeltaTime;
                    elapsed += delta;
                    Tick(delta);
                    yield return null;
                }
            }
            Complete = true;
            yield return FinishStage();
            Shake.ResetShake();
        }

        protected abstract void BuildStage();
        protected abstract void Tick(float deltaTime);
        protected virtual void RecoverRoute() => Player?.Restart();
        protected virtual IEnumerator FinishStage() { yield return null; }

        protected AutoplayerController CreatePlayer(Vector2 position)
        {
            var playerObject = new GameObject("Scripted Player");
            playerObject.transform.SetParent(transform, false);
            playerObject.transform.localPosition = position;
            var renderer = playerObject.AddComponent<SpriteRenderer>();
            var palette = new[] { PixelPalette.Cream, PixelPalette.Red, PixelPalette.Blue, PixelPalette.DarkBlue, PixelPalette.Brown };
            var walkA = PixelSpriteFactory.FromMatrix("climber-walk-a", new[,]
            {
                { -1,-1,1,1,1,1,1,-1,-1,-1 }, { -1,1,1,1,1,1,1,1,-1,-1 },
                { -1,4,4,0,0,0,4,4,-1,-1 }, { -1,4,0,4,0,4,0,4,-1,-1 },
                { -1,-1,4,4,4,4,4,-1,-1,-1 }, { -1,1,1,2,2,2,1,1,-1,-1 },
                { 1,1,1,2,2,2,1,1,1,-1 }, { -1,0,1,2,2,2,1,0,-1,-1 },
                { -1,-1,2,2,2,2,2,-1,-1,-1 }, { -1,-1,2,2,-1,2,2,-1,-1,-1 },
                { -1,3,3,-1,-1,-1,3,3,-1,-1 }, { 3,3,3,-1,-1,-1,-1,3,3,-1 }
            }, palette);
            var walkB = PixelSpriteFactory.FromMatrix("climber-walk-b", new[,]
            {
                { -1,-1,1,1,1,1,1,-1,-1,-1 }, { -1,1,1,1,1,1,1,1,-1,-1 },
                { -1,4,4,0,0,0,4,4,-1,-1 }, { -1,4,0,4,0,4,0,4,-1,-1 },
                { -1,-1,4,4,4,4,4,-1,-1,-1 }, { -1,1,1,2,2,2,1,1,-1,-1 },
                { -1,1,1,2,2,2,1,1,-1,-1 }, { -1,-1,1,2,2,2,1,-1,-1,-1 },
                { -1,-1,2,2,2,2,2,-1,-1,-1 }, { -1,2,2,-1,-1,2,2,-1,-1,-1 },
                { 3,3,-1,-1,-1,-1,3,3,3,-1 }, { 3,-1,-1,-1,-1,-1,-1,-1,3,3 }
            }, palette);
            var climbA = PixelSpriteFactory.FromMatrix("climber-climb-a", new[,]
            {
                { -1,-1,1,1,1,1,1,-1,-1,-1 }, { -1,1,1,1,1,1,1,1,-1,-1 },
                { -1,4,4,0,0,0,4,4,-1,-1 }, { -1,-1,4,4,4,4,4,-1,-1,-1 },
                { 0,0,1,2,2,2,1,-1,-1,-1 }, { -1,-1,1,2,2,2,1,0,0,-1 },
                { -1,-1,2,2,2,2,2,-1,-1,-1 }, { -1,-1,2,2,2,2,2,-1,-1,-1 },
                { -1,3,3,-1,-1,3,3,-1,-1,-1 }, { -1,3,3,-1,-1,3,3,-1,-1,-1 },
                { -1,3,-1,-1,-1,-1,3,-1,-1,-1 }, { 3,3,-1,-1,-1,-1,3,3,-1,-1 }
            }, palette);
            var climbB = PixelSpriteFactory.FromMatrix("climber-climb-b", new[,]
            {
                { -1,-1,1,1,1,1,1,-1,-1,-1 }, { -1,1,1,1,1,1,1,1,-1,-1 },
                { -1,4,4,0,0,0,4,4,-1,-1 }, { -1,-1,4,4,4,4,4,-1,-1,-1 },
                { -1,-1,1,2,2,2,1,0,0,-1 }, { 0,0,1,2,2,2,1,-1,-1,-1 },
                { -1,-1,2,2,2,2,2,-1,-1,-1 }, { -1,-1,2,2,2,2,2,-1,-1,-1 },
                { 3,3,-1,-1,-1,-1,3,-1,-1,-1 }, { -1,3,3,-1,-1,3,3,-1,-1,-1 },
                { -1,3,-1,-1,-1,-1,3,3,-1,-1 }, { -1,3,3,-1,-1,-1,3,-1,-1,-1 }
            }, palette);
            renderer.sprite = walkA;
            renderer.sortingOrder = 20;
            var controller = playerObject.AddComponent<AutoplayerController>();
            controller.ConfigureVisual(renderer, new[] { walkA, walkB }, new[] { climbA, climbB });
            return controller;
        }
    }
}
