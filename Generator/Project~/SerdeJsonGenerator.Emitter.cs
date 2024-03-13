/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: SerdeJsonGenerator_Emitter
 */

using System;
using System.Diagnostics.CodeAnalysis;
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
        var reference = new ReferenceSymbols(context.Compilation);
        if (!TypeMeta.TryNew(typeSyntax, typeSymbol, reference, out var typeMeta, out var diagnostic))
        {
            context.ReportDiagnostic(diagnostic);
            return;
        }
        var ns = typeSymbol.ContainingNamespace;
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
            partial {{classOrStructOrRecordStr}} {{typeName}}
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
}