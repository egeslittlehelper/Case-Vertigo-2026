using System.Collections.Generic;
using VertigoCase.Core;

namespace VertigoCase.Tests.Fakes
{
    public sealed class FakeWheelDefinition : IWheelDefinition
    {
        public IReadOnlyList<ISliceDefinition> Slices { get; }
        public ZoneType ZoneType { get; }

        public FakeWheelDefinition(ZoneType zoneType, params ISliceDefinition[] slices)
        {
            ZoneType = zoneType;
            Slices = slices;
        }
    }
}
