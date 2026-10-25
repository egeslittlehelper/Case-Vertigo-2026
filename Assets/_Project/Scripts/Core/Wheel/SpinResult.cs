namespace VertigoCase.Core
{
    /// <summary>
    /// Immutable outcome of a spin, decided before the animation starts.
    /// </summary>
    public readonly struct SpinResult
    {
        public int SliceIndex { get; }
        public bool IsBomb { get; }
        public IRewardDefinition Reward { get; }
        public int Amount { get; }

        public SpinResult(int sliceIndex, bool isBomb, IRewardDefinition reward, int amount)
        {
            SliceIndex = sliceIndex;
            IsBomb = isBomb;
            Reward = reward;
            Amount = amount;
        }
    }
}
