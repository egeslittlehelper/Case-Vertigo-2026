namespace VertigoCase.Core
{
    /// <summary>
    /// Picks the right wheel config for a zone — implemented over ScriptableObjects in Data.
    /// </summary>
    public interface IWheelProvider
    {
        IWheelDefinition GetWheelFor(int zone, ZoneType type);
    }
}
