/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/03-14:32:04
 */

using System.Linq;
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

            // Element types must be registered before the Collection, otherwise its formatter cannot be found
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
                ? string.Empty // eager registration is not allowed for generics
                : """
                      #if UNITY_5_3_OR_NEWER
                      #if UNITY_EDITOR
                      [global::UnityEditor.InitializeOnLoadMethod]
                      #else
                      [global::UnityEngine.RuntimeInitializeOnLoadMethod(global::UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
                      #endif
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

                    // the arity guards keep a misclassified type from throwing inside the generator
                    _ when member.kind is BuiltinKind.Nullable && namedTypeSymbol.TypeArguments.Length == 1
                        => $"PeroTools2.Serde.Json.NullableFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.Lazy && namedTypeSymbol.TypeArguments.Length == 1
                        => $"PeroTools2.Serde.Json.LazyFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.Dictionary && namedTypeSymbol.TypeArguments.Length == 2
                        => $"PeroTools2.Serde.Json.DictionaryFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}, {namedTypeSymbol.TypeArguments[1].ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.KeyValuePair && namedTypeSymbol.TypeArguments.Length == 2
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

        // Add member serialization writes
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
        AppendUtf8KeyDispatch(utf8Builder, members);

        AppendUtf16KeyDispatch(utf16Builder, members);

        // P1 prototype: when all members are builtin primitive types, use the new cursor + static direct-call reader
        var utf8OldBody = new StringBuilder();
        utf8OldBody.AppendLine("            System.ReadOnlySpan<byte> key;");
        utf8OldBody.Append(deserializerBuilder);
        var utf8Body = utf8OldBody.ToString();

        var utf16OldBody = new StringBuilder();
        utf16OldBody.AppendLine("            System.ReadOnlySpan<char> key;");
        utf16OldBody.Append(deserializerBuilder);
        var utf16Body = utf16OldBody.ToString();


        // Generate the partial serializer
        return $$"""
                 // Deserializer
                 partial {{type.GetTypeKind()}} {{typeName}}
                 {
                     public partial class JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{type.ToDisplayString()}}>
                     {
                         public {{type.ToDisplayString()}} DeserializeUtf8(ref System.ReadOnlySpan<byte> value, ref PeroTools2.Serde.Json.JsonReaderUtf8 reader)
                         {
                 {{utf8Body}}
                         }

                         public {{typeName}} DeserializeUtf16(ref System.ReadOnlySpan<char> value, ref PeroTools2.Serde.Json.JsonReaderUtf16 reader)
                         {
                 {{utf16Body}}
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


    /// <summary>
    /// Emits the UTF16 key dispatch as a <c>switch</c> over the first four characters of the key packed into a
    /// <see cref="ulong"/>, instead of a chain of comparisons: the jump table reaches the candidate in one step, and a
    /// member name that fits in eight characters is fully identified by the packed prefix plus the key length, so it
    /// needs no string comparison at all. Names that share their first eight characters fall back to a full compare.
    /// </summary>
    /// <summary>Emits the UTF8 key dispatch as a <c>switch</c> over the first eight key bytes packed into a <c>ulong</c>.</summary>
    /// <remarks>
    /// Eight bytes plus the key length identify a member name of up to eight bytes, so the common case needs no string
    /// comparison. Longer names can collide on the packed prefix and verify in full; names that are not pure ASCII keep
    /// the byte-for-byte comparison because their packed prefix depends on the encoded length.
    /// </remarks>
    private static void AppendUtf8KeyDispatch(StringBuilder builder, MemberMeta[] members)
    {
        if (members.Length == 0)
        {
            builder.AppendLine("                reader.ReadUntilEnd();");
            return;
        }

        var switchable = members.Where(member => IsSimpleKey(member.formatName)).ToArray();
        var fallback = members.Where(member => !IsSimpleKey(member.formatName)).ToArray();
        var groups = switchable
            .GroupBy(member => PackKeyPrefix(member.formatName))
            .Select(group => (prefix: group.Key, candidates: group.ToArray()))
            .ToArray();

        builder.AppendLine("            var prefix = PeroTools2.Serde.StringHelper.GetKeyPrefix(key);");
        builder.AppendLine("            switch (prefix)");
        builder.AppendLine("            {");
        foreach (var (prefix, candidates) in groups)
        {
            builder.AppendLine($"                case 0x{prefix:X}UL:");
            foreach (var member in candidates)
            {
                var name = member.formatName;
                var guard = $"key.Length == {name.Length}";
                if (name.Length > 8)
                {
                    // The switch above already matched the first eight bytes, so only the tail is left to compare
                    guard += $" && PeroTools2.Serde.StringHelper.EqualsUnaligned(key, \"{name}\", 8)";
                }

                builder.AppendLine($"                    if ({guard})");
                builder.AppendLine("                    {");
                builder.AppendLine($"                        obj.{member.name} = {MemberReadStatement(member)};");
                builder.AppendLine("                        return;");
                builder.AppendLine("                    }");
            }

            builder.AppendLine("                    break;");
        }

        builder.AppendLine("            }");
        foreach (var member in fallback)
        {
            // A non-ASCII member name has a different length in UTF16 characters and UTF8 bytes, so this branch
            // compares the bytes instead of guessing a length
            builder.AppendLine($"            if (PeroTools2.Serde.StringHelper.EqualsUtf8(key, \"{member.formatName}\"))");
            builder.AppendLine("            {");
            builder.AppendLine($"                obj.{member.name} = {MemberReadStatement(member)};");
            builder.AppendLine("                return;");
            builder.AppendLine("            }");
        }

        builder.AppendLine("            reader.ReadUntilEnd();");
    }

    /// <summary>Packs the first eight bytes of an ASCII member name into a <c>ulong</c>, like StringHelper.GetKeyPrefix.</summary>
    private static ulong PackKeyPrefix(string name)
    {
        ulong value = 0;
        for (var i = 0; i < 8 && i < name.Length; i++)
        {
            value |= (ulong) (byte) name[i] << (i * 8);
        }

        return value;
    }
    private static void AppendUtf16KeyDispatch(StringBuilder builder, MemberMeta[] members)
    {
        if (members.Length == 0)
        {
            builder.AppendLine("                reader.ReadUntilEnd();");
            return;
        }

        // Non-ASCII member names go through the string comparer; the packed switch only covers ASCII names
        var switchable = members.Where(member => IsSimpleKey(member.formatName)).ToArray();
        var fallback = members.Where(member => !IsSimpleKey(member.formatName)).ToArray();
        var groups = switchable
            .GroupBy(member => PackPrefix(member.formatName, 0))
            .Select(group => (low: group.Key, candidates: group.ToArray()))
            .ToArray();

        builder.AppendLine("            PeroTools2.Serde.StringHelper.GetKeyPrefix(key, out var prefixLow, out var prefixHigh);");
        builder.AppendLine("            switch (prefixLow)");
        builder.AppendLine("            {");
        foreach (var (low, candidates) in groups)
        {
            builder.AppendLine($"                case 0x{low:X}UL:");
            foreach (var member in candidates)
            {
                var name = member.formatName;
                var high = PackPrefix(name, 4);
                // Names longer than eight characters can still collide on the packed prefix, so they verify in full
                var guard = $"prefixHigh == 0x{high:X}UL && key.Length == {name.Length}";
                if (name.Length > 8)
                {
                    guard += $" && PeroTools2.Serde.StringHelper.Equals(key, \"{name}\")";
                }

                builder.AppendLine($"                    if ({guard})");
                builder.AppendLine("                    {");
                builder.AppendLine($"                        obj.{member.name} = {MemberReadStatement(member)};");
                builder.AppendLine("                        return;");
                builder.AppendLine("                    }");
            }

            builder.AppendLine("                    break;");
        }

        builder.AppendLine("            }");
        foreach (var member in fallback)
        {
            builder.AppendLine($"            if (key.Length == {member.formatName.Length} && PeroTools2.Serde.StringHelper.Equals(key, \"{member.formatName}\"))");
            builder.AppendLine("            {");
            builder.AppendLine($"                obj.{member.name} = {MemberReadStatement(member)};");
            builder.AppendLine("                return;");
            builder.AppendLine("            }");
        }

        builder.AppendLine("            reader.ReadUntilEnd();");
    }

    /// <summary>Emits the reader call that parses <paramref name="member"/>.</summary>
    /// <remarks>
    /// Primitives, and arrays or lists of them, call the reader's per-type parse directly. Going through the formatter
    /// registry for every member costs a cache lookup and an interface call per value, which is what an emit-based
    /// serializer avoids.
    /// </remarks>
    private static string MemberReadStatement(MemberMeta member)
    {
        if (member.formatter == null)
        {
            var direct = DirectReaderMethod(member.type);
            if (direct != null)
            {
                return $"reader.{direct}(false)";
            }

            var collection = DirectCollectionReader(member);
            if (collection != null)
            {
                return $"reader.{collection}(false)";
            }
        }

        var formatter = member.useFormatter ? $"s_{member.name}Formatter, " : string.Empty;
        return $"reader.ReadValue<{member.type.ToDisplayString()}>({formatter}false)";
    }

    /// <summary>Maps a primitive member type onto the reader method that parses it without a formatter.</summary>
    private static string? DirectReaderMethod(ITypeSymbol type) =>
        type.SpecialType switch
        {
            SpecialType.System_Int32 => "ReadInt32Value",
            SpecialType.System_Int64 => "ReadInt64Value",
            SpecialType.System_Int16 => "ReadInt16Value",
            SpecialType.System_UInt16 => "ReadUInt16Value",
            SpecialType.System_Byte => "ReadByteValue",
            SpecialType.System_SByte => "ReadSByteValue",
            SpecialType.System_UInt32 => "ReadUInt32Value",
            SpecialType.System_UInt64 => "ReadUInt64Value",
            SpecialType.System_Single => "ReadSingleValue",
            SpecialType.System_Double => "ReadDoubleValue",
            SpecialType.System_Decimal => "ReadDecimalValue",
            SpecialType.System_Boolean => "ReadBooleanValue",
            SpecialType.System_Char => "ReadCharValue",
            SpecialType.System_String => "ReadStringValue",
            _ => null
        };

    /// <summary>
    /// Maps an array or <see cref="System.Collections.Generic.List{T}"/> member of a primitive element type onto the
    /// reader method that parses the whole collection, or null when the member has to go through its formatter.
    /// </summary>
    private static string? DirectCollectionReader(MemberMeta member)
    {
        ITypeSymbol elementType;
        bool isArray;
        switch (member.kind)
        {
            case BuiltinKind.Array when member.type is IArrayTypeSymbol arrayType:
                elementType = arrayType.ElementType;
                isArray = true;
                break;
            case BuiltinKind.Collection
                when member.type is INamedTypeSymbol { Name: "List", TypeArguments.Length: 1 } named
                     && named.ContainingNamespace?.ToDisplayString() == "System.Collections.Generic":
                elementType = named.TypeArguments[0];
                isArray = false;
                break;
            default:
                return null;
        }

        var direct = DirectReaderMethod(elementType);
        if (direct == null)
        {
            return null;
        }

        // ReadStringValue -> ReadStringArray / ReadStringList
        return direct.Substring(0, direct.Length - "Value".Length) + (isArray ? "Array" : "List");
    }

    /// <summary>Packs four characters of a member name into a <see cref="ulong"/>, exactly like StringHelper.GetKeyPrefix.</summary>
    private static ulong PackPrefix(string name, int start)    {
        ulong value = 0;
        for (var i = 0; i < 4 && start + i < name.Length; i++)
        {
            value |= (ulong) name[start + i] << (i * 16);
        }
        return value;
    }

    // Only pure ASCII member names may use the generated byte/char length guard for the key comparison
    private static bool IsSimpleKey(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        foreach (var c in name)
        {
            if (c < 0x20 || c > 0x7e || c == '"' || c == '\\')
            {
                return false;
            }
        }

        return true;
    }

    private static string GenerateRecordsSerialization(ITypeSymbol type, TypeMeta typeMeta)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var members = typeMeta.members;

        // Add member serialization writes
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
            // When null members are ignored the member count is only known at runtime, so the leading-comma pattern is used:
            // before writing each member, append a comma if a member was already written, which avoids a trailing comma.
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

        // Generate the partial serializer
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
