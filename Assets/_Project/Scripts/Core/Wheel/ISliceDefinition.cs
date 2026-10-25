namespace VertigoCase.Core
{
    /// <summary>
    /// Read-only data for one wheel slice — implemented by ScriptableObjects in Data.
    /// </summary>
    public interface ISliceDefinition
    {
        IRewardDefinition Reward { get; }
        int BaseAmount { get; }
        int Weight { get; }
        bool IsBomb { get; }
    }
}
