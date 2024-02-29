/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: SerdeJsonGenerator_Emitter
 */

using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PeroTools2.Serde.Generator;

public partial class SerdeJsonGenerator
{
    private static void Generate(ref GeneratorExecutionContext context, TypeDeclarationSyntax typeSyntax)
    {
        var semanticModel = context.Compilation.GetSemanticModel(typeSyntax.SyntaxTree);
        var typeSymbol = semanticModel.GetDeclaredSymbol(typeSyntax, context.CancellationToken);
        if (typeSymbol == null) return;
        if (!IsPartial(typeSyntax))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.mustBePartial, typeSyntax.Identifier.GetLocation(), typeSymbol.Name
            ));
            return;
        }
        if (IsNested(typeSyntax))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.nestedNotAllow, typeSyntax.Identifier.GetLocation(), typeSymbol.Name
            ));
            return;
        }
        var reference = new ReferenceSymbols(context.Compilation);
        // var typeMeta = new TypeMeta(typeSymbol, reference);
        var ns = typeSymbol.ContainingNamespace;
        var accessibilityStr = typeSymbol.DeclaredAccessibility switch
        {
            Accessibility.NotApplicable => "",
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Public => "public",
            _ => throw new ArgumentOutOfRangeException()
        };
        var classOrStructOrRecordStr = (typeSymbol.IsRecord, typeSymbol.IsValueType) switch
        {
            (true, true) => "record struct",
            (true, false) => "record",
            (false, true) => "struct",
            (false, false) => "class",
        };
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        
        var sb = new StringBuilder();
        if (ns != null && !ns.IsGlobalNamespace)
        {
            sb.AppendLine($"namespace {ns} {{");
        }
        sb.AppendLine($$"""
        {{accessibilityStr}} partial {{classOrStructOrRecordStr}} {{typeName}}
        {
        }
        """);
        if (ns != null && !ns.IsGlobalNamespace)
        {
            sb.AppendLine("}");
        }
        var src = sb.ToString();
        context.AddSource($"{typeSymbol.Name}.SerdeJsonSerializer.g.cs", src);
    }

    private static bool IsPartial(TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

    private static bool IsNested(TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Parent is TypeDeclarationSyntax;
}