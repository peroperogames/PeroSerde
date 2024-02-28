/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/19
 * Description: TypeHelpers
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde
{
    public static class TypeHelper
    {
        public static bool IsAnonymous(Type type) =>
            type.Namespace == null
            && type.IsSealed
            &&
            (
                type.Name.StartsWith("<>f__AnonymousType", StringComparison.Ordinal)
                || type.Name.StartsWith("<>__AnonType", StringComparison.Ordinal)
                || type.Name.StartsWith("VB$AnonymousType_", StringComparison.Ordinal)
            )
            && type.IsDefined(typeof(CompilerGeneratedAttribute), false);
    }
}