/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/04
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using SerdeJson = PeroTools2.Serde.Json.JsonConverter;
#if !SERDE_BENCH_NO_SWIFTER
using SwifterFormatter = Swifter.Json.JsonFormatter;
#endif

namespace PeroTools2.Serde.Benchmarks.Unity
{
    /// <summary>
    /// Stopwatch benchmark for runtimes where BenchmarkDotNet cannot run (Mono and IL2CPP player builds).
    /// Inside the Editor prefer <see cref="JsonPerformanceTests"/>, which reports statistics through
    /// com.unity.test-framework.performance.
    /// </summary>
    /// <remarks>
    /// Requires the Newtonsoft.Json package and the Swifter.Json/Swifter.Core assemblies (netstandard2.0) to be
    /// referenced. Swifter.Json is reported as "unsupported" when its reflection-based path is stripped, which is
    /// what can happen under IL2CPP.
    /// </remarks>
    public static class SerdeBench
    {
        private const int warmupIterations = 2000;
        private const int measureIterations = 200000;
        private const int bigIterations = 2000;

        private static readonly string s_utf16 = SerdeJson.UTF16.Serialize(TestObject.instance);
        private static readonly byte[] s_utf8 = SerdeJson.UTF8.Serialize(TestObject.instance);
        private static readonly string s_newtonsoft = JsonConvert.SerializeObject(TestObject.instance);
        private static readonly string s_unityjson = JsonUtility.ToJson(TestObject.instance);

