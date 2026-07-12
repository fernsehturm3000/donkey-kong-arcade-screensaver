#if UNITY_INCLUDE_TESTS
using System;
using System.Collections;
using CleanRoomArcade.Core;
using CleanRoomArcade.Data;
using CleanRoomArcade.Gameplay;
using CleanRoomArcade.Rendering;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
        public void StageOrderIsStable()
        {
            CollectionAssert.AreEqual(new[] { "Barrel Works", "Mixer Line", "Lift Junction", "Fastener Deck" }, StageSequenceController.StageOrder);
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

        [UnityTest]
        public IEnumerator ShakeReturnsExactlyToOrigin()
        {
            var host = new GameObject("Shake Test");
            var camera = new GameObject("Target");
            camera.transform.SetParent(host.transform, false);
            var shake = host.AddComponent<CameraShakeController>();
            shake.Initialize(camera.transform);
            shake.Impulse(3f, .03f);
            yield return new WaitForSecondsRealtime(.06f);
            Assert.That(camera.transform.localPosition, Is.EqualTo(Vector3.zero));
            UnityEngine.Object.DestroyImmediate(host);
        }
    }
}
#endif
