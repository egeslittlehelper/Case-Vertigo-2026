using VertigoCase.Core;

namespace VertigoCase.Tests.Fakes
{
    public sealed class FakeRewardDefinition : IRewardDefinition
    {
        public string Id { get; }
        public RewardCategory Category { get; }

        public FakeRewardDefinition(string id, RewardCategory category = RewardCategory.Currency)
        {
            Id = id;
            Category = category;
        }
    }
}
