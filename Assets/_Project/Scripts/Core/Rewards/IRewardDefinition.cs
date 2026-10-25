namespace VertigoCase.Core
{
    /// <summary>
    /// Read-only identity of a reward type — implemented by ScriptableObjects in Data.
    /// </summary>
    public interface IRewardDefinition
    {
        string Id { get; }
        RewardCategory Category { get; }
    }
}
