namespace VertigoCase.Core
{
    /// <summary>
    /// Abstracts zone classification so views never hard-code interval logic.
    /// </summary>
    public interface IZoneRules
    {
        ZoneType GetZoneType(int zone);
        int GetNextZoneOfType(int fromZone, ZoneType type);
    }
}
