using System;
using UnityEngine;

namespace CleanRoomArcade.Gameplay
{
    [Serializable]
    public readonly struct RoutePoint
    {
        public RoutePoint(Vector2 position, float travelSeconds, int cue = 0)
        {
            Position = position;
            TravelSeconds = Mathf.Max(.05f, travelSeconds);
            Cue = cue;
        }
        public Vector2 Position { get; }
        public float TravelSeconds { get; }
        public int Cue { get; }
    }

    public sealed class ScriptedRoute
    {
        public ScriptedRoute(params RoutePoint[] points)
        {
            if (points == null || points.Length < 2) throw new ArgumentException("A route needs at least two points.", nameof(points));
            Points = points;
            TotalDuration = 0f;
            for (var index = 1; index < points.Length; index++) TotalDuration += points[index].TravelSeconds;
        }
        public RoutePoint[] Points { get; }
        public float TotalDuration { get; }
    }
}
