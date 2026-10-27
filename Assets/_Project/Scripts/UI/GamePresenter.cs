using System;
using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    /// <summary>
    /// Connects domain events to views — plain C# class, no MonoBehaviour needed.
    /// Subscribes in Initialize, unsubscribes in Dispose.
    /// </summary>
    public sealed class GamePresenter : IDisposable
    {
        private readonly GameSession _session;
        private readonly WheelView _wheelView;
        private readonly ZoneBarView _zoneBar;
        private readonly ZoneInfoView _zoneInfo;
        private readonly IZoneRules _zoneRules;
        private readonly IRewardScaling _scaling;
        private readonly RewardCatalogSO _catalog;

        private ZoneType _previousZoneType;

        public GamePresenter(
            GameSession session,
            WheelView wheelView,
            ZoneBarView zoneBar,
            ZoneInfoView zoneInfo,
            IZoneRules zoneRules,
            IRewardScaling scaling,
            RewardCatalogSO catalog)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _wheelView = wheelView ?? throw new ArgumentNullException(nameof(wheelView));
            _zoneBar = zoneBar;
            _zoneInfo = zoneInfo;
            _zoneRules = zoneRules;
            _scaling = scaling;
            _catalog = catalog;
        }

        public void Initialize()
        {
            _wheelView.SpinClicked += OnSpinClicked;
            _session.ZoneChanged += OnZoneChanged;
            _session.RewardCollected += OnRewardCollected;
            _session.BombHit += OnBombHit;
            _session.CashedOut += OnCashedOut;
            _session.RunReset += OnRunReset;

            int zone = _session.CurrentZone;
            _previousZoneType = _session.CurrentZoneType;

            if (_zoneBar != null)
                _zoneBar.Initialize(zone, _zoneRules);

            UpdateZoneInfo(zone);
            BindWheel(zone, animate: false);
        }

        public void Dispose()
        {
            _wheelView.SpinClicked -= OnSpinClicked;
            _session.ZoneChanged -= OnZoneChanged;
            _session.RewardCollected -= OnRewardCollected;
            _session.BombHit -= OnBombHit;
            _session.CashedOut -= OnCashedOut;
            _session.RunReset -= OnRunReset;
        }

        private void OnSpinClicked()
        {
            if (!_session.CanSpin) return;

            SpinResult result = _session.BeginSpin();
            _wheelView.SetInteractable(false);
            _wheelView.PlaySpin(result.SliceIndex, () => _session.CompleteSpin());
        }

        private void OnZoneChanged(int zone, ZoneType type)
        {
            if (_zoneBar != null)
                _zoneBar.AdvanceTo(zone, animate: true);

            UpdateZoneInfo(zone);

            bool typeChanged = type != _previousZoneType;
            _previousZoneType = type;

            BindWheel(zone, animate: typeChanged);
        }

        private void OnRewardCollected(SpinResult result)
        {
            Debug.Log($"[Presenter] Collected: {result.Reward?.Id} x{result.Amount}");
        }

        private void OnBombHit(SpinResult result)
        {
            Debug.Log("[Presenter] Bomb hit! Rewards lost. (Popup in CP10)");
            _wheelView.SetInteractable(false);
        }

        private void OnCashedOut(IReadOnlyList<RewardStack> rewards)
        {
            Debug.Log($"[Presenter] Cashed out with {rewards.Count} reward type(s). (Popup in CP10)");
        }

        private void OnRunReset()
        {
            int zone = _session.CurrentZone;
            _previousZoneType = _session.CurrentZoneType;

            if (_zoneBar != null)
                _zoneBar.AdvanceTo(zone, animate: false);

            UpdateZoneInfo(zone);
            BindWheel(zone, animate: false);
        }

        private void BindWheel(int zone, bool animate)
        {
            var wheel = _session.CurrentWheel;
            var visuals = wheel as WheelConfigSO;

            if (animate)
                _wheelView.TransitionTo(wheel, visuals, zone, _scaling, _catalog);
            else
                _wheelView.Bind(wheel, visuals, zone, _scaling, _catalog);

            _wheelView.SetInteractable(_session.CanSpin);
        }

        private void UpdateZoneInfo(int zone)
        {
            if (_zoneInfo == null || _zoneRules == null) return;

            int nextSafe = _zoneRules.GetNextZoneOfType(zone, ZoneType.Safe);
            int nextSuper = _zoneRules.GetNextZoneOfType(zone, ZoneType.Super);
            _zoneInfo.Set(nextSafe, nextSuper);
        }
    }
}
