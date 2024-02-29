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
    private static readonly SymbolDisplayFormat s_TypeFullNameDisplayFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
    );

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
        if (!TypeMeta.TryNew(ref context, typeSyntax, typeSymbol, reference, out var typeMeta))
        {
            return;
        }
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
            (false, false) => "class",
            (false, true) => "struct",
            (true, false) => "record",
            (true, true) => "record struct",
        };
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var typeFullName = typeSymbol.ToDisplayString(s_TypeFullNameDisplayFormat);

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
        context.AddSource($"{typeFullName}.SerdeJsonSerializer.g.cs", src);
    }

    private static bool IsPartial(TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

    private static bool IsNested(TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Parent is TypeDeclarationSyntax;
}