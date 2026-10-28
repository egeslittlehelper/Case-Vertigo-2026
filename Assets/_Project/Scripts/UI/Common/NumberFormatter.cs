namespace VertigoCase.UI
{
    /// <summary>
    /// Formats numbers for display: 999, 1,634, 10K, 2.5M — allocation-free for cached small values.
    /// </summary>
    public static class NumberFormatter
    {
        private const int CacheSize = 100;
        private static readonly string[] Cache = BuildCache();

        public static string Format(int value)
        {
            if (value >= 0 && value < CacheSize)
                return Cache[value];

            return FormatUncached(value);
        }

        private static string FormatUncached(int value)
        {
            if (value >= 1_000_000)
            {
                float m = value / 1_000_000f;
                return m % 1f == 0f ? $"{(int)m}M" : $"{m:0.#}M";
            }

            if (value >= 10_000)
            {
                float k = value / 1_000f;
                return k % 1f == 0f ? $"{(int)k}K" : $"{k:0.#}K";
            }

            if (value >= 1_000)
                return string.Format("{0:#,0}", value);

            return value.ToString();
        }

        private static string[] BuildCache()
        {
            var cache = new string[CacheSize];
            for (int i = 0; i < CacheSize; i++)
                cache[i] = i.ToString();
            return cache;
        }
    }
}
