/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/19
 */

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde
{
    internal sealed class ReferenceEqualityComparer : IEqualityComparer<object>, IEqualityComparer
    {
        public static readonly ReferenceEqualityComparer shared = new();

        private ReferenceEqualityComparer()
        {
        }

        public new bool Equals(object x, object y)
            => ReferenceEquals(x, y);

        public int GetHashCode(object obj)
            => RuntimeHelpers.GetHashCode(obj);
    }
}