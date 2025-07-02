/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/03-14:42:21
 */

using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace PeroTools2.Serde.Generator;

public partial class SerdeJsonGenerator
{
    private string GenerateEnumsFormatter(ITypeSymbol type, TypeMeta typeMeta)
    {
        var ns = type.ContainingNamespace;
        var sb = new StringBuilder();
        if (ns is { IsGlobalNamespace: false })
        {
            sb.AppendLine($"namespace {ns}\n{{");
        }
        if (type.TryGetAttribute(m_ReferenceSymbols.serdeEnumFormatterAttribute, out var attr))
        {
            var serializationFormat = (int) attr!.ConstructorArguments[0].Value!;
            var deserializationFormat = (int) attr!.ConstructorArguments[1].Value!;
            // 非自定义的 Number 格式化器
            if (serializationFormat == 1 && deserializationFormat == 1)
            {
                sb.AppendLine($"{type.GetAccessibilityString()} sealed class {type.Name}JsonFormatter : PeroTools2.Serde.Json.EnumNumberFormatter<{type.ToDisplayString()}> {{ }}");
                if (ns is { IsGlobalNamespace: false })
                {
                    sb.AppendLine("}");
                }
                return sb.ToString();
            }
            // 非自定义的 String 格式化器
            if (serializationFormat == 0 && deserializationFormat == 0 && typeMeta.members.All(static m => m.formatName == m.name))
            {
                sb.AppendLine($"{type.GetAccessibilityString()} sealed class {type.Name}JsonFormatter : PeroTools2.Serde.Json.EnumStringFormatter<{type.ToDisplayString()}> {{ }}");
                if (ns is { IsGlobalNamespace: false })
                {
                    sb.AppendLine("}");
                }
                return sb.ToString();
            }
            // 自定义的 string 或 number 序列化器
            sb.AppendLine(GenerateEnumsSerialization(type, typeMeta, serializationFormat == 1));
            sb.Append(GenerateEnumsDeserialization(type, typeMeta, deserializationFormat == 1));
        }
        else
        {
            sb.AppendLine($"{type.GetAccessibilityString()} sealed class {type.Name}JsonFormatter : PeroTools2.Serde.Json.EnumNumberFormatter<{type.ToDisplayString()}> {{ }}");
        }
        if (ns is { IsGlobalNamespace: false })
        {
            sb.AppendLine("}");
        }
        return sb.ToString();
    }

    private static string GenerateEnumsDeserialization(ITypeSymbol type, TypeMeta meta, bool isNumberDeserialize)
    {
        var typeName = type.Name;
        var sbUtf8 = new StringBuilder();
        var sbUtf16 = new StringBuilder();
        if (isNumberDeserialize)
        {
            sbUtf8.Append($"return ({type.ToDisplayString()}) reader.ReadValue<long>();");
            sbUtf16.Append($"return ({type.ToDisplayString()}) reader.ReadValue<long>();");
        }
        else
        {
            sbUtf8.Append("var result = reader.ReadValue();\n");
            sbUtf16.Append("var result = reader.ReadValue();\n");
            foreach (var member in type.GetMembers().OfType<IFieldSymbol>())
            {
                // 直接比较 UTF8 和 UTF16 字符串，避免 Allocated.
                var memberMeta = meta.members.FirstOrDefault(m => m.name == member.Name);
                sbUtf8.AppendLine($"        if (PeroTools2.Serde.StringHelper.EqualsUnaligned(result, \"{memberMeta.formatName}\")) return {type.ToDisplayString()}.{member.Name};");
                sbUtf16.AppendLine($"        if (PeroTools2.Serde.StringHelper.Equals(result, \"{memberMeta.formatName}\")) return {type.ToDisplayString()}.{member.Name};");
            }
            sbUtf8.AppendLine("        throw new System.ArgumentOutOfRangeException(nameof(value), System.Text.Encoding.UTF8.GetString(result), null);");
            sbUtf16.AppendLine("        throw new System.ArgumentOutOfRangeException(nameof(value), result.ToString(), null);");
            sbUtf16.Remove(sbUtf16.Length - 1, 1);
            sbUtf8.Remove(sbUtf8.Length - 1, 1);
        }

        return $$"""
                 {{type.GetAccessibilityString()}} sealed partial class {{typeName}}JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{type.ToDisplayString()}}>
                 {
                     [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                     public {{type.ToDisplayString()}} DeserializeUtf8(ref System.ReadOnlySpan<byte> value, ref PeroTools2.Serde.Json.JsonReaderUtf8 reader)
                     {
                         {{sbUtf8}}
                     }

                     [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                     public {{type.ToDisplayString()}} DeserializeUtf16(ref System.ReadOnlySpan<char> value, ref PeroTools2.Serde.Json.JsonReaderUtf16 reader)
                     {
                         {{sbUtf16}}
                     }
                 }
                 """;
    }

    private static string GenerateEnumsSerialization(ITypeSymbol type, TypeMeta meta, bool isNumberSerialize)
    {
        var typeName = type.Name;
        var sb = new StringBuilder();
        if (isNumberSerialize)
        {
            sb.Append("writer.WriteValue((long) value);");
        }
        else
        {
            sb.Append("switch (value)\n");
            sb.AppendLine("        {");

            foreach (var member in type.GetMembers().OfType<IFieldSymbol>())
            {
                var memberValue = $"\"{meta.members.FirstOrDefault(m => m.name == member.Name).formatName ?? member.Name}\"";
                sb.AppendLine($"            case {type.Name}.{member.Name}: writer.WriteValue({memberValue}); break;");
            }
            sb.AppendLine("            default: throw new System.ArgumentOutOfRangeException(nameof(value), value, null);");
            sb.AppendLine("        }");
            sb.Remove(sb.Length - 1, 1);
        }
        return $$"""
                 {{type.GetAccessibilityString()}} sealed partial class {{typeName}}JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{type.ToDisplayString()}}>
                 {
                     [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                     public void SerializeUtf8({{type.ToDisplayString()}} value, ref PeroTools2.Serde.Json.JsonWriterUtf8 writer)
                     {
                         {{sb}}
                     }

                     [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                     public void SerializeUtf16({{type.ToDisplayString()}} value, ref PeroTools2.Serde.Json.JsonWriterUtf16 writer)
                     {
                         {{sb}}
                     }
                 }
                 """;
    }
}