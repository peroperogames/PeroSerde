namespace PeroTools2.Serde.Generator;

/// <summary>
/// 可序列化的类型
/// </summary>
public enum MemberKind
{
    Value,
    List,
    ReadonlyList,
    Dictionary,
    ReadonlyDictionary
}