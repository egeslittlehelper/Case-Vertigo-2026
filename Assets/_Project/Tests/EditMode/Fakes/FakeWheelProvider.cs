using System.Collections.Generic;
using VertigoCase.Core;

namespace VertigoCase.Tests.Fakes
{
    public sealed class FakeWheelProvider : IWheelProvider
    {
        private readonly Dictionary<ZoneType, IWheelDefinition> _wheels = new Dictionary<ZoneType, IWheelDefinition>();

        public ZoneType LastRequestedType { get; private set; }

        public void Register(ZoneType type, IWheelDefinition wheel)
        {
            _wheels[type] = wheel;
        }

        public IWheelDefinition GetWheelFor(int zone, ZoneType type)
        {
            LastRequestedType = type;
            return _wheels[type];
        }
    }
}
