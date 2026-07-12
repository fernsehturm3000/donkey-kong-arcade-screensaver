using UnityEngine;

namespace CleanRoomArcade.Core
{
    public sealed class ScreensaverRuntime : MonoBehaviour
    {
        private ScreensaverMode mode;
        private Vector3 initialMousePosition;
        private float armedAt;
        private const float GraceSeconds = 1f;
        private const float MouseThresholdPixels = 8f;

        public void Initialize(ScreensaverLaunch launch)
        {
            mode = launch.Mode;
            if (mode == ScreensaverMode.Fullscreen)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.fullScreen = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (mode == ScreensaverMode.Preview)
            {
                Screen.fullScreen = false;
                Cursor.visible = true;
            }
            initialMousePosition = Input.mousePosition;
            armedAt = Time.unscaledTime + GraceSeconds;
        }

        private void Update()
        {
            if (mode != ScreensaverMode.Fullscreen || Time.unscaledTime < armedAt) return;
            if (Input.anyKeyDown || Vector3.Distance(Input.mousePosition, initialMousePosition) > MouseThresholdPixels)
                Quit();
        }

        private static void Quit()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnApplicationQuit()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
