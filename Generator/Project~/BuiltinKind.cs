/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23-17:41:00
 */

namespace PeroTools2.Serde.Generator;

/// <summary>
/// 可序列化的类型
/// </summary>
public enum BuiltinKind
{
    Value,
    Enum,
    Lazy,
    Nullable,
    Array,
    Collection,
    ReadonlyCollection,
    KeyValuePair,
}