/*
 * Copyright (c) PeroPeroGames Co., Ltd. 
 * Author: zero 
 * Created On: 2025/07/03  
 */

using NUnit.Framework.Constraints;

namespace PeroTools2.Serde.Editor.Tests
{
    /// <summary>
    /// 用于扩展Is.Not语法
    /// </summary>
    internal static class ConstraintExtensions
    {
        /// <summary>
        /// 是否有进行分配
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static AllocatingGCMemoryConstraint AllocatingGCMemory(this ConstraintExpression chain, int threshold = 0)
        {
            var constraint = new AllocatingGCMemoryConstraint(threshold);
            chain.Append(constraint);
            return constraint;
        }
    }
}
