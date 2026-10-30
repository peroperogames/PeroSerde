/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: SerdeJsonGenerator
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PeroTools2.Serde.Generator;

// 适用于Microsoft.CodeAnalysis.CSharp 3.x 的非增量式代码生成器
[Generator(LanguageNames.CSharp)]
public partial class SerdeJsonGenerator : ISourceGenerator
{
    private ReferenceSymbols m_ReferenceSymbols;
    private readonly Dictionary<ITypeSymbol, Location> m_TypeSymbols = new(SymbolEqualityComparer.Default);
    private readonly List<(DiagnosticDescriptor descriptor, Location location)> diagnostics = new();

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

        m_ReferenceSymbols = new ReferenceSymbols(context.Compilation);

        foreach (var typeSyntax in receiver.typeDeclarations)
        {
            try
            {
                var semanticModel = context.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);
                var typeSymbol = semanticModel.GetDeclaredSymbol(typeSyntax, context.CancellationToken) as ITypeSymbol;
                if (typeSymbol == null) continue;
                m_TypeSymbols.Add(typeSymbol, typeSymbol.Locations[0]);
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CreateErrorDescriptor(e), typeSyntax.GetLocation()));
            }
        }

        foreach (var typeSymbol in m_TypeSymbols)
        {
            try
            {
                if (!TryGetDiagnostic(typeSymbol.Key, out var descriptor))
                {
                    Generate(ref context, typeSymbol.Key);
                }
                else
                {
                    context.ReportDiagnostic(descriptor!);
                }
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CreateErrorDescriptor(e), typeSymbol.Value));
            }
        }
    }

    private static bool TryGetDiagnostic(ITypeSymbol typeSymbol, out Diagnostic? diagnostic)
    {
        diagnostic = null;
        if (!typeSymbol.IsValueType && typeSymbol.GetMembers().FirstOrDefault(static m => m.Name == ".ctor") is not IMethodSymbol { DeclaredAccessibility: Accessibility.Public, Parameters.Length: 0 })
            diagnostic = Diagnostic.Create(DiagnosticDescriptors.needEmptyCtor, typeSymbol.Locations[0], typeSymbol.ToDisplayString());
        else if (typeSymbol.IsAbstract || typeSymbol.TypeKind == TypeKind.Interface)
            diagnostic = Diagnostic.Create(DiagnosticDescriptors.abstractClassOrInterfaceNotAllow, typeSymbol.Locations[0], typeSymbol.ToDisplayString());
        else if (typeSymbol.IsRefLikeType)
            diagnostic = Diagnostic.Create(DiagnosticDescriptors.refStructNotAllow, typeSymbol.Locations[0], typeSymbol.ToDisplayString());
        else if (typeSymbol.IsStatic)
            diagnostic = Diagnostic.Create(DiagnosticDescriptors.staticClassNotAllow, typeSymbol.Locations[0], typeSymbol.ToDisplayString());
        else if (typeSymbol.ContainingType != null)
            diagnostic = Diagnostic.Create(DiagnosticDescriptors.nestedNotAllow, typeSymbol.Locations[0], typeSymbol.ToDisplayString());
        return diagnostic != null;
    }

    public sealed class SyntaxContentReceiver : ISyntaxContextReceiver
    {
        public HashSet<BaseTypeDeclarationSyntax> typeDeclarations { get; } = [];

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