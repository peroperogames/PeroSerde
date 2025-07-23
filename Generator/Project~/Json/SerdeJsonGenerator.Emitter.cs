/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23-17:41:00
 */

using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.TypeKind;

namespace PeroTools2.Serde.Generator;

public partial class SerdeJsonGenerator
{
    private static readonly SymbolDisplayFormat s_TypeFullNameDisplayFormat = new(
        SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
    );

    private void Generate(ref GeneratorExecutionContext context, ITypeSymbol typeSymbol)
    {
        var typeFullName = typeSymbol.ToDisplayString(s_TypeFullNameDisplayFormat);
        var typeMeta = new TypeMeta(typeSymbol, m_ReferenceSymbols);
        var source = typeSymbol.TypeKind switch
        {
            Enum => typeSymbol.ContainsAttribute(m_ReferenceSymbols.flagsAttribute) ? GenerateFlagsFormatter(typeSymbol, typeMeta) : GenerateEnumsFormatter(typeSymbol, typeMeta),
            Class or Struct => GenerateRecordsFormatter(typeSymbol, typeMeta),
            _ => string.Empty
        };
        if (!string.IsNullOrEmpty(source))
        {
            context.AddSource($"{typeFullName}.SerdeJsonSerializer.g.cs", source);
        }
    }
}