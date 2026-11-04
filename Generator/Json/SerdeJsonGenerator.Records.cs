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
            // If the Json property is redundant, read it directly
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

        // P1 prototype: when all members are builtin primitive types, use the new cursor + static direct-call reader
        var utf8OldBody = new StringBuilder();
        utf8OldBody.AppendLine("            System.ReadOnlySpan<byte> key;");
        utf8OldBody.Append(deserializerBuilder);
        var utf8Body = utf8OldBody.ToString();
        if (members.Length > 0 && members.All(member => TryGetCursorMemberAccess(member, out _)))
        {
            var cursorBody = BuildCursorDeserializeBody(type, members);
            if (cursorBody != null)
            {
                // An unpinned reader (the caller creates one and then calls the formatter) falls back to the old form
                utf8Body = cursorBody;
            }
        }

        var utf16OldBody = new StringBuilder();
        utf16OldBody.AppendLine("            System.ReadOnlySpan<char> key;");
        utf16OldBody.Append(deserializerBuilder);
        var utf16Body = utf16OldBody.ToString();
        if (members.Length > 0 && members.All(member => TryGetCursorMemberAccessUtf16(member, out _)))
        {
            var cursorBody = BuildCursorDeserializeBodyUtf16(type, members);
            if (cursorBody != null)
            {
                utf16Body = cursorBody;
            }
        }

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

    // Whether this is a fixed formatter type built into this package (already registered in the FixedCache of SerdeJsonFormatterProvider)
    // Generation-time prefix matching the runtime StringHelper.GetKeyPrefix (ASCII assumption, consistent with the existing equality comparison)
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

    // Only for member names that are pure ASCII and contain neither " nor \ do the generation-time constants match the runtime prefix/length semantics

    private static string IndentBody(string body, string indent)
    {
        var builder = new StringBuilder();
        foreach (var line in body.Split('\n'))
        {
            var trimmed = line.TrimEnd('\r');
            if (trimmed.Length == 0)
            {
                builder.AppendLine();
                continue;
            }

            builder.Append(indent).AppendLine(trimmed);
        }

        return builder.ToString();
    }
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

    // Cursor read expression for a member (including the ref cursor); returns false when unsupported
    private static bool TryGetCursorMemberAccess(MemberMeta member, out string expression)
    {
        expression = null;

        // For non-ASCII or characters that need escaping the generation-time constant does not match the runtime prefix/length, so fall back to the old path
        if (!IsSimpleKey(member.formatName))
        {
            return false;
        }

        var type = member.type;
        const string ns = "global::PeroTools2.Serde.Json";

        // Members with an explicit [SerdeJsonFormatter]: the formatter is not in the provider registry, so read with the instance attached to that member
        if (member.formatter != null)
        {
            expression = $"{ns}.JsonReader.ReadNested<{type.ToDisplayString()}>(ref cursor, s_{member.name}Formatter)";
            return true;
        }

        if (type.GetAttributes().Any(attribute => attribute.AttributeClass?.Name == "SerdeJsonSerializableAttribute"))
        {
            expression = $"{ns}.JsonReader.ReadNested<{type.ToDisplayString()}>(ref cursor)";
            return true;
        }

        // enum / Nullable / collection / builtin struct, etc.: the formatter is already in the registry, so read through the reader
        // Members with an explicit [SerdeJsonFormatter] are not registered, so they must take the original path
        if (member.useFormatter && member.formatter == null)
        {
            expression = $"{ns}.JsonReader.ReadNested<{type.ToDisplayString()}>(ref cursor)";
            return true;
        }

        if (TryGetCursorReader(type, out var readerName))
        {
            expression = $"{ns}.JsonReader.{readerName}(ref cursor)";
            return true;
        }

        return false;
    }
    // Member types supported by the cursor path (primitive types + string + char); returns the matching reader name
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

    // Cursor-style deserialization body (P1 prototype): generated only when the prefix+length pair is unique
    // UTF-16 version of the member read expression
    private static bool TryGetCursorMemberAccessUtf16(MemberMeta member, out string expression)
    {
        expression = null;

        // For non-ASCII or characters that need escaping the generation-time constant does not match the runtime prefix/length, so fall back to the old path
        if (!IsSimpleKey(member.formatName))
        {
            return false;
        }

        var type = member.type;
        const string ns = "global::PeroTools2.Serde.Json";

        if (member.formatter != null)
        {
            expression = $"{ns}.JsonReader.ReadNested<{type.ToDisplayString()}>(ref cursor, s_{member.name}Formatter)";
            return true;
        }

        if (type.GetAttributes().Any(attribute => attribute.AttributeClass?.Name == "SerdeJsonSerializableAttribute"))
        {
            expression = $"{ns}.JsonReader.ReadNested<{type.ToDisplayString()}>(ref cursor)";
            return true;
        }

        if (member.useFormatter && member.formatter == null)
        {
            expression = $"{ns}.JsonReader.ReadNested<{type.ToDisplayString()}>(ref cursor)";
            return true;
        }

        if (TryGetCursorReader(type, out var readerName))
        {
            expression = $"{ns}.JsonReader.{readerName}(ref cursor)";
            return true;
        }

        return false;
    }

    // Pack the first 8 characters of the key into two ulongs (4 characters each)
    private static void GetKeyPrefixUtf16(string key, out ulong low, out ulong high)
    {
        low = 0;
        high = 0;
        var first = key.Length < 4 ? key.Length : 4;
        for (var i = 0; i < first; i++)
        {
            low |= (ulong) key[i] << (i * 16);
        }

        var second = key.Length - 4;
        if (second > 4) second = 4;
        for (var i = 0; i < second; i++)
        {
            high |= (ulong) key[i + 4] << (i * 16);
        }
    }

    private static string BuildCursorDeserializeBodyUtf16(ITypeSymbol type, MemberMeta[] members)
    {
        var seen = new System.Collections.Generic.HashSet<(ulong, ulong)>();
        foreach (var member in members)
        {
            GetKeyPrefixUtf16(member.formatName, out var low, out var high);
            if (!TryGetCursorMemberAccessUtf16(member, out _))
            {
                return null;
            }

            if (!seen.Add((low, high)))
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
        body.AppendLine("            cursor.Expect('{');");
        body.AppendLine("            while (cursor.TryReadKey(out var keyLength, out var prefixLow, out var prefixHigh))");
        body.AppendLine("            {");
        body.AppendLine("                switch (prefixLow, prefixHigh)");
        body.AppendLine("                {");
        foreach (var member in members)
        {
            TryGetCursorMemberAccessUtf16(member, out var memberAccess);
            GetKeyPrefixUtf16(member.formatName, out var low, out var high);
            body.AppendLine($"                    case (0x{low:X16}UL, 0x{high:X16}UL):");
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
        body.AppendLine("                cursor.TryConsume(',');");
        body.AppendLine("            }");
        body.AppendLine();
        body.AppendLine("            cursor.Expect('}');");
        body.AppendLine("            reader.SyncCursor(in cursor);");
        body.AppendLine("            return obj;");
        return body.ToString();
    }
    private static string BuildCursorDeserializeBody(ITypeSymbol type, MemberMeta[] members)
    {
        // switch labels are only the prefix: if two keys share the first 8 bytes the whole approach must be abandoned, otherwise duplicate cases are generated
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
