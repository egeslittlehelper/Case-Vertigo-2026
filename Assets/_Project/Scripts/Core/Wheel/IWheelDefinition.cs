using System.Collections.Generic;

namespace VertigoCase.Core
{
    /// <summary>
    /// Read-only config for one wheel variant — implemented by WheelConfigSO in Data.
    /// </summary>
    public interface IWheelDefinition
    {
        IReadOnlyList<ISliceDefinition> Slices { get; }
        ZoneType ZoneType { get; }
    }
}
