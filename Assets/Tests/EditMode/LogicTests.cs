#if UNITY_INCLUDE_TESTS
using System;
using CleanRoomArcade.Core;
using CleanRoomArcade.Data;
using CleanRoomArcade.Gameplay;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.Stages;
using CleanRoomArcade.UI;
using NUnit.Framework;
using UnityEngine;

namespace CleanRoomArcade.Tests
{
    public sealed class LogicTests
    {
        [TestCase("/s", ScreensaverMode.Fullscreen)]
        [TestCase("/S", ScreensaverMode.Fullscreen)]
        [TestCase("/c", ScreensaverMode.Configuration)]
        [TestCase("/p:12345", ScreensaverMode.Preview)]
        public void ArgumentsParseCommonForms(string argument, ScreensaverMode expected)
        {
            Assert.That(ScreensaverArguments.Parse(new[] { argument }).Mode, Is.EqualTo(expected));
        }

        [Test]
        public void PreviewHandleAcceptsSeparateAndHexForms()
        {
            Assert.That(ScreensaverArguments.Parse(new[] { "/p", "42" }).PreviewHandle, Is.EqualTo(new IntPtr(42)));
            Assert.That(ScreensaverArguments.Parse(new[] { "/p:0x2A" }).PreviewHandle, Is.EqualTo(new IntPtr(42)));
        }

        [Test]
        public void RuntimeSequenceContainsOnlyBarrels()
        {
            CollectionAssert.AreEqual(new[] { "Barrel Works" }, StageSequenceController.StageOrder);
            Assert.That(StageSequenceController.ActiveStageType, Is.EqualTo(typeof(BarrelsStage)));
            Assert.That(StageSequenceController.EscalatesDifficulty, Is.False);
        }

        [Test]
        public void DifficultyEscalatesAndRemainsBounded()
        {
            var difficulty = new DifficultyController();
            for (var index = 0; index < 100; index++) difficulty.AdvanceLoop();
            Assert.That(difficulty.SpeedMultiplier, Is.EqualTo(1.75f));
            Assert.That(difficulty.SpawnIntervalMultiplier, Is.EqualTo(.55f));
        }

        [Test]
        public void MalformedSettingsRecoverToSafeDefaults()
        {
            var settings = SettingsStore.DeserializeOrDefault("{ definitely not json");
            Assert.That(settings.crtEnabled, Is.True);
            Assert.That(settings.shakeIntensity, Is.EqualTo(70));
        }

        [Test]
        public void RouteCanRestartAfterCompletion()
        {
            var gameObject = new GameObject("Route Test");
            try
            {
                var player = gameObject.AddComponent<AutoplayerController>();
                player.Initialize(new ScriptedRoute(new RoutePoint(Vector2.zero, .1f), new RoutePoint(Vector2.right, .1f)), 1f);
                player.Step(1f);
                Assert.That(player.IsComplete, Is.True);
                player.Restart();
                Assert.That(player.IsComplete, Is.False);
                Assert.That(gameObject.transform.localPosition, Is.EqualTo(Vector3.zero));
            }
            finally { UnityEngine.Object.DestroyImmediate(gameObject); }
        }

        [Test]
        public void RollingHazardTraversesEveryGirderSegmentAndCompletes()
        {
            var gameObject = new GameObject("Rolling Hazard Test");
            try
            {
                var hazard = gameObject.AddComponent<RollingHazard>();
                hazard.Initialize(new[] { Vector2.zero, Vector2.right, Vector2.one }, 10f);
                hazard.Step(1f);
                Assert.That(hazard.IsComplete, Is.True);
                Assert.That((Vector2)gameObject.transform.localPosition, Is.EqualTo(Vector2.one));
            }
            finally { UnityEngine.Object.DestroyImmediate(gameObject); }
        }

        [Test]
        public void ArcadeHudBuildsCompactReusablePixelText()
        {
            var host = new GameObject("Pixel Text Test");
            try
            {
                var label = ArcadeHud.Label(host.transform, "Label", "BARREL WORKS", Vector2.zero, 6);
                var renderer = label.GetComponent<SpriteRenderer>();
                Assert.That(renderer.sprite.rect.width, Is.LessThan(64f));
                Assert.That(renderer.sprite.rect.height, Is.EqualTo(5f));
                var originalSprite = renderer.sprite;
                label.text = "BONUS 5000!!";
                Assert.That(renderer.sprite, Is.SameAs(originalSprite));
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        [Test]
        public void ShakeReturnsExactlyToOrigin()
        {
            var host = new GameObject("Shake Test");
            var camera = new GameObject("Target");
            camera.transform.SetParent(host.transform, false);
            var shake = host.AddComponent<CameraShakeController>();
            shake.Initialize(camera.transform);
            shake.Impulse(3f, .03f);
            shake.Advance(.06f);
            Assert.That(camera.transform.localPosition, Is.EqualTo(Vector3.zero));
            UnityEngine.Object.DestroyImmediate(host);
        }
    }
}
#endif
