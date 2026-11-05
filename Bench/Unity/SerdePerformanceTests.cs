/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/04
 */

using System.Text;
using NUnit.Framework;
using PeroTools2.Serde.Json;
using Unity.PerformanceTesting;
using UnityEngine;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using SerdeJson = PeroTools2.Serde.Json.JsonConverter;
#if !SERDE_BENCH_NO_SWIFTER
using SwifterFormatter = Swifter.Json.JsonFormatter;
#endif

namespace PeroTools2.Serde.Benchmarks.Unity
{
    /// <summary>
    /// The Unity half of the comparison, measured with com.unity.test-framework.performance instead of
    /// BenchmarkDotNet. The same payload is used, so the sample groups can be put next to the .NET Core table.
    /// </summary>
    /// <remarks>
    /// Run from the Editor (Mono):
    ///   Unity.exe -batchmode -nographics -projectPath &lt;project&gt; -runTests -testPlatform EditMode
    ///             -testFilter PeroTools2.Serde.Benchmarks.Unity.JsonPerformanceTests -testResults results.xml
    /// Run in a player (IL2CPP/Mono):
    ///   Unity.exe -batchmode -nographics -projectPath &lt;project&gt; -runTests -testPlatform StandaloneWindows64
    ///             -testFilter PeroTools2.Serde.Benchmarks.Unity.JsonPerformanceTests -testResults results.xml
    /// </remarks>
    public class JsonPerformanceTests
    {
        private const int warmupCount = 10;
        private const int measurementCount = 50;
        private const int iterationsPerMeasurement = 100;

        private static readonly string s_utf16 = SerdeJson.UTF16.Serialize(TestObject.instance);
        private static readonly byte[] s_utf8 = SerdeJson.UTF8.Serialize(TestObject.instance);
        private static readonly string s_newtonsoft = JsonConvert.SerializeObject(TestObject.instance);
        private static readonly string s_unityjson = JsonUtility.ToJson(TestObject.instance);

        [Test, Performance]
        public void Serialize_PeroSerde_Utf16()
        {
            Measure.Method(() => SerdeJson.UTF16.Serialize(TestObject.instance))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Serialize_PeroSerde_Utf8()
        {
            Measure.Method(() => SerdeJson.UTF8.Serialize(TestObject.instance))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Serialize_NewtonsoftJson()
        {
            Measure.Method(() => JsonConvert.SerializeObject(TestObject.instance))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }
        [Test, Performance]
        public void Serialize_UnityJson()
        {
            // UnityEngine.JsonUtility: it skips the char member, which the readme calls out
            Measure.Method(() => JsonUtility.ToJson(TestObject.instance))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }

#if !SERDE_BENCH_NO_SWIFTER
        [Test, Performance]
        public void Serialize_SwifterJson()
        {
            Measure.Method(() => SwifterFormatter.SerializeObject(TestObject.instance))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }
#endif
#if !SERDE_BENCH_NO_SWIFTER
        [Test, Performance]
        public void Serialize_SwifterJson_Utf8()
        {
            Measure.Method(() => SwifterFormatter.SerializeObject(TestObject.instance, Encoding.UTF8))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }
#endif

        [Test, Performance]
        public void Deserialize_PeroSerde_Utf16()
        {
            Measure.Method(() => SerdeJson.UTF16.Deserialize<TestObject>(s_utf16))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Deserialize_PeroSerde_Utf8()
        {
            Measure.Method(() => SerdeJson.UTF8.Deserialize<TestObject>(s_utf8))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Deserialize_NewtonsoftJson()
        {
            Measure.Method(() => JsonConvert.DeserializeObject<TestObject>(s_newtonsoft))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }
        [Test, Performance]
        public void Deserialize_UnityJson()
        {
            Measure.Method(() => JsonUtility.FromJson<TestObject>(s_unityjson))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }

#if !SERDE_BENCH_NO_SWIFTER
        [Test, Performance]
        public void Deserialize_SwifterJson()
        {
            Measure.Method(() => SwifterFormatter.DeserializeObject<TestObject>(s_utf16))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }
#endif
#if !SERDE_BENCH_NO_SWIFTER
        [Test, Performance]
        public void Deserialize_SwifterJson_Utf8()
        {
            Measure.Method(() => SwifterFormatter.DeserializeObject<TestObject>(s_utf8, Encoding.UTF8))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(iterationsPerMeasurement)
                .GC()
                .Run();
        }
#endif

        [Test, Performance]
        public void Serialize_Large_PeroSerde_Utf16()
        {
            var big = BigObject.Create();
            Measure.Method(() => SerdeJson.UTF16.Serialize(big))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Serialize_Large_PeroSerde_Utf8()
        {
            var big = BigObject.Create();
            Measure.Method(() => SerdeJson.UTF8.Serialize(big))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Serialize_Large_NewtonsoftJson()
        {
            var big = BigObject.Create();
            Measure.Method(() => JsonConvert.SerializeObject(big))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }
        [Test, Performance]
        public void Serialize_Large_UnityJson()
        {
            var big = BigObject.Create();
            Measure.Method(() => JsonUtility.ToJson(big))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }

#if !SERDE_BENCH_NO_SWIFTER
        [Test, Performance]
        public void Serialize_Large_SwifterJson()
        {
            var big = BigObject.Create();
            Measure.Method(() => SwifterFormatter.SerializeObject(big))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }
#endif
#if !SERDE_BENCH_NO_SWIFTER
        [Test, Performance]
        public void Serialize_Large_SwifterJson_Utf8()
        {
            var big = BigObject.Create();
            Measure.Method(() => SwifterFormatter.SerializeObject(big, Encoding.UTF8))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }
#endif

        [Test, Performance]
        public void Deserialize_Large_PeroSerde_Utf16()
        {
            var json = SerdeJson.UTF16.Serialize(BigObject.Create());
            Measure.Method(() => SerdeJson.UTF16.Deserialize<BigObject>(json))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Deserialize_Large_PeroSerde_Utf8()
        {
            var json = SerdeJson.UTF8.Serialize(BigObject.Create());
            Measure.Method(() => SerdeJson.UTF8.Deserialize<BigObject>(json))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Deserialize_Large_NewtonsoftJson()
        {
            var json = JsonConvert.SerializeObject(BigObject.Create());
            Measure.Method(() => JsonConvert.DeserializeObject<BigObject>(json))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }
        [Test, Performance]
        public void Deserialize_Large_UnityJson()
        {
            var json = JsonUtility.ToJson(BigObject.Create());
            Measure.Method(() => JsonUtility.FromJson<BigObject>(json))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }
#if !SERDE_BENCH_NO_SWIFTER
        [Test, Performance]
        public void Deserialize_Large_SwifterJson()
        {
            var json = SerdeJson.UTF16.Serialize(BigObject.Create());
            Measure.Method(() => SwifterFormatter.DeserializeObject<BigObject>(json))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }
#endif
#if !SERDE_BENCH_NO_SWIFTER
        [Test, Performance]
        public void Deserialize_Large_SwifterJson_Utf8()
        {
            var json = SerdeJson.UTF8.Serialize(BigObject.Create());
            Measure.Method(() => SwifterFormatter.DeserializeObject<BigObject>(json, Encoding.UTF8))
                .WarmupCount(warmupCount)
                .MeasurementCount(measurementCount)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }
#endif

    }
}
