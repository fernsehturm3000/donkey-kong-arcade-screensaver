using System;
using CleanRoomArcade.Data;
using CleanRoomArcade.Rendering;
using UnityEngine;

namespace CleanRoomArcade.Core
{
    public sealed class RuntimeBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateRuntime()
        {
            if (FindObjectOfType<RuntimeBootstrap>() != null) return;
            var root = new GameObject("Arcade Screensaver Runtime");
            DontDestroyOnLoad(root);
            root.AddComponent<RuntimeBootstrap>().Initialize();
        }

        private void Initialize()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            var settings = SettingsStore.Load();
            var launch = ScreensaverArguments.Parse(Environment.GetCommandLineArgs());

            var resolution = gameObject.AddComponent<LogicalResolutionController>();
            resolution.Initialize();
            var shake = gameObject.AddComponent<CameraShakeController>();
            shake.Initialize(resolution.WorldCamera.transform);
            shake.GlobalMultiplier = settings.shakeIntensity / 100f;
            var crt = gameObject.AddComponent<CrtOverlayController>();
            crt.Initialize(resolution.OverlayRoot, settings.crtEnabled);

            gameObject.AddComponent<ScreensaverRuntime>().Initialize(launch);
            gameObject.AddComponent<StageSequenceController>().Initialize(settings, shake);
        }
    }
}
