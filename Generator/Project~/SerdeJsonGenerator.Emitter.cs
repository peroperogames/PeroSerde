/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/27
 * Description: SerdeJsonGenerator_Emitter
 */

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
        context.AddSource($"{typeSymbol.Name}.SerdeJsonSerializer.g.cs", "");
    }

    private static bool IsPartial(TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

    private static bool IsNested(TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Parent is TypeDeclarationSyntax;
}