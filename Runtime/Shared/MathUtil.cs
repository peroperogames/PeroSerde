/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/19
 * Description: Utils
 */

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// 让C#9的新特性init only setter在Unity中能正常工作
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class IsExternalInit { }
}

namespace PeroTools2.Serde
{
    internal static class MathUtil
    {
        private const int array_max_length = 0x7FFFFFC7; // int[]的最大长度, 比int.MaxValue小一丢丢

        public static int NewArrayCapacity(int oldSize)
        {
            var newSize = unchecked(oldSize * 2);
            if ((uint) newSize > array_max_length)
            {
                newSize = array_max_length;
            }
            return newSize;
        }
    }
}