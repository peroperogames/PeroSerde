namespace PeroTools2.Serde.Json
{
    /// <summary>Registers the UnityEngine-specific formatters that the core library cannot know about.</summary>
    internal static class UnityFormattersRegistration
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif
        private static void Register()
        {
            SerdeJsonFormatterProvider.Register(new Vector2Formatter());
            SerdeJsonFormatterProvider.Register(new Vector3Formatter());
            SerdeJsonFormatterProvider.Register(new Vector4Formatter());
            SerdeJsonFormatterProvider.Register(new Vector2IntFormatter());
            SerdeJsonFormatterProvider.Register(new Vector3IntFormatter());
            SerdeJsonFormatterProvider.Register(new QuaternionFormatter());
            SerdeJsonFormatterProvider.Register(new ColorFormatter());
        }
    }
}