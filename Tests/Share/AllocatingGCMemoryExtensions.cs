// Allocation assertions cannot be evaluated outside Unity: Unity counts GC allocations through its
// profiler recorder, while a plain .NET host can only observe allocated bytes, which is not comparable.
// The tests are kept intact and are skipped here with an explicit reason instead of being faked green.
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace PeroTools2.Serde.Share.Tests;

public static class ConstraintExtensions
{
    public static AllocatingGCMemoryConstraint AllocatingGCMemory(this ConstraintExpression expression, int maxAllocations)
    {
        var constraint = new AllocatingGCMemoryConstraint(maxAllocations);
        expression.Append(constraint);
        return constraint;
    }
}

public sealed class AllocatingGCMemoryConstraint : Constraint
{
    private readonly int m_MaxAllocations;

    public AllocatingGCMemoryConstraint(int maxAllocations)
    {
        m_MaxAllocations = maxAllocations;
    }

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        throw new IgnoreException(
            $"GC allocation counting (limit {m_MaxAllocations}) requires the Unity profiler recorder; "
            + "run this test inside Unity to validate allocations.");
    }
}