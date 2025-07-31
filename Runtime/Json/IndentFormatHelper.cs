/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Dundun
 * Created On: 2025/07/29
 * Description: JSON data formatting
 */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PeroTools2.Serde.Json
{
    public static class IndentFormatHelper
    {
        /// <summary>
        /// Json数据格式化
        /// </summary>
        public static string FormatJson(string json)
        {
            StringBuilder result = new StringBuilder();
            int indent = 0;
            bool inString = false;
            bool escape = false;
            const string indentString = "    ";
            bool isEmptyStructure = false;

            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];
                if (inString)
                    InStringFormatJson(c, result, ref escape, ref inString);
                else
                {
                    if(char.IsWhiteSpace(c)) continue;
                    switch (c)
                    {
                        case '{':
                        case '[':
                            //判断内容是否为空，如果为空则那一部分不缩进
                            int j = i + 1;
                            //跳过所有空白的字符，保证j不越界
                            while (j < json.Length && char.IsWhiteSpace(json[j])) j++;
                            if ((j < json.Length) && (json[j] == '}' && c == '{') || (json[j] == ']' && c == '['))
                            {
                                isEmptyStructure = true;
                            }
                            result.Append(c);
                            if (!isEmptyStructure)
                            {
                                indent++;
                                AppendIndent(result, indent,indentString);
                            }
                            break;
                        case '}':
                        case ']':
                            if (!isEmptyStructure)
                            {
                                indent--;
                                AppendIndent(result, indent,indentString);
                            }
                            else
                            {
                                isEmptyStructure = false;
                            }
                            result.Append(c);
                            break;
                        case ',':
                            result.Append(c);
                            // 空结构后不换行
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
            //换行
            result.Append('\n');
            for (int i = 0; i < indent; i++)
            {
                result.Append(indentString);
            }
        }
        
        private static void InStringFormatJson(char jsonc,StringBuilder result, ref bool escape, ref bool inString)
        {
            if (escape)
            {
                escape = false;
                result.Append(jsonc);
            }
            else if (jsonc == '\\')
            {
                escape = true;
                result.Append(jsonc);
            }
            else if (jsonc == '"')
            {
                inString = false;
                result.Append(jsonc);
            }
            else
            {
                result.Append(jsonc);
            }
        }
    }
}
