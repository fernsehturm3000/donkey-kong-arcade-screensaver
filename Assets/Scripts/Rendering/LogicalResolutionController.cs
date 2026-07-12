using UnityEngine;
using UnityEngine.UI;

namespace CleanRoomArcade.Rendering
{
    public sealed class LogicalResolutionController : MonoBehaviour
    {
        public const int LogicalWidth = 224;
        public const int LogicalHeight = 256;
        private RectTransform display;
        private RenderTexture renderTexture;
        private int previousWidth;
        private int previousHeight;

        public Camera WorldCamera { get; private set; }
        public RectTransform OverlayRoot { get; private set; }

        public void Initialize()
        {
            renderTexture = new RenderTexture(LogicalWidth, LogicalHeight, 16, RenderTextureFormat.ARGB32)
            {
                name = "224x256 Logical Frame",
                filterMode = FilterMode.Point,
                useMipMap = false,
                autoGenerateMips = false
            };
            renderTexture.Create();

            var cameraObject = new GameObject("Logical Camera");
            cameraObject.transform.SetParent(transform, false);
            WorldCamera = cameraObject.AddComponent<Camera>();
            WorldCamera.orthographic = true;
            WorldCamera.orthographicSize = LogicalHeight * .5f;
            WorldCamera.aspect = (float)LogicalWidth / LogicalHeight;
            WorldCamera.clearFlags = CameraClearFlags.SolidColor;
            WorldCamera.backgroundColor = PixelPalette.Black;
            WorldCamera.targetTexture = renderTexture;
            WorldCamera.nearClipPlane = -10f;
            WorldCamera.farClipPlane = 10f;

            var canvasObject = new GameObject("Integer Scaled Display");
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>().enabled = false;
            var background = canvasObject.AddComponent<Image>();
            background.color = Color.black;

            var frame = new GameObject("Logical Frame");
            frame.transform.SetParent(canvasObject.transform, false);
            display = frame.AddComponent<RectTransform>();
            display.anchorMin = display.anchorMax = new Vector2(.5f, .5f);
            display.pivot = new Vector2(.5f, .5f);
            var image = frame.AddComponent<RawImage>();
            image.texture = renderTexture;
            image.raycastTarget = false;

            var overlay = new GameObject("Presentation Overlay");
            overlay.transform.SetParent(frame.transform, false);
            OverlayRoot = overlay.AddComponent<RectTransform>();
            OverlayRoot.anchorMin = Vector2.zero;
            OverlayRoot.anchorMax = Vector2.one;
            OverlayRoot.offsetMin = OverlayRoot.offsetMax = Vector2.zero;
            Resize();
        }

        private void Update()
        {
            if (Screen.width != previousWidth || Screen.height != previousHeight) Resize();
        }

        private void Resize()
        {
            if (display == null) return;
            previousWidth = Screen.width;
            previousHeight = Screen.height;
            var integerScale = Mathf.FloorToInt(Mathf.Min((float)Screen.width / LogicalWidth, (float)Screen.height / LogicalHeight));
            var scale = integerScale >= 1 ? integerScale : Mathf.Min((float)Screen.width / LogicalWidth, (float)Screen.height / LogicalHeight);
            display.sizeDelta = new Vector2(LogicalWidth * scale, LogicalHeight * scale);
            display.anchoredPosition = Vector2.zero;
        }

        private void OnDestroy()
        {
            if (renderTexture == null) return;
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}
