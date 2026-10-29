// Allocation assertion for hosts without the Unity test framework.
// Unity counts GC allocations through ProfilerRecorder; outside Unity only allocated bytes are observable,
// so a threshold of n allocations is approximated by a small byte budget per allocation.
using NUnit.Framework.Constraints;

namespace PeroTools2.Serde.Share.Tests;

public static class ConstraintExtensions
{
    public static AllocatingGCMemoryConstraint AllocatingGCMemory(this ConstraintExpression expression, int maxBytes) => new(maxBytes);
}

public sealed class AllocatingGCMemoryConstraint(int maxAllocations) : Constraint
{
    private const int bytes_per_allocation = 4096;

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        // Invoke through the delegate type directly: DynamicInvoke allocates on its own and would
        // pollute the measurement. NUnit passes a TestDelegate for delegate-based assertions.
        var action = (TestDelegate) (object) actual;
        action();

        var budget = maxAllocations * bytes_per_allocation;
        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        return new ConstraintResult(this, allocated, allocated <= budget);
    }
}