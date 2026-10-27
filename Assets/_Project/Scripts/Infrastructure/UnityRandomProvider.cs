using VertigoCase.Core;

namespace VertigoCase.Infrastructure
{
    /// <summary>
    /// Bridges Core's IRandomProvider to UnityEngine.Random for production use.
    /// </summary>
    public sealed class UnityRandomProvider : IRandomProvider
    {
        public int Range(int minInclusive, int maxExclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }
    }
}