        public static string Run(string outputPath)
        {
            var sb = new StringBuilder();
            string Sink(object value)
            {
                if (value == null) throw new InvalidOperationException("null result");
                return value.GetType().Name;
            }

            void Flush()
            {
                if (string.IsNullOrEmpty(outputPath)) return;
                File.WriteAllText(outputPath, sb.ToString());
            }

            void Report(string category, string name, int iterations, Func<int> action, int payloadSize)
            {
                for (var i = 0; i < warmupIterations; i++) action();
                var sw = Stopwatch.StartNew();
                var checksum = 0;
                for (var i = 0; i < iterations; i++) checksum += action();
                sw.Stop();
                var nsPerOp = sw.Elapsed.TotalMilliseconds * 1_000_000.0 / iterations;
                var throughput = payloadSize > 0 ? payloadSize / nsPerOp * 1000.0 : 0;
                sb.AppendLine($"{category},{name},{nsPerOp:F1},{throughput:F1},{checksum}");
                Debug.Log($"[BENCH] {category,-15} {name,-22} {nsPerOp,10:F1} ns/op {throughput,8:F1} MB/s");
                Flush();
            }

            // A serializer that is unavailable or stripped reports itself instead of taking the whole run down
            void ReportUnsupported(string category, string name, Exception exception)
            {
                sb.AppendLine($"{category},{name},unsupported,0,0");
                sb.AppendLine($"# {name} failed: {exception.GetType().Name}: {exception.Message}");
                Debug.Log($"[BENCH] {category,-15} {name,-22} unsupported ({exception.GetType().Name})");
                Flush();
            }

            sb.AppendLine($"# runtime={Application.unityVersion} platform={Application.platform} scripting={GetScriptingBackend()} " +
                          $"payload=utf8:{s_utf8.Length}B utf16:{s_utf16.Length}chars");
            sb.AppendLine("category,case,ns_per_op,mb_per_s,checksum");

            Report("serialize", "serde-utf16", measureIterations, () => SerdeJson.UTF16.Serialize(TestObject.instance).Length, s_utf16.Length * 2);
            Report("serialize", "serde-utf8", measureIterations, () => SerdeJson.UTF8.Serialize(TestObject.instance).Length, s_utf8.Length);
            Report("serialize", "newtonsoft", measureIterations, () => JsonConvert.SerializeObject(TestObject.instance).Length, s_newtonsoft.Length * 2);
            Report("serialize", "unityjson", measureIterations, () => JsonUtility.ToJson(TestObject.instance).Length, s_unityjson.Length * 2);
#if SERDE_BENCH_NO_SWIFTER
            ReportUnsupported("serialize", "swifter-utf16", new PlatformNotSupportedException("Swifter.Json is excluded from this build"));
            ReportUnsupported("serialize", "swifter-utf8", new PlatformNotSupportedException("Swifter.Json is excluded from this build"));
#else
            try
            {
                Report("serialize", "swifter-utf16", measureIterations, () => SwifterFormatter.SerializeObject(TestObject.instance).Length, s_newtonsoft.Length * 2);
            }
            catch (Exception e)
            {
                ReportUnsupported("serialize", "swifter-utf16", e);
            }

            try
            {
                Report("serialize", "swifter-utf8", measureIterations, () => SwifterFormatter.SerializeObject(TestObject.instance, Encoding.UTF8).Length, s_utf8.Length);
            }
            catch (Exception e)
            {
                ReportUnsupported("serialize", "swifter-utf8", e);
            }
#endif

            Report("deserialize", "serde-utf16", measureIterations, () => Sink(SerdeJson.UTF16.Deserialize<TestObject>(s_utf16)).Length, s_utf16.Length * 2);
            Report("deserialize", "serde-utf8", measureIterations, () => Sink(SerdeJson.UTF8.Deserialize<TestObject>(s_utf8)).Length, s_utf8.Length);
            Report("deserialize", "newtonsoft", measureIterations, () => Sink(JsonConvert.DeserializeObject<TestObject>(s_newtonsoft)).Length, s_newtonsoft.Length * 2);
            Report("deserialize", "unityjson", measureIterations, () => Sink(JsonUtility.FromJson<TestObject>(s_unityjson)).Length, s_unityjson.Length * 2);
#if SERDE_BENCH_NO_SWIFTER
            ReportUnsupported("deserialize", "swifter-utf16", new PlatformNotSupportedException("Swifter.Json is excluded from this build"));
            ReportUnsupported("deserialize", "swifter-utf8", new PlatformNotSupportedException("Swifter.Json is excluded from this build"));
#else
            try
            {
                Report("deserialize", "swifter-utf16", measureIterations, () => Sink(SwifterFormatter.DeserializeObject<TestObject>(s_utf16)).Length, s_utf16.Length * 2);
            }
            catch (Exception e)
            {
                ReportUnsupported("deserialize", "swifter-utf16", e);
            }

            try
            {
                Report("deserialize", "swifter-utf8", measureIterations, () => Sink(SwifterFormatter.DeserializeObject<TestObject>(s_utf8, Encoding.UTF8)).Length, s_utf8.Length);
            }
            catch (Exception e)
            {
                ReportUnsupported("deserialize", "swifter-utf8", e);
            }
#endif

            var big = BigObject.Create();
            var bigUtf16 = SerdeJson.UTF16.Serialize(big);
            var bigUtf8 = SerdeJson.UTF8.Serialize(big);
            var bigNewtonsoft = JsonConvert.SerializeObject(big);
            sb.AppendLine($"# big payload=utf8:{bigUtf8.Length}B utf16:{bigUtf16.Length}chars newtonsoft:{bigNewtonsoft.Length}chars");
            Report("big-serialize", "serde-utf16", bigIterations, () => SerdeJson.UTF16.Serialize(big).Length, bigUtf16.Length * 2);
            Report("big-serialize", "serde-utf8", bigIterations, () => SerdeJson.UTF8.Serialize(big).Length, bigUtf8.Length);
            Report("big-serialize", "newtonsoft", bigIterations, () => JsonConvert.SerializeObject(big).Length, bigNewtonsoft.Length * 2);
            var bigUnityJson = JsonUtility.ToJson(big);
            Report("big-serialize", "unityjson", bigIterations, () => JsonUtility.ToJson(big).Length, bigUnityJson.Length * 2);
#if SERDE_BENCH_NO_SWIFTER
            ReportUnsupported("big-serialize", "swifter-utf16", new PlatformNotSupportedException("Swifter.Json is excluded from this build"));
            ReportUnsupported("big-serialize", "swifter-utf8", new PlatformNotSupportedException("Swifter.Json is excluded from this build"));
#else
            try
            {
                Report("big-serialize", "swifter-utf16", bigIterations, () => SwifterFormatter.SerializeObject(big).Length, bigNewtonsoft.Length * 2);
            }
            catch (Exception e)
            {
                ReportUnsupported("big-serialize", "swifter-utf16", e);
            }

            try
            {
                Report("big-serialize", "swifter-utf8", bigIterations, () => SwifterFormatter.SerializeObject(big, Encoding.UTF8).Length, bigUtf8.Length);
            }
            catch (Exception e)
            {
                ReportUnsupported("big-serialize", "swifter-utf8", e);
            }
#endif

            Report("big-deserialize", "serde-utf16", bigIterations, () => Sink(SerdeJson.UTF16.Deserialize<BigObject>(bigUtf16)).Length, bigUtf16.Length * 2);
            Report("big-deserialize", "serde-utf8", bigIterations, () => Sink(SerdeJson.UTF8.Deserialize<BigObject>(bigUtf8)).Length, bigUtf8.Length);
            Report("big-deserialize", "newtonsoft", bigIterations, () => Sink(JsonConvert.DeserializeObject<BigObject>(bigNewtonsoft)).Length, bigNewtonsoft.Length * 2);
            Report("big-deserialize", "unityjson", bigIterations, () => Sink(JsonUtility.FromJson<BigObject>(bigUnityJson)).Length, bigUnityJson.Length * 2);
#if SERDE_BENCH_NO_SWIFTER
            ReportUnsupported("big-deserialize", "swifter-utf16", new PlatformNotSupportedException("Swifter.Json is excluded from this build"));
            ReportUnsupported("big-deserialize", "swifter-utf8", new PlatformNotSupportedException("Swifter.Json is excluded from this build"));
#else
            try
            {
                Report("big-deserialize", "swifter-utf16", bigIterations, () => Sink(SwifterFormatter.DeserializeObject<BigObject>(bigUtf16)).Length, bigUtf16.Length * 2);
            }
            catch (Exception e)
            {
                ReportUnsupported("big-deserialize", "swifter-utf16", e);
            }

            try
            {
                Report("big-deserialize", "swifter-utf8", bigIterations, () => Sink(SwifterFormatter.DeserializeObject<BigObject>(bigUtf8, Encoding.UTF8)).Length, bigUtf8.Length);
            }
            catch (Exception e)
            {
                ReportUnsupported("big-deserialize", "swifter-utf8", e);
            }
#endif

            return sb.ToString();
        }

        private static string GetScriptingBackend()
        {
#if ENABLE_IL2CPP
            return "IL2CPP";
#else
            return "Mono";
#endif
        }
    }

    /// <summary>
    /// Writes the benchmark table next to the player and quits, so a headless Mono/IL2CPP run needs no scene wiring.
    /// Pass <c>-serde-bench</c> to the player to enable it.
    /// </summary>
    public static class BenchLauncher
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RunAtStartup()
        {
            var args = Environment.GetCommandLineArgs();
            if (Array.IndexOf(args, "-serde-bench") < 0)
            {
                return;
            }
            var directory = Path.GetDirectoryName(args[0]) ?? ".";
            var path = Path.Combine(directory, "serde-bench.csv");
            SerdeBench.Run(path);
            Debug.Log($"[BENCH] written to {path}");
            Application.Quit(0);
        }
    }
}
