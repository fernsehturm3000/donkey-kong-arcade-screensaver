#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CleanRoomArcade.EditorTools
{
    public static class ProjectSetup
    {
        public const string BootScene = "Assets/Scenes/Boot.unity";

        [MenuItem("Construction Climb/Validate Project Setup")]
        public static void Validate()
        {
            if (!System.IO.File.Exists(BootScene)) throw new System.IO.FileNotFoundException("Boot scene is missing.", BootScene);
            var scenes = EditorBuildSettings.scenes.ToList();
            var existing = scenes.FindIndex(scene => scene.path == BootScene);
            if (existing < 0) scenes.Insert(0, new EditorBuildSettingsScene(BootScene, true));
            else scenes[existing] = new EditorBuildSettingsScene(BootScene, true);
            EditorBuildSettings.scenes = scenes.ToArray();
            PlayerSettings.productName = "DK Arcade Player";
            PlayerSettings.companyName = "Clean Room Arcade";
            Debug.Log("Construction Climb project setup is valid. Boot scene is enabled for builds.");
        }
    }
}
#endif
