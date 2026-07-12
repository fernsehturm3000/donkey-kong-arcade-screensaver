using UnityEngine;
using UnityEngine.UI;

namespace CleanRoomArcade.Rendering
{
    public sealed class CrtOverlayController : MonoBehaviour
    {
        private GameObject overlay;

        public void Initialize(RectTransform parent, bool enabled)
        {
            overlay = new GameObject("CRT Scanlines");
            overlay.transform.SetParent(parent, false);
            var rect = overlay.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var texture = new Texture2D(1, 2, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat
            };
            texture.SetPixels32(new[] { new Color32(0, 0, 0, 8), new Color32(0, 0, 0, 50) });
            texture.Apply(false, true);
            var image = overlay.AddComponent<RawImage>();
            image.texture = texture;
            image.uvRect = new Rect(0, 0, 1, LogicalResolutionController.LogicalHeight / 2f);
            image.raycastTarget = false;
            CreateEdge("Left Vignette", rect, new Vector2(0, 0), new Vector2(.055f, 1));
            CreateEdge("Right Vignette", rect, new Vector2(.945f, 0), new Vector2(1, 1));
            CreateEdge("Top Vignette", rect, new Vector2(0, .96f), new Vector2(1, 1));
            CreateEdge("Bottom Vignette", rect, new Vector2(0, 0), new Vector2(1, .04f));
            SetEnabled(enabled);
        }

        private static void CreateEdge(string name, RectTransform parent, Vector2 minimum, Vector2 maximum)
        {
            var edge = new GameObject(name);
            edge.transform.SetParent(parent, false);
            var rect = edge.AddComponent<RectTransform>();
            rect.anchorMin = minimum;
            rect.anchorMax = maximum;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var image = edge.AddComponent<Image>();
            image.color = new Color(0, 0, 0, .22f);
            image.raycastTarget = false;
        }

        public void SetEnabled(bool value)
        {
            if (overlay != null) overlay.SetActive(value);
        }
    }
}
