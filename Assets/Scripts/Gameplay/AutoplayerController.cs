using System;
using UnityEngine;

namespace CleanRoomArcade.Gameplay
{
    public sealed class AutoplayerController : MonoBehaviour
    {
        private ScriptedRoute route;
        private Action<int> cueHandler;
        private float routeTime;
        private int segment;
        private float speed = 1f;
        private SpriteRenderer spriteRenderer;
        private Sprite[] walkFrames;
        private Sprite[] climbFrames;
        private float frameClock;

        public bool IsComplete { get; private set; }

        public void Initialize(ScriptedRoute scriptedRoute, float speedMultiplier, Action<int> onCue = null)
        {
            route = scriptedRoute;
            speed = Mathf.Max(.1f, speedMultiplier);
            cueHandler = onCue;
            Restart();
        }

        public void ConfigureVisual(SpriteRenderer renderer, Sprite[] horizontalFrames, Sprite[] verticalFrames)
        {
            spriteRenderer = renderer;
            walkFrames = horizontalFrames;
            climbFrames = verticalFrames;
            if (spriteRenderer != null && walkFrames != null && walkFrames.Length > 0) spriteRenderer.sprite = walkFrames[0];
        }

        public void Restart()
        {
            routeTime = 0f;
            segment = 1;
            frameClock = 0f;
            IsComplete = false;
            if (route != null) transform.localPosition = route.Points[0].Position;
        }

        public void Step(float deltaTime)
        {
            if (route == null || IsComplete) return;
            var previousPosition = transform.localPosition;
            routeTime += deltaTime * speed;
            var elapsedBeforeSegment = 0f;
            for (var index = 1; index < route.Points.Length; index++)
            {
                var point = route.Points[index];
                var segmentEnd = elapsedBeforeSegment + point.TravelSeconds;
                if (routeTime <= segmentEnd)
                {
                    var t = Mathf.InverseLerp(elapsedBeforeSegment, segmentEnd, routeTime);
                    transform.localPosition = Vector2.Lerp(route.Points[index - 1].Position, point.Position, t);
                    if (index > segment)
                    {
                        segment = index;
                        cueHandler?.Invoke(route.Points[index - 1].Cue);
                    }
                    UpdateVisual(previousPosition, transform.localPosition, deltaTime);
                    return;
                }
                elapsedBeforeSegment = segmentEnd;
            }
            transform.localPosition = route.Points[route.Points.Length - 1].Position;
            cueHandler?.Invoke(route.Points[route.Points.Length - 1].Cue);
            IsComplete = true;
            UpdateVisual(previousPosition, transform.localPosition, deltaTime);
        }

        private void UpdateVisual(Vector3 previousPosition, Vector3 currentPosition, float deltaTime)
        {
            if (spriteRenderer == null) return;
            var motion = currentPosition - previousPosition;
            var climbing = Mathf.Abs(motion.y) > Mathf.Abs(motion.x) * .7f;
            var frames = climbing ? climbFrames : walkFrames;
            if (frames == null || frames.Length == 0) return;
            frameClock += deltaTime * speed;
            spriteRenderer.sprite = frames[Mathf.FloorToInt(frameClock / .11f) % frames.Length];
            if (!climbing && Mathf.Abs(motion.x) > .001f) spriteRenderer.flipX = motion.x < 0f;
        }
    }
}
