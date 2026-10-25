using UnityEngine;
using VertigoCase.Core;

namespace VertigoCase.Data
{
    /// <summary>
    /// Defines one reward type — implements Core's IRewardDefinition so domain stays Unity-free.
    /// </summary>
    [CreateAssetMenu(menuName = "VertigoCase/Reward Item")]
    public sealed class RewardItemSO : ScriptableObject, IRewardDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private RewardCategory _category;
        [SerializeField] private Sprite _icon;

        public string Id => _id;
        public string DisplayName => _displayName;
        public RewardCategory Category => _category;
        public Sprite Icon => _icon;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
                _id = name;
        }
    }
}
