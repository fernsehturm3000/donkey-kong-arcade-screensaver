#if UNITY_INCLUDE_TESTS
using System.Collections;
using CleanRoomArcade.Core;
using CleanRoomArcade.Data;
using CleanRoomArcade.Rendering;
using CleanRoomArcade.Stages;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CleanRoomArcade.Tests
{
    public sealed class SingleStageLoopTests
    {
        [UnityTest]
        public IEnumerator ReinitializingSequenceReplacesItsStageWithoutDuplicates()
        {
            var host = new GameObject("Sequence Test");
            var camera = new GameObject("Shake Target");
            camera.transform.SetParent(host.transform, false);
            var shake = host.AddComponent<CameraShakeController>();
            shake.Initialize(camera.transform);
            var sequence = host.AddComponent<StageSequenceController>();
            var settings = new AppSettings { shortStageMode = true };

            sequence.Initialize(settings, shake);
            yield return null;
            var firstStage = host.GetComponentInChildren<BarrelsStage>();
            Assert.That(firstStage, Is.Not.Null);
            Assert.That(host.GetComponentsInChildren<BarrelsStage>(), Has.Length.EqualTo(1));

            sequence.Initialize(settings, shake);
            yield return null;
            var replacementStage = host.GetComponentInChildren<BarrelsStage>();
            Assert.That(firstStage == null, Is.True);
            Assert.That(replacementStage, Is.Not.Null);
            Assert.That(host.GetComponentsInChildren<BarrelsStage>(), Has.Length.EqualTo(1));

            Object.Destroy(host);
            yield return null;
            Assert.That(replacementStage == null, Is.True);
        }
    }
}
#endif
