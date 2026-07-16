using System.Collections.Generic;
using CleanRoomArcade.Rendering;
using UnityEngine;

namespace CleanRoomArcade.UI
{
    public static class ArcadeHud
    {
        public static PixelText Label(Transform parent, string name, string text, Vector2 position, int size = 10, TextAnchor anchor = TextAnchor.MiddleCenter)
        {
            var item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = new Vector3(position.x, position.y, -1f);
            var label = item.AddComponent<PixelText>();
            label.Initialize(text, size, anchor);
            return label;
        }
    }

    public sealed class PixelText : MonoBehaviour
    {
        private const int GlyphWidth = 3;
        private const int GlyphHeight = 5;
        private const int GlyphSpacing = 1;

        private static readonly Dictionary<char, string> Glyphs = new Dictionary<char, string>
        {
            ['A'] = "010101111101101", ['B'] = "110101110101110", ['C'] = "011100100100011",
            ['D'] = "110101101101110", ['E'] = "111100110100111", ['F'] = "111100110100100",
            ['G'] = "011100101101011", ['H'] = "101101111101101", ['I'] = "111010010010111",
            ['J'] = "001001001101010", ['K'] = "101101110101101", ['L'] = "100100100100111",
            ['M'] = "101111111101101", ['N'] = "101111111111101", ['O'] = "010101101101010",
            ['P'] = "110101110100100", ['Q'] = "010101101111011", ['R'] = "110101110101101",
            ['S'] = "011100010001110", ['T'] = "111010010010010", ['U'] = "101101101101111",
            ['V'] = "101101101101010", ['W'] = "101101111111101", ['X'] = "101101010101101",
            ['Y'] = "101101010010010", ['Z'] = "111001010100111",
            ['0'] = "111101101101111", ['1'] = "010110010010111", ['2'] = "110001010100111",
            ['3'] = "110001010001110", ['4'] = "101101111001001", ['5'] = "111100110001110",
            ['6'] = "011100110101010", ['7'] = "111001010010010", ['8'] = "010101010101010",
            ['9'] = "010101011001110", ['-'] = "000000111000000", [':'] = "000010000010000",
            ['.'] = "000000000000010", [' '] = "000000000000000"
        };

        private SpriteRenderer spriteRenderer;
        private Texture2D texture;
        private Sprite sprite;
        private int pixelScale = 1;
        private TextAnchor alignment;
        private string currentText = string.Empty;
        private Color currentColor = Color.white;

        public string text
        {
            get => currentText;
            set
            {
                value = (value ?? string.Empty).ToUpperInvariant();
                if (currentText == value) return;
                currentText = value;
                Rebuild();
            }
        }

        public Color color
        {
            get => currentColor;
            set
            {
                currentColor = value;
                if (spriteRenderer != null) spriteRenderer.color = value;
            }
        }

        public void Initialize(string value, int size, TextAnchor anchor)
        {
            alignment = anchor;
            pixelScale = Mathf.Max(1, Mathf.RoundToInt(size / 5f));
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 50;
            currentText = (value ?? string.Empty).ToUpperInvariant();
            Rebuild();
        }

        private void Rebuild()
        {
            if (spriteRenderer == null) return;
            var glyphCount = Mathf.Max(1, currentText.Length);
            var width = Mathf.Max(1, (glyphCount * (GlyphWidth + GlyphSpacing) - GlyphSpacing) * pixelScale);
            var height = GlyphHeight * pixelScale;
            if (texture == null || texture.width != width || texture.height != height)
            {
                ReleaseTexture();
                texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
                {
                    name = name + " Pixel Font",
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };
                sprite = Sprite.Create(texture, new Rect(0, 0, width, height), PivotFor(alignment), 1f, 0, SpriteMeshType.FullRect);
                sprite.name = name + " Pixel Text";
                spriteRenderer.sprite = sprite;
            }
            var pixels = new Color32[width * height];
            for (var index = 0; index < currentText.Length; index++) DrawGlyph(pixels, width, index, currentText[index]);
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            spriteRenderer.color = currentColor;
        }

        private void DrawGlyph(Color32[] pixels, int textureWidth, int characterIndex, char character)
        {
            if (!Glyphs.TryGetValue(character, out var glyph)) glyph = Glyphs[' '];
            var originX = characterIndex * (GlyphWidth + GlyphSpacing) * pixelScale;
            for (var row = 0; row < GlyphHeight; row++)
            for (var column = 0; column < GlyphWidth; column++)
            {
                if (glyph[row * GlyphWidth + column] != '1') continue;
                for (var y = 0; y < pixelScale; y++)
                for (var x = 0; x < pixelScale; x++)
                {
                    var targetX = originX + column * pixelScale + x;
                    var targetY = (GlyphHeight - 1 - row) * pixelScale + y;
                    pixels[targetY * textureWidth + targetX] = PixelPalette.White;
                }
            }
        }

        private static Vector2 PivotFor(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.LowerLeft: return new Vector2(0f, .5f);
                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight: return new Vector2(1f, .5f);
                default: return new Vector2(.5f, .5f);
            }
        }

        private void ReleaseTexture()
        {
            DestroyRuntimeObject(sprite);
            DestroyRuntimeObject(texture);
            sprite = null;
            texture = null;
        }

        private static void DestroyRuntimeObject(Object target)
        {
            if (target == null) return;
            if (Application.isPlaying) Destroy(target);
            else DestroyImmediate(target);
        }

        private void OnDestroy() => ReleaseTexture();
    }
}
