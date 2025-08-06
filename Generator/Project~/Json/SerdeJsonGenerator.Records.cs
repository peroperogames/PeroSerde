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
        var ignoreMissingMember = typeSymbol.TryGetAttribute(m_ReferenceSymbols.serdeJsonSerializableAttribute, out var attributeData)
            ? (bool) attributeData!.ConstructorArguments[0].Value!
            : true;
        if (ns is { IsGlobalNamespace: false })
        {
            sb.AppendLine($"namespace {ns} {{");
        }
        sb.AppendLine(GenerateRecordsFormatterRegistry(typeSymbol, typeMeta));
        sb.AppendLine(GenerateRecordsSerialization(typeSymbol, typeMeta));
        sb.Append(GenerateRecordsDeserialization(typeSymbol, typeMeta, ignoreMissingMember));
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
            if (member.kind is BuiltinKind.Collection or BuiltinKind.Array or BuiltinKind.ReadonlyCollection
                && member.type.TryGetCollectionElementType(out var elementType))
            {
                if (m_TypeSymbols.Any(kv => SymbolEqualityComparer.Default.Equals(kv.Key, elementType)))
                {
                    formatterRegistry.AppendLine($"            if (!global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.IsRegistered<{elementType.ToDisplayString()}>())");
                    formatterRegistry.AppendLine("            {");
                    formatterRegistry.AppendLine($"                {elementType.ToDisplayString()}.RegisterFormattersFor{elementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}();");
                    formatterRegistry.AppendLine("            }");
                }
                else if (elementType.ContainsAttribute(m_ReferenceSymbols.serdeJsonSerializableAttribute))
                {
                    formatterRegistry.AppendLine($"            if (!global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.IsRegistered<{elementType.ToDisplayString()}>())");
                    formatterRegistry.AppendLine("            {");
                    formatterRegistry.AppendLine($"                {elementType.ToDisplayString()}.RegisterFormattersFor{elementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}();");
                    formatterRegistry.AppendLine("            }");
                }
                else if (TryGetFormatterType(new MemberMeta
                         {
                             type = elementType,
                             kind = TypeMeta.GetMemberKind(elementType, m_ReferenceSymbols)
                         }, out var elementFormatter))
                {
                    formatterRegistry.AppendLine($"            if (!global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.IsRegistered<{elementType.ToDisplayString()}>())");
                    formatterRegistry.AppendLine("            {");
                    formatterRegistry.AppendLine($"                global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.Register(new {elementFormatter}());");
                    formatterRegistry.AppendLine("            }");
                }
            }
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
                ? string.Empty
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

        static bool TryGetFormatterType(MemberMeta member, out string formatter)
        {
            formatter = member switch
            {
                { formatter: not null } => member.formatter.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),

                { kind: BuiltinKind.Enum } => $"{member.type.ToDisplayString()}JsonFormatter",

                { kind: BuiltinKind.Array, type: IArrayTypeSymbol arrayType } => $"PeroTools2.Serde.Json.ArrayFormatter<{arrayType.ElementType.ToDisplayString()}>",

                { type: INamedTypeSymbol namedTypeSymbol } => namedTypeSymbol switch
                {
                    _ when member.kind is BuiltinKind.Collection &&
                           namedTypeSymbol.TryGetCollectionElementType(out var elementType)
                        => $"PeroTools2.Serde.Json.CollectionFormatter<{member.type.ToDisplayString()}, {elementType.ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.ReadonlyCollection &&
                           namedTypeSymbol.TryGetCollectionElementType(out var elementType)
                        => $"PeroTools2.Serde.Json.ReadonlyCollectionFormatter<{member.type.ToDisplayString()}, {elementType.ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.Nullable
                        => $"PeroTools2.Serde.Json.NullableFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.Lazy
                        => $"PeroTools2.Serde.Json.LazyFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}>",

                    _ when member.kind is BuiltinKind.KeyValuePair
                        => $"PeroTools2.Serde.Json.KeyValuePairFormatter<{namedTypeSymbol.TypeArguments[0].ToDisplayString()}, {namedTypeSymbol.TypeArguments[1].ToDisplayString()}>",

                    _ => string.Empty
                },
                _ => string.Empty
            };
            return !string.IsNullOrEmpty(formatter);
        }
    }

    private static string GenerateRecordsDeserialization(ITypeSymbol type, TypeMeta typeMeta, bool ignoreMissingMember)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var members = typeMeta.members;

        // 添加成员序列化写入
        var deserializerBuilder = new StringBuilder();
        var utf16Builder = new StringBuilder();
        var utf8Builder = new StringBuilder();
        deserializerBuilder.AppendLine($"var obj = new {typeName}();");
        deserializerBuilder.AppendLine("            reader.ReadBeginObject();");
        if (ignoreMissingMember)
        {
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var readComma = (index == members.Length - 1).ToString().ToLower();
                var memberTypeName = member.type.ToDisplayString();
                var formatter = member.useFormatter ? $"s_{member.name}Formatter, " : string.Empty;
                deserializerBuilder.AppendLine($$"""
                                                           if (reader.TryReadKey(out key))
                                                               OnReadKey(ref key, ref obj, ref reader, {{readComma}});
                                                           else
                                                           {
                                                               reader.ReadEndObject();
                                                               return obj;
                                                           }
                                               
                                               """);
                utf8Builder.AppendLine($"            {(index == 0 ? "if" : "else if")} (PeroTools2.Serde.StringHelper.EqualsUnaligned(key, \"{member.formatName}\"))");
                utf16Builder.AppendLine($"            {(index == 0 ? "if" : "else if")} (PeroTools2.Serde.StringHelper.Equals(key, \"{member.formatName}\"))");
                var memberReadStr = $$"""
                                                  {
                                                      obj.{{member.name}} = reader.ReadValue<{{memberTypeName}}>({{formatter}}false);
                                                      if(!isEnd) reader.TryReadComma();
                                                  }
                                      """;
                utf16Builder.AppendLine(memberReadStr);
                utf8Builder.AppendLine(memberReadStr);
            }
        }
        else
        {
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var readComma = (index == members.Length - 1).ToString().ToLower();
                var memberTypeName = member.type.ToDisplayString();
                var formatter = member.useFormatter ? $"s_{member.name}Formatter, " : string.Empty;
                deserializerBuilder.AppendLine("            key = reader.ReadKey();");
                deserializerBuilder.AppendLine($"            OnReadKey(ref key, ref obj, ref reader, {readComma});");
                utf8Builder.AppendLine($"            {(index == 0 ? "if" : "else if")} (PeroTools2.Serde.StringHelper.EqualsUnaligned(key, \"{member.formatName}\"))");
                utf16Builder.AppendLine($"            {(index == 0 ? "if" : "else if")} (PeroTools2.Serde.StringHelper.Equals(key, \"{member.formatName}\"))");
                var memberReadStr = $"                obj.{member.name} = reader.ReadValue<{memberTypeName}>({formatter}isEnd);";
                utf16Builder.AppendLine(memberReadStr);
                utf8Builder.AppendLine(memberReadStr);
            }
        }

        deserializerBuilder.AppendLine("            reader.ReadEndObject();");
        deserializerBuilder.Append("            return obj;");
        if (utf8Builder.Length > 0) utf8Builder.Remove(utf8Builder.Length - 1, 1);
        if (utf16Builder.Length > 0) utf16Builder.Remove(utf16Builder.Length - 1, 1);

        // 生成分部序列化器
        return $$"""
                 // Deserializer
                 partial {{type.GetTypeKind()}} {{typeName}}
                 {
                     public partial class JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{type.ToDisplayString()}}>
                     {
                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         public {{type.ToDisplayString()}} DeserializeUtf8(ref System.ReadOnlySpan<byte> value, ref PeroTools2.Serde.Json.JsonReaderUtf8 reader)
                         {
                             System.ReadOnlySpan<byte> key;
                             {{deserializerBuilder}}
                         }

                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         public {{typeName}} DeserializeUtf16(ref System.ReadOnlySpan<char> value, ref PeroTools2.Serde.Json.JsonReaderUtf16 reader)
                         {
                             System.ReadOnlySpan<char> key;
                             {{deserializerBuilder}}
                         }

                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         private void OnReadKey(ref System.ReadOnlySpan<byte> key, ref {{type.ToDisplayString()}} obj, ref PeroTools2.Serde.Json.JsonReaderUtf8 reader, bool isEnd)
                         {
                 {{utf8Builder}}
                         }

                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         private void OnReadKey(ref System.ReadOnlySpan<char> key, ref {{type.ToDisplayString()}} obj, ref PeroTools2.Serde.Json.JsonReaderUtf16 reader, bool isEnd)
                         {
                 {{utf16Builder}}
                         }
                     }
                 }
                 """;
    }

    private static string GenerateRecordsSerialization(ITypeSymbol type, TypeMeta typeMeta)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var members = typeMeta.members;

        // 添加成员序列化写入
        var stringBuilder = new StringBuilder();
        if (type.IsNullable())
        {
            stringBuilder.AppendLine("if (value == null)");
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
        for (var index = 0; index < members.Length; index++)
        {
            var member = members[index];
            var writeComma = (index != members.Length - 1).ToString().ToLower();
            var args = member.useFormatter
                ? $"\"{member.formatName}\", value.{member.name}, s_{member.name}Formatter, {writeComma}"
                : $"\"{member.formatName}\", value.{member.name}, {writeComma}";
            stringBuilder.AppendLine($"            writer.WriteNamedValue({args});");
        }
        stringBuilder.Append("            writer.WriteEndObject();");

        // 生成分部序列化器
        return $$"""
                 // Serializer
                 partial {{type.GetTypeKind()}} {{typeName}}
                 {
                     public partial class JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{type.ToDisplayString()}}>
                     {
                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         public void SerializeUtf16({{type.ToDisplayString()}} value, ref PeroTools2.Serde.Json.JsonWriterUtf16 writer)
                         {
                             {{stringBuilder}}
                         }

                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         public void SerializeUtf8({{type.ToDisplayString()}} value, ref PeroTools2.Serde.Json.JsonWriterUtf8 writer)
                         {
                             {{stringBuilder}}
                         }
                     }
                 }
                 """;
    }
}