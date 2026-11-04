/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: zero
 * Created On: 2025/07/03
 */

using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine.Profiling;

namespace PeroTools2.Serde.Share.Editor.Tests
{
    /// <summary>
    /// Extends the Is.Not syntax
    /// </summary>
    internal static class ConstraintExtensions
    {
        /// <summary>
        /// Whether any allocation occurred
        /// </summary>
        public static AllocatingGCMemoryConstraint AllocatingGCMemory(this ConstraintExpression chain, int threshold)
        {
            var constraint = new AllocatingGCMemoryConstraint(threshold);
            chain.Append(constraint);
            return constraint;
        }
    }

    /// <summary>
    /// Constraint that checks whether the number of heap allocations exceeds a threshold
    /// </summary>
    internal sealed class AllocatingGCMemoryConstraint : Constraint
    {
        private sealed class AllocatingGCMemoryResult : ConstraintResult
        {
            private readonly int m_Diff;
            private readonly int m_Threshold;

            public AllocatingGCMemoryResult(IConstraint constraint, object actualValue, int diff, int threshold) : base(constraint, actualValue, diff > threshold)
            {
                m_Diff = diff;
                m_Threshold = threshold;
            }

            public override void WriteMessageTo(MessageWriter writer)
            {
                if (m_Diff <= m_Threshold)
                {
                    writer.WriteMessageLine("The provided delegate did make {0} GC allocations.", m_Diff);
                }
                else
                {
                    writer.WriteMessageLine("The provided delegate made {0} GC allocation(s) over {1}.", m_Diff, m_Threshold);
                }
            }
        }

        private readonly int m_Threshold;

        public AllocatingGCMemoryConstraint(int threshold) => m_Threshold = threshold;

        private ConstraintResult ApplyTo(Action action, object original)
        {
            var recorder = Recorder.Get("GC.Alloc");

            // The recorder was created enabled, which means it captured the creation of the Recorder object itself, etc.
            // Disabling it flushes its data, so that we can retrieve the sample block count and have it correctly account
            // for these initial allocations.
            recorder.enabled = false;

#if !UNITY_WEBGL
            recorder.FilterToCurrentThread();
#endif

            recorder.enabled = true;

            try
            {
                action();
            }
            finally
            {
                recorder.enabled = false;
#if !UNITY_WEBGL
                recorder.CollectFromAllThreads();
#endif
            }

            return new AllocatingGCMemoryResult(this, original, recorder.sampleBlockCount, m_Threshold);
        }

        /// <summary>
        /// Applies GC memory constraint to the test.
        /// </summary>
        /// <param name="obj">An object to apply the GC constraint to. Should be a <see cref="TestDelegate"/>.</param>
        /// <returns>A ConstraintResult</returns>
        /// <exception cref="ArgumentNullException">Throws a <see cref="ArgumentNullException"/> if the provided object is null.</exception>
        /// <exception cref="ArgumentException">Throws a <see cref="ArgumentException"/> if the provided object is not a <see cref="TestDelegate"/>.</exception>
        public override ConstraintResult ApplyTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }

            if (obj is not TestDelegate d)
            {
                throw new ArgumentException($"The actual value must be a TestDelegate but was {obj.GetType()}");
            }

            return ApplyTo(d.Invoke, obj);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given reference.
        /// The default implementation simply dereferences the value but
        /// derived classes may override it to provide for delayed processing.
        /// </summary>
        /// <typeparam name="TActual">The type of the actual value delegate to be tested.</typeparam>
        /// <param name="del">A reference to the value delegate to be tested</param>
        /// <returns>A ConstraintResult</returns>
        /// <exception cref="ArgumentNullException">Throws a <see cref="ArgumentNullException"/> if the provided delegate is null.</exception>
        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            if (del == null)
            {
                throw new ArgumentNullException();
            }

            return ApplyTo(() => del.Invoke(), del);
        }

        /// <summary>
        /// The Description of what this constraint tests, for to use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "allocates GC memory";
    }
}