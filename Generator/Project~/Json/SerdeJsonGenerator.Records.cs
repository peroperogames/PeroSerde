/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/03-14:32:04
 */

using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.CodeAnalysis;

namespace PeroTools2.Serde.Generator;

public partial class SerdeJsonGenerator
{
    private string GenerateRecordsFormatter(ITypeSymbol typeSymbol, TypeMeta typeMeta)
    {
        var ns = typeSymbol.ContainingNamespace;
        var sb = new StringBuilder();

        if (ns is { IsGlobalNamespace: false })
        {
            sb.AppendLine($"namespace {ns} {{");
        }
        sb.AppendLine(GenerateRecordsFormatterRegistry(typeSymbol, typeMeta));
        sb.AppendLine(GenerateRecordsSerialization(typeSymbol, typeMeta));
        sb.Append(GenerateRecordsDeserialization(typeSymbol, typeMeta));
        if (ns is { IsGlobalNamespace: false })
        {
            sb.AppendLine("}");
        }
        return sb.ToString();
    }

    private string GenerateRecordsFormatterRegistry(ITypeSymbol type, TypeMeta typeMeta)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var members = typeMeta.members;
        var formatterBuilder = new StringBuilder();
        var formatterRegistry = new StringBuilder();
        foreach (var member in members)
        {
            if (!TryGetFormatterType(member, out var formatter))
            {
                continue;
            }
            formatterBuilder.AppendLine($"    private static {formatter} s_{member.name}Formatter;");

            // 在注册 Collection 之前需要先注册元素类型，以免找不到 Formatter
            RegisterElementTypes(member.kind, member.type, 12);

            formatterRegistry.AppendLine($"            s_{member.name}Formatter = new {formatter}();");
            if (member.formatter == null)
            {
                formatterRegistry.AppendLine($"            global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.Register(s_{member.name}Formatter);");
            }
        }
        var body = $$"""
                     {
                         if (!global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.IsRegistered<{{type.ToDisplayString()}}>())
                         {
                             global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.Register(new JsonFormatter());
                 {{formatterRegistry}}
                         }
                     }
                 """;
        return
            $$"""
            // Registry
            partial {{type.GetTypeKind()}} {{typeName}}
            {
            {{formatterBuilder}}
                static {{type.Name}}()
            {{body}}
            """
            + '\n'
            + (type.IsGeneric()
                ? string.Empty // 泛型不允许饿汉注册
                : """
                      #if UNITY_EDITOR
                      [global::UnityEditor.InitializeOnLoadMethod]
                      #else
                      [global::UnityEngine.RuntimeInitializeOnLoadMethod(global::UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
                      #endif
                  """)
            + '\n'
            + $$"""
                  public static void RegisterFormattersFor{{typeName}}()
              {{body}}
              }
              """;

