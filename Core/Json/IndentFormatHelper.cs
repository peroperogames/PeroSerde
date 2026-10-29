/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Dundun
 * Created On: 2025/07/29
 */
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json;

public static class IndentFormatHelper
{
    /// <summary>
    /// Formats JSON data
    /// </summary>
    public static string FormatJson(string json)
    {
        var result = new StringBuilder();
        var indent = 0;
        var inString = false;
        var escape = false;
        const string indentString = "    ";
        var isEmptyStructure = false;

        for (var i = 0; i < json.Length; i++)
        {
            var c = json[i];
            if (inString)
            {
                InStringFormatJson(c, result, ref escape, ref inString);
            }
            else
            {
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }
                switch (c)
                {
                    case '{':
                    case '[':
                        // Check whether the content is empty; an empty structure is not indented
                        var j = i + 1;
                        // Skip all whitespace characters while keeping j in bounds
                        while (j < json.Length && char.IsWhiteSpace(json[j]))
                        {
                            j++;
                        }
                        if (j < json.Length && json[j] == '}' && c == '{' || (json[j] == ']' && c == '['))
                        {
                            isEmptyStructure = true;
                        }
                        result.Append(c);
                        if (!isEmptyStructure)
                        {
                            indent++;
                            AppendIndent(result, indent, indentString);
                        }
                        break;
                    case '}':
                    case ']':
                        if (!isEmptyStructure)
                        {
                            indent--;
                            AppendIndent(result, indent, indentString);
                        }
                        else
                        {
                            isEmptyStructure = false;
                        }
                        result.Append(c);
                        break;
                    case ',':
                        result.Append(c);
                        // Do not start a new line after an empty structure
                        if (!isEmptyStructure)
                        {
                            AppendIndent(result, indent, indentString);
                        }
                        break;
                    case ':':
                        result.Append(": ");
                        break;
                    case '"':
                        inString = true;
                        result.Append(c);
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }
        }
        return result.ToString();
    }

    private static void AppendIndent(StringBuilder result, int indent, string indentString)
    {
        // New line
        result.Append('\n');
        for (var i = 0; i < indent; i++)
        {
            result.Append(indentString);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InStringFormatJson(char jsonc, StringBuilder result, ref bool escape, ref bool inString)
    {
        if (escape)
        {
            escape = false;
        }
        else if (jsonc == '\\')
        {
            escape = true;
        }
        else if (jsonc == '"')
        {
            inString = false;
        }
        result.Append(jsonc);
    }
}