using UnityEngine;

namespace CleanRoomArcade.UI
{
    public static class ArcadeHud
    {
        public static TextMesh Label(Transform parent, string name, string text, Vector2 position, int size = 10, TextAnchor anchor = TextAnchor.MiddleCenter)
        {
            var item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = new Vector3(position.x, position.y, -1f);
            var mesh = item.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.fontSize = size;
            mesh.characterSize = 1f;
            mesh.anchor = anchor;
            mesh.alignment = TextAlignment.Center;
            mesh.color = Color.white;
            mesh.GetComponent<MeshRenderer>().sortingOrder = 50;
            return mesh;
        }
    }
}
