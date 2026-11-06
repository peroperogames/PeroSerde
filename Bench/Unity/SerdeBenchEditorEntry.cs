/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/04
 */

using UnityEngine;

namespace PeroTools2.Serde.Benchmarks.Unity
{
    /// <summary>
    /// Empty component that keeps the benchmark assembly in a player build, because a scene has to reference it.
    /// The measurements themselves run from <see cref="BenchLauncher"/>.
    /// </summary>
    public sealed class BenchMarker : MonoBehaviour
    {
    }
}

#if UNITY_EDITOR
namespace PeroTools2.Serde.Benchmarks.Unity
{
    using System.IO;
    using UnityEditor;

    /// <summary>
    /// Editor entry point for a headless Mono measurement:
    /// -executeMethod PeroTools2.Serde.Benchmarks.Unity.BenchEditorEntry.Run
    /// </summary>
    public static class BenchEditorEntry
    {
        public static void Run()
        {
            const string path = "BenchResults/serde-bench-editor.csv";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, SerdeBench.Run(path));
            EditorApplication.Exit(0);
        }
    }
}
#endif
