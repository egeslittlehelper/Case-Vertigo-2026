using System;
using System.Collections.Generic;
using VertigoCase.Core;

namespace VertigoCase.Tests.Fakes
{
    /// <summary>
    /// Returns predetermined values for deterministic spin tests.
    /// </summary>
    public sealed class FakeRandomProvider : IRandomProvider
    {
        private readonly Queue<int> _values;

        public FakeRandomProvider(params int[] values)
        {
            _values = new Queue<int>(values);
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            if (_values.Count == 0)
                throw new InvalidOperationException("FakeRandomProvider has no more values.");

            return _values.Dequeue();
        }
    }
}