        void RegisterElementTypes(BuiltinKind kind, ITypeSymbol type, int padding)
        {
            if (kind is not (BuiltinKind.Collection or BuiltinKind.Array or BuiltinKind.ReadonlyCollection or BuiltinKind.Dictionary) || !type.TryGetCollectionElementType(out var elementType))
            {
                return;
            }
            var elementKind = TypeMeta.GetMemberKind(elementType, m_ReferenceSymbols);
            if (m_TypeSymbols.Any(kv => SymbolEqualityComparer.Default.Equals(kv.Key, elementType)))
            {
                formatterRegistry.AppendLine($"{new string(' ', padding)}if (!global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.IsRegistered<{elementType.ToDisplayString()}>())");
                formatterRegistry.AppendLine($"{new string(' ', padding)}{{");
                formatterRegistry.AppendLine(
                    $"{new string(' ', padding + 4)}{elementType.ToDisplayString()}.RegisterFormattersFor{elementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}();");
                formatterRegistry.AppendLine($"{new string(' ', padding)}}}");
            }
            else if (elementType.ContainsAttribute(m_ReferenceSymbols.serdeJsonSerializableAttribute))
            {
                formatterRegistry.AppendLine($"{new string(' ', padding)}if (!global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.IsRegistered<{elementType.ToDisplayString()}>())");
                formatterRegistry.AppendLine($"{new string(' ', padding)}{{");
                formatterRegistry.AppendLine(
                    $"{new string(' ', padding + 4)}{elementType.ToDisplayString()}.RegisterFormattersFor{elementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}();");
                formatterRegistry.AppendLine($"{new string(' ', padding)}}}");
            }
            else if (TryGetFormatterType(new MemberMeta
                     {
                         type = elementType,
                         kind = kind == BuiltinKind.Dictionary ? BuiltinKind.Dictionary : elementKind
                     }, out var elementFormatter))
            {
                if (kind == BuiltinKind.Dictionary && elementType is INamedTypeSymbol kvType)
                {
                    var keyType = kvType.TypeArguments[0];
                    var valueType = kvType.TypeArguments[1];
                    RegisterElementTypes(BuiltinKind.Dictionary, valueType, padding);
                    formatterRegistry.AppendLine(
                        $"{new string(' ', padding)}if (!global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.IsRegistered<System.Collections.Generic.Dictionary<{keyType.ToDisplayString()}, {valueType.ToDisplayString()}>>())");
                    formatterRegistry.AppendLine($"{new string(' ', padding)}{{");
                    formatterRegistry.AppendLine($"{new string(' ', padding + 4)}global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.Register(new {elementFormatter}());");
                    formatterRegistry.AppendLine($"{new string(' ', padding)}}}");
                }
                else
                {
                    RegisterElementTypes(elementKind, elementType, padding);
                    formatterRegistry.AppendLine($"{new string(' ', padding)}if (!global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.IsRegistered<{elementType.ToDisplayString()}>())");
                    formatterRegistry.AppendLine($"{new string(' ', padding)}{{");
                    formatterRegistry.AppendLine($"{new string(' ', padding + 4)}global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.Register(new {elementFormatter}());");
                    formatterRegistry.AppendLine($"{new string(' ', padding)}}}");
                }
            }
        }

