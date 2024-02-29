/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: SerdeJsonGenerator
 */

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PeroTools2.Serde.Generator;

// 适用于Microsoft.CodeAnalysis.CSharp 3.x 的非增量式代码生成器
[Generator(LanguageNames.CSharp)]
public partial class SerdeJsonGenerator : ISourceGenerator
{
    public const string serde_json_namespace = "PeroTools2.Serde.Json";
    public const string serde_json_serializable_attr_name = "SerdeJsonSerializableAttribute";
    public const string serde_json_member_attr_name = "SerdeJsonMemberAttribute";
    public const string serde_json_include_member_attr_name = "SerdeJsonIncludeMemberAttribute";
    public const string serde_json_ignore_member_attr_name = "SerdeJsonIgnoreMemberAttribute";
    public const string serde_json_constructor_attr_name = "SerdeJsonConstructorAttribute";
    public const string serde_json_custom_formatter_attr_name = "SerdeJsonCustomFormatterAttribute";

    public const string serde_json_serializable_attr_fullname = $"{serde_json_namespace}.{serde_json_serializable_attr_name}";
    public const string serde_json_member_attr_fullname = $"{serde_json_namespace}.{serde_json_member_attr_name}";
    public const string serde_json_include_member_attr_fullname = $"{serde_json_namespace}.{serde_json_include_member_attr_name}";
    public const string serde_json_ignore_member_attr_fullname = $"{serde_json_namespace}.{serde_json_ignore_member_attr_name}";
    public const string serde_json_constructor_attr_fullname = $"{serde_json_namespace}.{serde_json_constructor_attr_name}";
    public const string serde_json_custom_formatter_attr_fullname = $"{serde_json_namespace}.{serde_json_custom_formatter_attr_name}";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(static () => new SyntaxContentReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxContentReceiver receiver || receiver.typeDeclarations.Count == 0)
        {
            return;
        }
        foreach (var typeSyntax in receiver.typeDeclarations)
        {
            Generate(ref context, typeSyntax);
        }
    }

    private sealed class SyntaxContentReceiver : ISyntaxContextReceiver
    {
        public HashSet<TypeDeclarationSyntax> typeDeclarations { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax or StructDeclarationSyntax or RecordDeclarationSyntax)
            {
                // 处于性能考虑这里不使用SemanticModel.
                // 另外, 使用SemanticModel还会导致具有多个partial部分的类被添加多次,
                // 因为HashSet的默认比较器不适用于判断Syntax是否相等
                var typeSyntax = (TypeDeclarationSyntax) context.Node;
                if (typeSyntax.AttributeLists.Count == 0)
                    return;
                foreach (var attrList in typeSyntax.AttributeLists)
                foreach (var attr in attrList.Attributes)
                {
                    var attrName = attr.Name.ToString();
                    if (attrName.Contains("SerdeJsonSerializable") || attrName.Contains("SerdeJsonCustomFormatter"))
                    {
                        typeDeclarations.Add((TypeDeclarationSyntax) context.Node);
                    }
                }
            }
        }
    }
}