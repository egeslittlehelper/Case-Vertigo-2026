namespace VertigoCase.Core
{
    /// <summary>
    /// Strategy for computing the revive cost — escalating, flat, or ad-based.
    /// </summary>
    public interface IReviveCostPolicy
    {
        int GetCost(int reviveCountThisRun, int zone);
    }
}
