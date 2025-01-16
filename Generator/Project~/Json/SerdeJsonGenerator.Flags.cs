/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/06-15:01:55
 */

using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace PeroTools2.Serde.Generator;

public partial class SerdeJsonGenerator
{
    private string GenerateFlagsFormatter(ITypeSymbol type, TypeMeta typeMeta)
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
            // 自定义的 string 或 number 序列化器
            sb.AppendLine(GenerateFlagsSerialization(type, typeMeta, serializationFormat == 1));
            sb.Append(GenerateFlagsDeserialization(type, typeMeta, deserializationFormat == 1));
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

    private string GenerateFlagsSerialization(ITypeSymbol type, TypeMeta meta, bool isNumberSerialize)
    {
        var typeName = type.Name;
        var sb = new StringBuilder();
        if (isNumberSerialize)
        {
            sb.Append("writer.WriteValue((long) value);");
        }
        else
        {
            sb.Append("var hasValue = false;\n");
            sb.AppendLine("        writer.WriteDoubleQuote();");

            var array = type.GetMembers().OfType<IFieldSymbol>().ToArray();
            for (var index = 0; index < array.Length; index++)
            {
                var member = array[index];
                var memberValue = $"{meta.members.FirstOrDefault(m => m.name == member.Name).formatName ?? member.Name}";
                sb.AppendLine($"        if ((value & {type.ToDisplayString()}.{member.Name}) != 0)");
                sb.AppendLine("        {");
                if (index != 0)
                {
                    sb.AppendLine("            if (hasValue) writer.WriteValueEnding();");
                }
                if (index != array.Length - 1)
                {
                    sb.AppendLine("            hasValue = true;");
                }
                sb.AppendLine($"            writer.WriteStringWithoutQuote(\"{memberValue}\");");
                sb.AppendLine("        }");
            }
            sb.AppendLine("        writer.WriteDoubleQuote();");
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

    private string GenerateFlagsDeserialization(ITypeSymbol type, TypeMeta meta, bool isNumberDeserialize)
    {
        var typeName = type.Name;
        var sbUtf8 = new StringBuilder();
        var sbUtf16 = new StringBuilder();
        const string code1 =
            """
            var result = default({0});
                    var value = reader.ReadValue();
                    var startIndex = 0;
                    var maxIndex = value.Length - 1;
                    for (var i = 0; i < value.Length; i++)
                    {{
                        var character = {1}value[i];
                        if (PeroTools2.Serde.StringHelper.IsWhiteSpace(character))
                        {{
                            startIndex = i + 1;
                            continue;
                        }}
                    
                        if (character != ',' && i != maxIndex) continue;
            """;
        const string code2 =
            """
            
                        startIndex = i + 1;
                    }
                    return result;
            """;
        if (isNumberDeserialize)
        {
            sbUtf8.Append($"return ({type.ToDisplayString()}) reader.ReadValue<long>();");
            sbUtf16.Append($"return ({type.ToDisplayString()}) reader.ReadValue<long>();");
        }
        else
        {
            sbUtf8.AppendLine(string.Format(code1, type.ToDisplayString(), "(byte) "));
            sbUtf16.AppendLine(string.Format(code1, type.ToDisplayString(), ""));
            var array = type.GetMembers().OfType<IFieldSymbol>().ToArray();
            for (var index = 0; index < array.Length; index++)
            {
                var member = array[index];
                // 直接比较 UTF8 和 UTF16 字符串，避免 Allocated.
                var memberMeta = meta.members.FirstOrDefault(m => m.name == member.Name);
                sbUtf8.AppendLine(
                    $"            {(index == 0 ? "if" : "else if")} (PeroTools2.Serde.StringHelper.EqualsUnaligned(value.Slice(startIndex, {memberMeta.formatName.Length}), \"{memberMeta.formatName}\")) result |= {type.ToDisplayString()}.{member.Name};");
                sbUtf16.AppendLine(
                    $"            {(index == 0 ? "if" : "else if")} (PeroTools2.Serde.StringHelper.Equals(value.Slice(startIndex, {memberMeta.formatName.Length}), \"{memberMeta.formatName}\")) result |= {type.ToDisplayString()}.{member.Name};");
            }
            sbUtf8.AppendLine(code2);
            sbUtf16.AppendLine(code2);
            sbUtf16.Remove(sbUtf16.Length - 1, 1);
            sbUtf8.Remove(sbUtf8.Length - 1, 1);
        }

        return $$"""
                  {{type.GetAccessibilityString()}} sealed partial class {{typeName}}JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{type.ToDisplayString()}}>
                  {
                      [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                      public {{type.ToDisplayString()}} DeserializeUtf8(ref System.ReadOnlySpan<byte> remains, ref PeroTools2.Serde.Json.JsonReaderUtf8 reader)
                      {
                          {{sbUtf8}}
                      }

                      [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                      public {{type.ToDisplayString()}} DeserializeUtf16(ref System.ReadOnlySpan<char> remains, ref PeroTools2.Serde.Json.JsonReaderUtf16 reader)
                      {
                          {{sbUtf16}}
                      }
                  }
                  """;
    }
}