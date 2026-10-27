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
        private readonly IRewardScaling _scaling;
        private readonly RewardCatalogSO _catalog;

        public GamePresenter(
            GameSession session,
            WheelView wheelView,
            IRewardScaling scaling,
            RewardCatalogSO catalog)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _wheelView = wheelView ?? throw new ArgumentNullException(nameof(wheelView));
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

            BindCurrentZone();
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
            BindCurrentZone();
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
            BindCurrentZone();
        }

        private void BindCurrentZone()
        {
            var wheel = _session.CurrentWheel;
            var visuals = wheel as WheelConfigSO;
            int zone = _session.CurrentZone;

            _wheelView.Bind(wheel, visuals, zone, _scaling, _catalog);
            _wheelView.SetInteractable(_session.CanSpin);
        }
    }
}
