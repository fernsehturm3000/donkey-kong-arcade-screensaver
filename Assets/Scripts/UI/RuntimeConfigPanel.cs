using CleanRoomArcade.Data;
using CleanRoomArcade.Rendering;
using UnityEngine;

namespace CleanRoomArcade.UI
{
    public sealed class RuntimeConfigPanel : MonoBehaviour
    {
        private AppSettings settings;
        private CrtOverlayController crt;
        private CameraShakeController shake;
        private string status = string.Empty;

        public void Initialize(AppSettings appSettings, CrtOverlayController crtController, CameraShakeController shakeController)
        {
            settings = appSettings;
            crt = crtController;
            shake = shakeController;
        }

        private void OnGUI()
        {
            const float width = 360f;
            const float height = 230f;
            var area = new Rect((Screen.width - width) * .5f, (Screen.height - height) * .5f, width, height);
            GUILayout.BeginArea(area, GUI.skin.window);
            GUILayout.Label("Construction Climb Screensaver");
            settings.crtEnabled = GUILayout.Toggle(settings.crtEnabled, "CRT scanlines");
            GUILayout.Label($"Shake intensity: {settings.shakeIntensity}");
            settings.shakeIntensity = Mathf.RoundToInt(GUILayout.HorizontalSlider(settings.shakeIntensity, 0, 100));
            settings.shortStageMode = GUILayout.Toggle(settings.shortStageMode, "Short stages (developer test mode)");
            crt.SetEnabled(settings.crtEnabled);
            shake.GlobalMultiplier = settings.shakeIntensity / 100f;
            GUILayout.Space(10);
            if (GUILayout.Button("Save"))
                status = SettingsStore.TrySave(settings, out var error) ? "Saved." : error;
            if (GUILayout.Button("Close")) Application.Quit();
            GUILayout.Label(status);
            GUILayout.EndArea();
        }
    }
}
