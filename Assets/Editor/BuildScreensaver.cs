#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace CleanRoomArcade.EditorTools
{
    public static class BuildScreensaver
    {
        private const string OutputDirectory = "Build/Windows";
        private const string PlayerPath = OutputDirectory + "/DKArcadePlayer.exe";

        [MenuItem("Construction Climb/Build/Windows Player")]
        public static void BuildPlayer()
        {
            ProjectSetup.Validate();
            Directory.CreateDirectory(OutputDirectory);
            var options = new BuildPlayerOptions
            {
                scenes = new[] { ProjectSetup.BootScene },
                locationPathName = PlayerPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"Unity player build failed: {report.summary.result}");
            Debug.Log($"Built {PlayerPath}");
        }

        [MenuItem("Construction Climb/Build/Assemble Screensaver")]
        public static void AssembleScreensaver()
        {
            BuildPlayer();
            var project = Path.GetFullPath("Wrapper/ScreensaverWrapper/ScreensaverWrapper.csproj");
            var wrapperOutput = Path.GetFullPath(OutputDirectory + "/WrapperPublish");
            var start = new ProcessStartInfo("dotnet", $"publish \"{project}\" -c Release -r win-x64 --self-contained false -o \"{wrapperOutput}\"")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using var process = Process.Start(start) ?? throw new InvalidOperationException("Could not start dotnet publish.");
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0) throw new BuildFailedException($"Wrapper publish failed.\n{output}\n{error}");
            var wrapperExecutable = Path.Combine(wrapperOutput, "DonkeyKongArcadeScreensaver.exe");
            var screensaver = Path.GetFullPath(OutputDirectory + "/DonkeyKongArcadeScreensaver.scr");
            File.Copy(wrapperExecutable, screensaver, true);
            Debug.Log($"Assembled screensaver at {screensaver}. Keep the Unity player and data folder beside it.");
        }
    }
}
#endif
