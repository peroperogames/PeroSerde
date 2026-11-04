/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/19
 * Description: Utils
 */

namespace PeroTools2.Serde;

internal static class MathUtil
{
    private const int array_max_length = 0x7FFFFFC7; // Maximum length of int[], slightly smaller than int.MaxValue

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
