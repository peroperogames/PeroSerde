/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/26
 * Description: JsonCode
 */

using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    public static class JsonCode
    {
        public static JsonCode<TSymbol> Get<TSymbol>() where TSymbol : unmanaged
        {
            if (typeof(TSymbol) == typeof(byte))
            {
                return Unsafe.As<JsonCode<byte>, JsonCode<TSymbol>>(ref utf8);
            }
            if (typeof(TSymbol) == typeof(char))
            {
                return Unsafe.As<JsonCode<char>, JsonCode<TSymbol>>(ref utf16);
            }
            SerdeException.ThrowInvalidTSymbol<TSymbol>();
            return default;
        }

        public static JsonCode<byte> utf8 = new()
        {
            doubleQuote = (byte) '"',
            leftBrace = (byte) '{',
            rightBrace = (byte) '}',
            leftBracket = (byte) '[',
            rightBracket = (byte) ']',
            comma = (byte) ',',
            colon = (byte) ':',
            negative = (byte) '-',
            reverseSolidus = (byte) '\\',
            newLine = (byte) '\n',
            space = (byte) ' ',
        };

        public static JsonCode<char> utf16 = new()
        {
            doubleQuote = '"',
            leftBrace = '{',
            rightBrace = '}',
            leftBracket = '[',
            rightBracket = ']',
            comma = ',',
            colon = ':',
            negative = '-',
            reverseSolidus = '\\',
            newLine = '\n',
            space = ' '
        };
    }

    public sealed class JsonCode<TSymbol> where TSymbol : unmanaged
    {
        /// <summary>"</summary>
        public TSymbol doubleQuote;

        /// <summary>{</summary>
        public TSymbol leftBrace;

        /// <summary>}</summary>
        public TSymbol rightBrace;

        /// <summary>[</summary>
        public TSymbol leftBracket;

        /// <summary>]</summary>
        public TSymbol rightBracket;

        /// <summary>,</summary>
        public TSymbol comma;

        /// <summary>:</summary>
        public TSymbol colon;

        /// <summary>-</summary>
        public TSymbol negative;

        /// <summary>\</summary>
        public TSymbol reverseSolidus;

        /// <summary>\n</summary>
        public TSymbol newLine;

        /// <summary>' '</summary>
        public TSymbol space;
    }
}