namespace VertigoCase.Core
{
    /// <summary>
    /// Wraps System.Random with optional seed for reproducible results.
    /// </summary>
    public sealed class SystemRandomProvider : IRandomProvider
    {
        private readonly System.Random _random;

        public SystemRandomProvider(int? seed = null)
        {
            _random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }
    }
}
