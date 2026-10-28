using DG.Tweening;
using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Data;
using VertigoCase.UI;

namespace VertigoCase.Infrastructure
{
    /// <summary>
    /// Composition root — builds every dependency and wires them together. No other class creates domain objects.
    /// </summary>
    public sealed class GameInstaller : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private ZoneProgressionSO _zoneProgression;
        [SerializeField] private RewardCatalogSO _rewardCatalog;
        [SerializeField] private SpinAnimationSettingsSO _spinSettings;

        [Header("Views")]
        [SerializeField] private WheelView _wheelView;
        [SerializeField] private ZoneBarView _zoneBar;
        [SerializeField] private ZoneInfoView _zoneInfo;
        [SerializeField] private CollectedRewardsView _rewardsView;
        [SerializeField] private RewardFlyAnimator _flyAnimator;

        private GamePresenter _presenter;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            DOTween.Init(recycleAllByDefault: false, useSafeMode: true, logBehaviour: LogBehaviour.ErrorsOnly)
                .SetCapacity(50, 20);

            IZoneRules zoneRules = _zoneProgression.CreateZoneRules();
            IRewardScaling scaling = new CurveRewardScaling(_zoneProgression.RewardMultiplier);
            IRandomProvider random = new UnityRandomProvider();
            IWheelProvider wheelProvider = new SoWheelProvider(_zoneProgression);
            var spinResolver = new SpinResolver(random, scaling);
            var inventory = new RewardInventory();

            var session = new GameSession(zoneRules, wheelProvider, spinResolver, inventory);

            _presenter = new GamePresenter(
                session, _wheelView, _zoneBar, _zoneInfo,
                _rewardsView, _flyAnimator,
                zoneRules, scaling, _rewardCatalog);
            _presenter.Initialize();
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}
