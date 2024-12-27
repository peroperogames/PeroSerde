/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/09/09 13:09:49
 */

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PeroTools2.Serde.Generator;

public partial class SerdeJsonGenerator
{
    private static readonly SymbolDisplayFormat s_TypeFullNameDisplayFormat = new(
        SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
    );

    private void Generate(ref GeneratorExecutionContext context, TypeDeclarationSyntax typeSyntax)
    {
        var semanticModel = context.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);
        var typeSymbol = semanticModel.GetDeclaredSymbol(typeSyntax, context.CancellationToken);
        if (typeSymbol == null) return;
        var ns = typeSymbol.ContainingNamespace;
        var typeFullName = typeSymbol.ToDisplayString(s_TypeFullNameDisplayFormat);

        var typeMeta = new TypeMeta(typeSymbol, m_ReferenceSymbols);

        var sb = new StringBuilder();
        if (ns != null && !ns.IsGlobalNamespace)
        {
            sb.AppendLine($"namespace {ns} {{");
        }
        sb.AppendLine(GenerateFormatter(typeSymbol, typeMeta));
        sb.AppendLine(GenerateSerializationCodes(typeSymbol, typeMeta));
        sb.Append(GenerateDeserializationCodes(typeSymbol, typeMeta));
        if (ns != null && !ns.IsGlobalNamespace)
        {
            sb.AppendLine("}");
        }
        var src = sb.ToString();
        context.AddSource($"{typeFullName}.SerdeJsonSerializer.g.cs", src);
    }

    private string GenerateFormatter(INamedTypeSymbol type, TypeMeta typeMeta)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var members = typeMeta.members;
        var formatterBuilder = new StringBuilder();
        var formatterRegistry = new StringBuilder();
        foreach (var member in members)
        {
            var field = string.Empty;
            if (member.formatter is (not null, not null))
            {
                var formatter1 = member.formatter.Value.Item1.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var forammter2 = member.formatter.Value.Item2.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                field = $"private readonly static ({formatter1}, {forammter2}) s_{member.name}Formatter = (new {formatter1}(), new {forammter2}());";
            }
            else if (member.formatter is (not null, null))
            {
                var formatter = member.formatter.Value.Item1.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                field = $"private readonly static {formatter} s_{member.name}Formatter = new {formatter}();";
            }
            else
            {
                continue;
            }
            formatterBuilder.AppendLine($"    {field}");
        }
        return $$"""
                 partial {{type.GetTypeKind()}} {{typeName}}
                 {
                 {{formatterBuilder}}
                     [global::UnityEngine.RuntimeInitializeOnLoadMethod(global::UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
                     private static void RegisterFormattersFor{{typeName}}()
                     {
                         global::PeroTools2.Serde.Json.SerdeJsonFormatterProvider.Register(new {{typeName}}JsonFormatter());
                     }
                 }
                 """;
    }

    private string GenerateDeserializationCodes(INamedTypeSymbol type, TypeMeta typeMeta)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var members = typeMeta.members;

        // 添加成员序列化写入
        var deserializerBuilder = new StringBuilder();
        var utf16Builder = new StringBuilder();
        var utf8Builder = new StringBuilder();
        deserializerBuilder.Append($"var obj = new {typeName}();");
        deserializerBuilder.AppendLine("reader.ReadBeginObject();");
        for (var index = 0; index < members.Length; index++)
        {
            var member = members[index];
            var readComma = (index != members.Length - 1).ToString().ToLower();
            var memberTypeName = member.type.ToDisplayString();
            var formatter = member.formatter == null ? string.Empty : $", s_{member.name}Formatter";
            deserializerBuilder.AppendLine("            key = reader.ReadKey();");
            deserializerBuilder.AppendLine($"            OnReadKey(ref key, ref obj, ref reader, {readComma});");
            utf8Builder.AppendLine($"            if (PeroTools2.Serde.StringHelper.EqualsUnaligned(key, \"{member.formatName}\"))");
            utf16Builder.AppendLine($"            if (PeroTools2.Serde.StringHelper.Equals(key, \"{member.formatName}\"))");

            var memberAssign = string.Empty;
            if (member.type is IArrayTypeSymbol arrayTypeSymbol)
            {
                memberAssign = $"                obj.{member.name} = reader.ReadArray<{arrayTypeSymbol.ElementType.ToDisplayString()}>({readComma}{formatter});";
            }
            else if (member.kind is MemberKind.List or MemberKind.ReadonlyList && member.type is INamedTypeSymbol listSymbol && listSymbol.TryGetListElementType(out var elementType))
            {
                memberAssign = $"                obj.{member.name} = reader.ReadCollection<{memberTypeName}, {elementType.ToDisplayString()}>({readComma}{formatter});";
            }
            else if (member.kind is MemberKind.Dictionary or MemberKind.ReadonlyDictionary && member.type is INamedTypeSymbol dictSymbol &&
                     dictSymbol.TryGetDictionaryKeyValueTypes(out var keyType, out var valueType))
            {
                memberAssign = $"                obj.{member.name} = reader.ReadDictionary<{memberTypeName}, {keyType.ToDisplayString()}, {valueType.ToDisplayString()}>({readComma}{formatter});";
            }
            else if (member.kind is MemberKind.Value)
            {
                memberAssign = $"                obj.{member.name} = reader.ReadValue<{memberTypeName}>({readComma}{formatter});";
            }
            else
            {
                continue;
            }
            utf16Builder.AppendLine(memberAssign);
            utf8Builder.AppendLine(memberAssign);
        }

        deserializerBuilder.AppendLine("            reader.ReadEndObject();");
        deserializerBuilder.Append("            return obj;");

        // 生成分部序列化器
        return $$"""
                 // Deserializer
                 partial {{type.GetTypeKind()}} {{typeName}}
                 {
                     private partial class {{typeName}}JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{typeName}}>
                     {
                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         public {{typeName}} DeserializeUtf8(ref System.ReadOnlySpan<byte> value, ref PeroTools2.Serde.Json.JsonReaderUtf8 reader)
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
                         private void OnReadKey(ref System.ReadOnlySpan<byte> key, ref {{typeName}} obj, ref PeroTools2.Serde.Json.JsonReaderUtf8 reader, bool isEnd)
                         {
                 {{utf8Builder}}
                         }

                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         private void OnReadKey(ref System.ReadOnlySpan<char> key, ref {{typeName}} obj, ref PeroTools2.Serde.Json.JsonReaderUtf16 reader, bool isEnd)
                         {
                 {{utf16Builder}}
                         }
                     }
                 }
                 """;
    }

    private string GenerateSerializationCodes(INamedTypeSymbol type, TypeMeta typeMeta)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var members = typeMeta.members;

        // 添加成员序列化写入
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("writer.WriteBeginObject();");
        for (var index = 0; index < members.Length; index++)
        {
            var member = members[index];
            var writeComma = (index != members.Length - 1).ToString().ToLower();
            var args = member.formatter == null
                ? $"\"{member.formatName}\", value.{member.name}, {writeComma}"
                : member.kind is MemberKind.Dictionary or MemberKind.ReadonlyDictionary
                    ? $"\"{member.formatName}\", value.{member.name}, s_{member.name}Formatter.Item1, s_{member.name}Formatter.Item2, {writeComma}"
                    : $"\"{member.formatName}\", value.{member.name}, s_{member.name}Formatter, {writeComma}";

            var memberWriteStr = member.kind switch
            {
                MemberKind.Value => $"writer.WriteNamedValue({args});",
                MemberKind.List => $"writer.WriteNamedList({args});",
                MemberKind.ReadonlyList => $"writer.WriteNamedReadonlyList({args});",
                MemberKind.Dictionary => $"writer.WriteNamedDictionary({args});",
                MemberKind.ReadonlyDictionary => $"writer.WriteNamedReadonlyDictionary({args});",
                _ => throw new ArgumentOutOfRangeException()
            };
            stringBuilder.AppendLine($"            {memberWriteStr}");
        }
        stringBuilder.Append("            writer.WriteEndObject();");

        // 生成分部序列化器
        return $$"""
                 // Serializer
                 partial {{type.GetTypeKind()}} {{typeName}}
                 {
                     private partial class {{typeName}}JsonFormatter : PeroTools2.Serde.Json.ISerdeJsonFormatter<{{typeName}}>
                     {
                         public PeroTools2.Serde.Json.JsonType jsonType => PeroTools2.Serde.Json.JsonType.Object;

                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         public void SerializeUtf16({{typeName}} value, ref PeroTools2.Serde.Json.JsonWriterUtf16 writer)
                         {
                             {{stringBuilder}}
                         }

                         [global::System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         public void SerializeUtf8({{typeName}} value, ref PeroTools2.Serde.Json.JsonWriterUtf8 writer)
                         {
                             {{stringBuilder}}
                         }
                     }
                 }
                 """;
    }
}