        static bool TryGetFormatterType(MemberMeta member, out string formatter)
        {
            formatter = member switch
            {
                { formatter: not null } => member.formatter.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),

                { kind: BuiltinKind.Enum } => $"{member.type.ToDisplayString()}JsonFormatter",

                { kind: BuiltinKind.Array, type: IArrayTypeSymbol arrayType } => $"PeroTools2.Serde.Json.ArrayFormatter<{arrayType.ElementType.ToDisplayString()}>",

                { type: INamedTypeSymbol namedTypeSymbol } => namedTypeSymbol switch
                {
                    _ when member.kind is BuiltinKind.Collection && namedTypeSymbol.TryGetCollectionElementType(out var elementType)
                        => $"PeroTools2.Serde.Json.CollectionFormatter<{member.type.ToDisplayString()}, {elementType.ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.ReadonlyCollection && namedTypeSymbol.TryGetCollectionElementType(out var elementType)
                        => $"PeroTools2.Serde.Json.ReadonlyCollectionFormatter<{member.type.ToDisplayString()}, {elementType.ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.Nullable
                        => $"PeroTools2.Serde.Json.NullableFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.Lazy
                        => $"PeroTools2.Serde.Json.LazyFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.Dictionary
                        => $"PeroTools2.Serde.Json.DictionaryFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}, {namedTypeSymbol.TypeArguments[1].ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.KeyValuePair
                        => $"PeroTools2.Serde.Json.KeyValuePairFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}, {namedTypeSymbol.TypeArguments[1].ToDisplayString()}>",

                    _ => string.Empty
                },
                _ => string.Empty
            };
            return !string.IsNullOrEmpty(formatter);
        }
    }

    private static string GenerateRecordsDeserialization(ITypeSymbol type, TypeMeta typeMeta)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var members = typeMeta.members;

        // 添加成员序列化写入
        var deserializerBuilder = new StringBuilder();
        var utf16Builder = new StringBuilder();
        var utf8Builder = new StringBuilder();

        deserializerBuilder.AppendLine("            if (reader.TryReadNull())");
        deserializerBuilder.AppendLine("            {");
        deserializerBuilder.AppendLine("                return default;");
        deserializerBuilder.AppendLine("            }");

        deserializerBuilder.AppendLine($"            var obj = new {typeName}();");
        deserializerBuilder.AppendLine("            reader.ReadBeginObject();");

        deserializerBuilder.AppendLine("""
                                                   while (reader.TryReadKey(out key))
                                                   {
                                                       OnReadKey(ref key, ref obj, ref reader);
                                                       reader.TryReadComma();
                                                   }
                                                   reader.ReadEndObject();
                                                   return obj;
                                       """);
        for (var index = 0; index < members.Length; index++)
        {
            var member = members[index];
            var memberTypeName = member.type.ToDisplayString();
            var formatter = member.useFormatter ? $"s_{member.name}Formatter, " : string.Empty;
            utf8Builder.AppendLine($"            {(index == 0 ? "if" : "else if")} (PeroTools2.Serde.StringHelper.EqualsUnaligned(key, \"{member.formatName}\"))");
            utf16Builder.AppendLine($"            {(index == 0 ? "if" : "else if")} (PeroTools2.Serde.StringHelper.Equals(key, \"{member.formatName}\"))");
            var memberReadStr = $$"""
                                                  {
                                                      obj.{{member.name}} = reader.ReadValue<{{memberTypeName}}>({{formatter}}false);
                                                  }
                                      """;
            utf16Builder.AppendLine(memberReadStr);
            utf8Builder.AppendLine(memberReadStr);
        }
        if (members.Length > 0)
        {
            // 如果是冗余的Json属性，那么直接读
            var finalCheck =
                """
                            else
                            {
                                reader.ReadUntilEnd();
                            }
                """;
            utf16Builder.Append(finalCheck);
            utf8Builder.Append(finalCheck);
        }
        else
        {
            utf8Builder.AppendLine("                reader.ReadUntilEnd();");
            utf16Builder.AppendLine("                reader.ReadUntilEnd();");
        }

        // P1 原型: 成员全为内置基础类型时, 走游标 + 静态直调的新形态读取器
        var utf8Body = deserializerBuilder.ToString();
        if (members.Length > 0 && members.All(member => TryGetCursorMemberAccess(member, out _)))
        {
            var cursorBody = BuildCursorDeserializeBody(type, members);
            if (cursorBody != null)
            {
                utf8Body = cursorBody;
            }
        }

        // 生成分部序列化器
        return $$"""
                 // Deserializer
                 partial {{type.GetTypeKind()}} {{typeName}}
                 {
                     public partial class JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{type.ToDisplayString()}}>
                     {
                         public {{type.ToDisplayString()}} DeserializeUtf8(ref System.ReadOnlySpan<byte> value, ref PeroTools2.Serde.Json.JsonReaderUtf8 reader)
                         {
                             System.ReadOnlySpan<byte> key;
                 {{utf8Body}}
                         }

                         public {{typeName}} DeserializeUtf16(ref System.ReadOnlySpan<char> value, ref PeroTools2.Serde.Json.JsonReaderUtf16 reader)
                         {
                             System.ReadOnlySpan<char> key;
                 {{deserializerBuilder}}
                         }

                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         private void OnReadKey(ref System.ReadOnlySpan<byte> key, ref {{type.ToDisplayString()}} obj, ref PeroTools2.Serde.Json.JsonReaderUtf8 reader)
                         {
                 {{utf8Builder}}
                         }

                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         private void OnReadKey(ref System.ReadOnlySpan<char> key, ref {{type.ToDisplayString()}} obj, ref PeroTools2.Serde.Json.JsonReaderUtf16 reader)
                         {
                 {{utf16Builder}}
                         }
                     }
                 }
                 """;
    }

    // 是否为本包内置的固定 formatter 类型（SerdeJsonFormatterProvider 的 FixedCache 已注册）
    // 与运行时 StringHelper.GetKeyPrefix 对应的生成期前缀（ASCII 假设，与既有等值比较一致）
    private static ulong GetKeyPrefix(string name)
    {
        ulong value = 0;
        var limit = name.Length < 8 ? name.Length : 8;
        for (var i = 0; i < limit; i++)
        {
            value |= (ulong) (byte) name[i] << (i * 8);
        }
        return value;
    }
    private static bool IsFixedPrimitive(ITypeSymbol type, out string typeName)
    {
        typeName = type.ToDisplayString();
        return type.SpecialType is SpecialType.System_Boolean
            or SpecialType.System_Byte
            or SpecialType.System_SByte
            or SpecialType.System_UInt16
            or SpecialType.System_Int16
            or SpecialType.System_UInt32
            or SpecialType.System_Int32
            or SpecialType.System_UInt64
            or SpecialType.System_Int64
            or SpecialType.System_Single
            or SpecialType.System_Double
            or SpecialType.System_Decimal;
    }

    // 元素/标量 -> struct 读取器（集合用）
    private static bool TryGetCursorElementReader(ITypeSymbol element, out string readerStruct)
    {
        readerStruct = element.SpecialType switch
        {
            SpecialType.System_String => "global::PeroTools2.Serde.Json.StringCursorReader",
            SpecialType.System_Char => "global::PeroTools2.Serde.Json.CharCursorReader",
            SpecialType.System_Boolean => "global::PeroTools2.Serde.Json.BooleanCursorReader",
            SpecialType.System_Byte => "global::PeroTools2.Serde.Json.ByteCursorReader",
            SpecialType.System_SByte => "global::PeroTools2.Serde.Json.SByteCursorReader",
            SpecialType.System_Int16 => "global::PeroTools2.Serde.Json.Int16CursorReader",
            SpecialType.System_UInt16 => "global::PeroTools2.Serde.Json.UInt16CursorReader",
            SpecialType.System_Int32 => "global::PeroTools2.Serde.Json.Int32CursorReader",
            SpecialType.System_UInt32 => "global::PeroTools2.Serde.Json.UInt32CursorReader",
            SpecialType.System_Int64 => "global::PeroTools2.Serde.Json.Int64CursorReader",
            SpecialType.System_UInt64 => "global::PeroTools2.Serde.Json.UInt64CursorReader",
            SpecialType.System_Single => "global::PeroTools2.Serde.Json.SingleCursorReader",
            SpecialType.System_Double => "global::PeroTools2.Serde.Json.DoubleCursorReader",
            SpecialType.System_Decimal => "global::PeroTools2.Serde.Json.DecimalCursorReader",
            _ => null,
        };
        return readerStruct != null;
    }

    // 成员的游标读取表达式（含 ref cursor）；不支持时返回 false
    private static bool TryGetCursorMemberAccess(MemberMeta member, out string expression)
    {
        expression = null;

        var type = member.type;
        const string ns = "global::PeroTools2.Serde.Json";

        // 显式 [SerdeJsonFormatter] 的成员: formatter 不在 provider 注册表里, 用成员自己的实例读取
        if (member.formatter != null)
        {
            expression = $"{ns}.JsonPrimitives.ReadNested<{type.ToDisplayString()}>(ref cursor, s_{member.name}Formatter)";
            return true;
        }

        if (type is IArrayTypeSymbol arrayType && TryGetCursorElementReader(arrayType.ElementType, out var arrayReader))
        {
            expression = $"{ns}.JsonCollections.ReadArray<{arrayType.ElementType.ToDisplayString()}, {arrayReader}>(ref cursor)";
            return true;
        }

        if (type is INamedTypeSymbol named && named.TypeArguments.Length == 1
            && named.Name == "List" && named.ContainingNamespace?.ToDisplayString() == "System.Collections.Generic"
            && TryGetCursorElementReader(named.TypeArguments[0], out var listReader))
        {
            expression = $"{ns}.JsonCollections.ReadList<{named.TypeArguments[0].ToDisplayString()}, {listReader}>(ref cursor)";
            return true;
        }

        if (type.GetAttributes().Any(attribute => attribute.AttributeClass?.Name == "SerdeJsonSerializableAttribute"))
        {
            expression = $"{ns}.JsonPrimitives.ReadNested<{type.ToDisplayString()}>(ref cursor)";
            return true;
        }

        // enum / Nullable / 集合 / 内置结构体等: formatter 已在注册表里, 借道 reader 读取
        // 显式 [SerdeJsonFormatter] 的成员没进注册表, 必须走原路径
        if (member.useFormatter && member.formatter == null)
        {
            expression = $"{ns}.JsonPrimitives.ReadNested<{type.ToDisplayString()}>(ref cursor)";
            return true;
        }

        if (TryGetCursorReader(type, out var readerName))
        {
            expression = $"{ns}.JsonPrimitives.{readerName}(ref cursor)";
            return true;
        }

        return false;
    }
    // 游标路径支持的成员类型（基础类型 + string + char），返回对应的读取器名
    private static bool TryGetCursorReader(ITypeSymbol type, out string readerName)
    {
        switch (type.SpecialType)
        {
            case SpecialType.System_String:
                readerName = "ReadString";
                return true;
            case SpecialType.System_Char:
                readerName = "ReadChar";
                return true;
        }

        if (IsFixedPrimitive(type, out _) && GetPrimitiveReaderName(type.SpecialType, out readerName))
        {
            return true;
        }

        readerName = null;
        return false;
    }
    private static bool GetPrimitiveReaderName(SpecialType specialType, out string readerName)
    {
        readerName = specialType switch
        {
            SpecialType.System_Boolean => "ReadBoolean",
            SpecialType.System_Byte => "ReadByte",
            SpecialType.System_SByte => "ReadSByte",
            SpecialType.System_Int16 => "ReadInt16",
            SpecialType.System_UInt16 => "ReadUInt16",
            SpecialType.System_Int32 => "ReadInt32",
            SpecialType.System_UInt32 => "ReadUInt32",
            SpecialType.System_Int64 => "ReadInt64",
            SpecialType.System_UInt64 => "ReadUInt64",
            SpecialType.System_Single => "ReadSingle",
            SpecialType.System_Double => "ReadDouble",
            SpecialType.System_Decimal => "ReadDecimal",
            SpecialType.System_Char => "ReadChar",
            _ => null,
        };
        return readerName != null;
    }

    // 游标形态的反序列化体（P1 原型）：仅在「前缀+长度」唯一时生成
    private static string BuildCursorDeserializeBody(ITypeSymbol type, MemberMeta[] members)
    {
        // switch 的标签只有前缀: 两个 key 前 8 字节相同就必须整体放弃, 否则生成重复 case
        var seen = new System.Collections.Generic.HashSet<ulong>();
        foreach (var member in members)
        {
            var prefix = GetKeyPrefix(member.formatName);
            if (!TryGetCursorMemberAccess(member, out _))
            {
                return null;
            }

            if (!seen.Add(prefix))
            {
                return null;
            }
        }

        var body = new StringBuilder();
        body.AppendLine("            if (reader.TryReadNull())");
        body.AppendLine("            {");
        body.AppendLine("                return default;");
        body.AppendLine("            }");
        body.AppendLine();
        body.AppendLine($"            var obj = new {type.ToDisplayString()}();");
        body.AppendLine("            var cursor = reader.CreateCursor();");
        body.AppendLine("            cursor.Expect((byte) '{');");
        body.AppendLine("            while (cursor.TryReadKey(out var keyLength, out var prefix))");
        body.AppendLine("            {");
        body.AppendLine("                switch (prefix)");
        body.AppendLine("                {");
        foreach (var member in members)
        {
            TryGetCursorMemberAccess(member, out var memberAccess);
            body.AppendLine($"                    case 0x{GetKeyPrefix(member.formatName):X16}UL:");
            body.AppendLine($"                        if (keyLength == {member.formatName.Length})");
            body.AppendLine("                        {");
            body.AppendLine($"                            obj.{member.name} = {memberAccess};");
            body.AppendLine("                        }");
            body.AppendLine("                        else");
            body.AppendLine("                        {");
            body.AppendLine("                            cursor.SkipValue();");
            body.AppendLine("                        }");
            body.AppendLine("                        break;");
        }
        body.AppendLine("                    default:");
        body.AppendLine("                        cursor.SkipValue();");
        body.AppendLine("                        break;");
        body.AppendLine("                }");
        body.AppendLine();
        body.AppendLine("                cursor.TryConsume((byte) ',');");
        body.AppendLine("            }");
        body.AppendLine();
        body.AppendLine("            cursor.Expect((byte) '}');");
        body.AppendLine("            reader.SyncCursor(in cursor);");
        body.AppendLine("            return obj;");
        return body.ToString();
    }
    private static string GenerateRecordsSerialization(ITypeSymbol type, TypeMeta typeMeta)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var members = typeMeta.members;

        // 添加成员序列化写入
        var stringBuilder = new StringBuilder();
        if (type.IsNullable())
        {
            stringBuilder.AppendLine("            if (value == null)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                writer.WriteNull();");
            stringBuilder.AppendLine("                return;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            writer.WriteBeginObject();");
        }
        else
        {
            stringBuilder.AppendLine("writer.WriteBeginObject();");
        }
        if (typeMeta.ignoreNullMembers)
        {
            // 忽略 null 成员时成员数量在运行时才能确定，故改用前置逗号模式：
            // 写入每个成员前，若之前已写入过成员则先补一个逗号，从而避免尾随逗号。
            stringBuilder.AppendLine("            var wroteMember = false;");
            foreach (var member in members)
            {
                var args = member.useFormatter
                    ? $"\"{member.formatName}\", value.{member.name}, s_{member.name}Formatter, false"
                    : $"\"{member.formatName}\", value.{member.name}, false";
                if (member.canBeNull)
                {
                    stringBuilder.AppendLine($"            if (value.{member.name} != null)");
                    stringBuilder.AppendLine("            {");
                    stringBuilder.AppendLine("                if (wroteMember) writer.WriteValueEnding();");
                    stringBuilder.AppendLine($"                writer.WriteNamedValue({args});");
                    stringBuilder.AppendLine("                wroteMember = true;");
                    stringBuilder.AppendLine("            }");
                }
                else
                {
                    stringBuilder.AppendLine("            if (wroteMember) writer.WriteValueEnding();");
                    stringBuilder.AppendLine($"            writer.WriteNamedValue({args});");
                    stringBuilder.AppendLine("            wroteMember = true;");
                }
            }
        }
        else
        {
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var writeComma = (index != members.Length - 1).ToString().ToLower();
                var args = member.useFormatter
                    ? $"\"{member.formatName}\", value.{member.name}, s_{member.name}Formatter, {writeComma}"
                    : $"\"{member.formatName}\", value.{member.name}, {writeComma}";
                stringBuilder.AppendLine($"            writer.WriteNamedValue({args});");
            }
        }
        stringBuilder.Append("            writer.WriteEndObject();");

        // 生成分部序列化器
        return $$"""
                 // Serializer
                 partial {{type.GetTypeKind()}} {{typeName}}
                 {
                     public partial class JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{type.ToDisplayString()}}>
                     {
                         public void SerializeUtf16({{type.ToDisplayString()}} value, ref PeroTools2.Serde.Json.JsonWriterUtf16 writer)
                         {
                             {{stringBuilder}}
                         }

                         public void SerializeUtf8({{type.ToDisplayString()}} value, ref PeroTools2.Serde.Json.JsonWriterUtf8 writer)
                         {
                             {{stringBuilder}}
                         }
                     }
                 }
                 """;
    }
}
