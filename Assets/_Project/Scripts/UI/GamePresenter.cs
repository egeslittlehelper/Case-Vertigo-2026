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
        private readonly CollectedRewardsView _rewardsView;
        private readonly RewardFlyAnimator _flyAnimator;
        private readonly ExitButtonView _exitButton;
        private readonly BombPopupView _bombPopup;
        private readonly ConfirmLossPopupView _confirmLossPopup;
        private readonly CashOutPopupView _cashOutPopup;
        private readonly IZoneRules _zoneRules;
        private readonly IRewardScaling _scaling;
        private readonly RewardCatalogSO _catalog;
        private readonly IWallet _wallet;
        private readonly WalletView _walletView;
        private readonly string _currencyId;

        private ZoneType _previousZoneType;

        public GamePresenter(
            GameSession session,
            WheelView wheelView,
            ZoneBarView zoneBar,
            ZoneInfoView zoneInfo,
            CollectedRewardsView rewardsView,
            RewardFlyAnimator flyAnimator,
            ExitButtonView exitButton,
            BombPopupView bombPopup,
            ConfirmLossPopupView confirmLossPopup,
            CashOutPopupView cashOutPopup,
            IZoneRules zoneRules,
            IRewardScaling scaling,
            RewardCatalogSO catalog,
            IWallet wallet = null,
            WalletView walletView = null,
            string currencyId = null)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _wheelView = wheelView ?? throw new ArgumentNullException(nameof(wheelView));
            _zoneBar = zoneBar;
            _zoneInfo = zoneInfo;
            _rewardsView = rewardsView;
            _flyAnimator = flyAnimator;
            _exitButton = exitButton;
            _bombPopup = bombPopup;
            _confirmLossPopup = confirmLossPopup;
            _cashOutPopup = cashOutPopup;
            _zoneRules = zoneRules;
            _scaling = scaling;
            _catalog = catalog;
            _wallet = wallet;
            _walletView = walletView;
            _currencyId = currencyId ?? "";
        }

        public void Initialize()
        {
            _wheelView.SpinClicked += OnSpinClicked;
            _session.ZoneChanged += OnZoneChanged;
            _session.RewardCollected += OnRewardCollected;
            _session.BombHit += OnBombHit;
            _session.CashedOut += OnCashedOut;
            _session.Revived += OnRevived;
            _session.RunReset += OnRunReset;

            if (_exitButton != null)
                _exitButton.Clicked += OnExitClicked;
            if (_bombPopup != null)
            {
                _bombPopup.GiveUpClicked += OnGiveUpRequested;
                _bombPopup.ReviveClicked += OnReviveClicked;
            }
            if (_confirmLossPopup != null)
            {
                _confirmLossPopup.LoseClicked += OnConfirmLose;
                _confirmLossPopup.GoBackClicked += OnConfirmGoBack;
            }
            if (_cashOutPopup != null)
                _cashOutPopup.ContinueClicked += OnContinue;

            if (_wallet != null)
                _wallet.BalanceChanged += OnBalanceChanged;

            int zone = _session.CurrentZone;
            _previousZoneType = _session.CurrentZoneType;

            if (_zoneBar != null)
                _zoneBar.Initialize(zone, _zoneRules);

            if (_rewardsView != null)
                _rewardsView.Initialize();

            if (_flyAnimator != null)
                _flyAnimator.Initialize();

            if (_cashOutPopup != null)
                _cashOutPopup.Initialize();

            if (_exitButton != null)
                _exitButton.SetVisible(_session.CanExit, animate: false);

            if (_walletView != null && _wallet != null)
            {
                _walletView.SetBalance(_wallet.Balance, animate: false);

                if (!string.IsNullOrEmpty(_currencyId) && _catalog.TryGetIcon(_currencyId, out Sprite icon))
                    _walletView.SetIcon(icon);
            }

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
            _session.Revived -= OnRevived;
            _session.RunReset -= OnRunReset;

            if (_exitButton != null)
                _exitButton.Clicked -= OnExitClicked;
            if (_bombPopup != null)
            {
                _bombPopup.GiveUpClicked -= OnGiveUpRequested;
                _bombPopup.ReviveClicked -= OnReviveClicked;
            }
            if (_confirmLossPopup != null)
            {
                _confirmLossPopup.LoseClicked -= OnConfirmLose;
                _confirmLossPopup.GoBackClicked -= OnConfirmGoBack;
            }
            if (_cashOutPopup != null)
                _cashOutPopup.ContinueClicked -= OnContinue;

            if (_wallet != null)
                _wallet.BalanceChanged -= OnBalanceChanged;
        }

        private void OnSpinClicked()
        {
            if (!_session.CanSpin) return;

            if (_exitButton != null)
                _exitButton.SetVisible(false, animate: false);

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

            if (_exitButton != null)
                _exitButton.SetVisible(_session.CanExit, animate: true);
        }

        private void OnRewardCollected(SpinResult result)
        {
            if (result.Reward == null || _rewardsView == null) return;

            string id = result.Reward.Id;
            _catalog.TryGetIcon(id, out Sprite icon);

            int newTotal = GetCurrentTotal(id);

            if (_flyAnimator != null)
            {
                RectTransform from = _wheelView.GetSliceTransform(result.SliceIndex);

                _rewardsView.AddOrUpdate(id, icon, newTotal - result.Amount, animate: false);
                RectTransform to = _rewardsView.GetEntryTarget(id);

                if (from != null && to != null)
                {
                    _flyAnimator.Play(icon, from, to, () =>
                    {
                        _rewardsView.AddOrUpdate(id, icon, newTotal, animate: true);
                        _wheelView.SetInteractable(_session.CanSpin);
                    });
                    return;
                }
            }

            _rewardsView.AddOrUpdate(id, icon, newTotal, animate: true);
            _wheelView.SetInteractable(_session.CanSpin);
        }

        private void OnBombHit(SpinResult result)
        {
            _wheelView.SetInteractable(false);

            if (_bombPopup != null)
                ShowBombPopup();
        }

        private void OnCashedOut(IReadOnlyList<RewardStack> rewards)
        {
            _wheelView.SetInteractable(false);

            if (_exitButton != null)
                _exitButton.SetVisible(false, animate: false);

            if (_cashOutPopup != null)
                _cashOutPopup.Show(rewards, _catalog);
        }

        private void OnExitClicked()
        {
            if (!_session.CanExit) return;
            _session.CashOut();
        }

        private void OnGiveUpRequested()
        {
            if (_confirmLossPopup != null)
            {
                if (_bombPopup != null)
                    _bombPopup.Hide(() => _confirmLossPopup.Show());
            }
            else
            {
                if (_bombPopup != null)
                    _bombPopup.Hide(() => _session.GiveUp());
            }
        }

        private void OnConfirmLose()
        {
            if (_confirmLossPopup != null)
                _confirmLossPopup.Hide(() => _session.GiveUp());
        }

        private void OnConfirmGoBack()
        {
            if (_confirmLossPopup != null)
            {
                _confirmLossPopup.Hide(() =>
                {
                    if (_bombPopup != null)
                        ShowBombPopup();
                });
            }
        }

        private void OnReviveClicked()
        {
            if (_bombPopup != null)
                _bombPopup.Hide(() => _session.TryRevive());
        }

        private void OnRevived()
        {
            if (_exitButton != null)
                _exitButton.SetVisible(_session.CanExit, animate: true);

            BindWheel(_session.CurrentZone, animate: false);
        }

        private void OnBalanceChanged(int newBalance)
        {
            if (_walletView != null)
                _walletView.SetBalance(newBalance, animate: true);
        }

        private void ShowBombPopup()
        {
            int lostCount = _session.CollectedRewards.Count;
            bool canRevive = _session.CanRevive;
            int cost = _session.ReviveCost;
            _bombPopup.Show(lostCount, canRevive, cost);
        }

        private void OnContinue()
        {
            if (_cashOutPopup != null)
                _cashOutPopup.Hide(() => _session.Restart());
        }

        private void OnRunReset()
        {
            int zone = _session.CurrentZone;
            _previousZoneType = _session.CurrentZoneType;

            if (_zoneBar != null)
                _zoneBar.AdvanceTo(zone, animate: false);

            if (_rewardsView != null)
                _rewardsView.ClearAll();

            if (_exitButton != null)
                _exitButton.SetVisible(_session.CanExit, animate: false);

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

        private int GetCurrentTotal(string rewardId)
        {
            var items = _session.CollectedRewards;
            for (int i = 0; i < items.Count; i++)
            {
                if (string.Equals(items[i].Reward.Id, rewardId, StringComparison.Ordinal))
                    return items[i].Amount;
            }
            return 0;
        }
    }
}
