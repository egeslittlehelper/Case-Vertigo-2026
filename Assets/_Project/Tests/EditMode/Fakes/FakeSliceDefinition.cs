using VertigoCase.Core;

namespace VertigoCase.Tests.Fakes
{
    public sealed class FakeSliceDefinition : ISliceDefinition
    {
        public IRewardDefinition Reward { get; }
        public int BaseAmount { get; }
        public int Weight { get; }
        public bool IsBomb { get; }

        public FakeSliceDefinition(IRewardDefinition reward, int baseAmount, int weight, bool isBomb = false)
        {
            Reward = reward;
            BaseAmount = baseAmount;
            Weight = weight;
            IsBomb = isBomb;
        }
    }
}
