#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildAndroid
{
    public static void Build()
    {
        const string outputPath = "Builds/Android/EchoLoop.apk";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        PlayerSettings.applicationIdentifier = "com.rihanpng.echoloop";
        PlayerSettings.productName = "Echo Loop";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 1;

        var scenes = new[]
        {
            "Assets/EchoLoop/Scenes/MainMenu.unity",
            "Assets/EchoLoop/Scenes/Prototype.unity"
        };

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.CompressWithLz4HC
        });

        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new BuildFailedException($"Android build failed: {report.summary.result}");
        }

        Debug.Log($"Android APK created: {Path.GetFullPath(outputPath)} ({report.summary.totalSize / (1024f * 1024f):0.00} MB)");
    }
}
#endif
