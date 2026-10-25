namespace VertigoCase.Core
{
    /// <summary>
    /// Strategy for random number generation — swappable for deterministic tests.
    /// </summary>
    public interface IRandomProvider
    {
        int Range(int minInclusive, int maxExclusive);
    }
}
