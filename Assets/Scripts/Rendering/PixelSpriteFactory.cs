using System.Collections.Generic;
using UnityEngine;

namespace CleanRoomArcade.Rendering
{
    public static class PixelSpriteFactory
    {
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static Sprite FromMatrix(string key, int[,] indices, Color32[] palette, float pixelsPerUnit = 1f)
        {
            if (Cache.TryGetValue(key, out var cached)) return cached;
            var height = indices.GetLength(0);
            var width = indices.GetLength(1);
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = key + " Texture",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            var colors = new Color32[width * height];
            for (var row = 0; row < height; row++)
            for (var column = 0; column < width; column++)
            {
                var paletteIndex = indices[height - 1 - row, column];
                colors[row * width + column] = paletteIndex >= 0 && paletteIndex < palette.Length
                    ? palette[paletteIndex]
                    : PixelPalette.Clear;
            }
            texture.SetPixels32(colors);
            texture.Apply(false, true);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(.5f, .5f), pixelsPerUnit, 0, SpriteMeshType.FullRect);
            sprite.name = key;
            Cache[key] = sprite;
            return sprite;
        }

        public static GameObject Block(string name, Transform parent, Vector2 position, Vector2 size, Color color, int order = 0)
        {
            var item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            item.transform.localScale = new Vector3(size.x, size.y, 1);
            var renderer = item.AddComponent<SpriteRenderer>();
            renderer.sprite = SolidSprite();
            renderer.color = color;
            renderer.sortingOrder = order;
            return item;
        }

        public static SpriteRenderer SpriteObject(string name, Transform parent, Vector2 position, Sprite sprite, int order = 0)
        {
            var item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            var renderer = item.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            return renderer;
        }

        private static Sprite SolidSprite()
        {
            return FromMatrix("solid-pixel", new[,] { { 0 } }, new[] { PixelPalette.White });
        }
    }
}
