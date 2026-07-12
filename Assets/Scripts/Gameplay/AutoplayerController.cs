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

        public bool IsComplete { get; private set; }

        public void Initialize(ScriptedRoute scriptedRoute, float speedMultiplier, Action<int> onCue = null)
        {
            route = scriptedRoute;
            speed = Mathf.Max(.1f, speedMultiplier);
            cueHandler = onCue;
            Restart();
        }

        public void Restart()
        {
            routeTime = 0f;
            segment = 1;
            IsComplete = false;
            if (route != null) transform.localPosition = route.Points[0].Position;
        }

        public void Step(float deltaTime)
        {
            if (route == null || IsComplete) return;
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
                    return;
                }
                elapsedBeforeSegment = segmentEnd;
            }
            transform.localPosition = route.Points[route.Points.Length - 1].Position;
            cueHandler?.Invoke(route.Points[route.Points.Length - 1].Cue);
            IsComplete = true;
        }
    }
}
