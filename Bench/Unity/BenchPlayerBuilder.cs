/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/04
 */

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PeroTools2.Serde.Benchmarks.Unity
{
    /// <summary>
    /// Builds a standalone Windows player that runs the Mono or the IL2CPP measurement at startup.
    /// Invoke from the command line with
    ///   -executeMethod PeroTools2.Serde.Benchmarks.Unity.BenchPlayerBuilder.BuildMono
    ///   -executeMethod PeroTools2.Serde.Benchmarks.Unity.BenchPlayerBuilder.BuildIl2Cpp
    /// then start the player with <c>-serde-bench -batchmode -nographics</c>.
    /// </summary>
    public static class BenchPlayerBuilder
    {
        private const string scenePath = "Assets/BenchScene.unity";

        public static void BuildMono()
        {
            Build(ScriptingImplementation.Mono2x, "Builds/Mono/PeroSerdeBench.exe");
            EditorApplication.Exit(0);
        }

        /// <summary>
        /// Swifter.Core cannot be converted by IL2CPP (EnumHelper::On throws "Cannot get stack type for T"), so the
        /// plugin folder is moved outside the project for the duration of the IL2CPP build and the harness is asked
        /// to skip its Swifter cases.
        /// </summary>
        public static void BuildIl2Cpp()
        {
            const string pluginPath = "Assets/Plugins/Swifter";
            var parkedPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../_swifter_off"));
            var parked = new List<string>();
            try
            {
                // Park the assemblies (not the folder, Unity recreates removed folders while refreshing)
                if (Directory.Exists(pluginPath))
                {
                    Directory.CreateDirectory(parkedPath);
                    foreach (var file in Directory.GetFiles(pluginPath))
                    {
                        var target = Path.Combine(parkedPath, Path.GetFileName(file));
                        File.Copy(file, target, true);
                        File.Delete(file);
                        parked.Add(target);
                    }
                }
                SetDefine("SERDE_BENCH_NO_SWIFTER");
                AssetDatabase.Refresh();
                Build(ScriptingImplementation.IL2CPP, "Builds/IL2CPP/PeroSerdeBench.exe");
            }
            finally
            {
                SetDefine(null);
                foreach (var parkedFile in parked)
                {
                    if (!File.Exists(parkedFile)) continue;
                    Directory.CreateDirectory(pluginPath);
                    File.Copy(parkedFile, Path.Combine(pluginPath, Path.GetFileName(parkedFile)), true);
                    File.Delete(parkedFile);
                }
                AssetDatabase.Refresh();
            }
            // Exit only after the assemblies and the define are restored
            EditorApplication.Exit(0);
        }

        private static void SetDefine(string define)
        {
#if UNITY_2022_2_OR_NEWER
            var target = NamedBuildTarget.Standalone;
            PlayerSettings.SetScriptingDefineSymbols(target, string.IsNullOrEmpty(define) ? string.Empty : define);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, define ?? string.Empty);
#endif
        }

        private static void Build(ScriptingImplementation backend, string outputPath)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, backend);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Standalone, ManagedStrippingLevel.Disabled);
            PlayerSettings.colorSpace = ColorSpace.Linear;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var marker = new GameObject("BenchMarker");
            marker.AddComponent<BenchMarker>();
            EditorSceneManager.SaveScene(scene, scenePath);
            EditorBuildSettings.scenes = new[] {new EditorBuildSettingsScene(scenePath, true)};

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] {scenePath},
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"build failed: {report.summary.result}");
            }
            Debug.Log($"[BENCH] built {backend} player: {outputPath}");
        }
    }
}
#endif
