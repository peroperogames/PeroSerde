/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: SerdeJsonGenerator
 */

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PeroTools2.Serde.Generator;

// 适用于Microsoft.CodeAnalysis.CSharp 3.x 的非增量式代码生成器
[Generator(LanguageNames.CSharp)]
public partial class SerdeJsonGenerator : ISourceGenerator
{
    private ReferenceSymbols m_ReferenceSymbols;
    private readonly Dictionary<ITypeSymbol, Location> m_TypeSymbols = new(SymbolEqualityComparer.Default);

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
        foreach (var diagnostic in receiver.diagnostics)
        {
            context.ReportDiagnostic(Diagnostic.Create(diagnostic.descriptor, diagnostic.location));
        }
        m_ReferenceSymbols = new ReferenceSymbols(context.Compilation);

        foreach (var typeSyntax in receiver.typeDeclarations)
        {
            var semanticModel = context.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeSyntax, context.CancellationToken) as ITypeSymbol;
            if (typeSymbol == null) continue;
            m_TypeSymbols.Add(typeSymbol, typeSymbol.Locations[0]);
        }

        foreach (var typeSymbol in m_TypeSymbols)
        {
            try
            {
                Generate(ref context, typeSymbol.Key);
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CreateErrorDescriptor(e), typeSymbol.Value));
            }
        }
    }

    private sealed class SyntaxContentReceiver : ISyntaxContextReceiver
    {
        public HashSet<BaseTypeDeclarationSyntax> typeDeclarations { get; } = [];
        public List<(DiagnosticDescriptor descriptor, Location location)> diagnostics { get; } = [];

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not (ClassDeclarationSyntax or StructDeclarationSyntax or RecordDeclarationSyntax or EnumDeclarationSyntax))
            {
                return;
            }
            // 出于性能考虑这里不使用SemanticModel.
            // 另外, 使用SemanticModel还会导致具有多个 partial 部分的类被添加多次,
            // 因为 HashSet 的默认比较器不适用于判断Syntax是否相等
            var typeSyntax = (BaseTypeDeclarationSyntax) context.Node;
            if (typeSyntax.AttributeLists.Count == 0)
            {
                return;
            }

            foreach (var attrList in typeSyntax.AttributeLists)
            foreach (var attr in attrList.Attributes)
            {
                var attrName = attr.Name.ToString();
                if (attrName.Contains("SerdeJsonSerializable"))
                {
                    typeDeclarations.Add(typeSyntax);
                }
            }
        }
    }
}