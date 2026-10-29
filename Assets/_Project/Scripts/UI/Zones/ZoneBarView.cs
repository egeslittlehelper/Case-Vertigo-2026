using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    /// <summary>
    /// Infinite zone strip — fixed pool of cells that slide left and recycle to the right end.
    /// </summary>
    public sealed class ZoneBarView : UIViewBase
    {
        [SerializeField] private RectTransform _cellContainer;
        [SerializeField] private ZoneCellView _cellPrefab;
        [SerializeField] private ZoneTypeColorsSO _colors;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private Image _highlightGradient;

        [Header("Highlight Sprites")]
        [SerializeField] private Sprite _highlightNormal;
        [SerializeField] private Sprite _highlightSafe;
        [SerializeField] private Sprite _highlightSuper;

        [SerializeField] [Range(7, 21)] private int _visibleCount = 13;
        [SerializeField] private float _slideDuration = 0.3f;

        private ZoneCellView[] _cells;
        private float _cellWidth;
        private int _currentZone;
        private int _leftmostZone;
        private IZoneRules _zoneRules;
        private Tween _slideTween;
        private int _pendingRecycleSteps;
        private TweenCallback _onSlideComplete;

        public void Initialize(int startZone, IZoneRules rules)
        {
            _zoneRules = rules;
            _currentZone = startZone;

            BuildCells();

            // Defer sizing to next frame — the highlight's AspectRatioFitter
            // needs one layout pass before rect.width is resolved.
            DOVirtual.DelayedCall(0f, () =>
            {
                MeasureAndSizeBar();
                LayoutCells(startZone);
                UpdateHighlight();
            }).SetLink(gameObject);
        }

        public void AdvanceTo(int zone, bool animate)
        {
            if (_cells == null) return;

            int steps = zone - _currentZone;
            _currentZone = zone;

            if (!animate || steps <= 0 || steps > _visibleCount)
            {
                LayoutCells(zone);
                UpdateHighlight();
                return;
            }

            _slideTween?.Kill();

            _pendingRecycleSteps = steps;
            _onSlideComplete ??= OnSlideComplete;

            float targetX = _cellContainer.anchoredPosition.x - steps * _cellWidth;

            _slideTween = _cellContainer
                .DOAnchorPosX(targetX, _slideDuration * steps)
                .SetEase(Ease.OutCubic)
                .OnComplete(_onSlideComplete)
                .SetLink(gameObject);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_cellContainer == null)
                _cellContainer = ChildReference.FindChild<RectTransform>(transform, "ui_container_zone_cells");
            if (_highlightImage == null)
                _highlightImage = ChildReference.FindChild<Image>(transform, "ui_image_zone_current_highlight");
            if (_highlightGradient == null)
                _highlightGradient = ChildReference.FindChild<Image>(transform, "ui_image_zone_current_gradient");
        }

        private void OnDisable()
        {
            _slideTween?.Kill();
            _slideTween = null;
        }

        private void BuildCells()
        {
            if (_cells != null) return;

            if (_highlightImage != null)
            {
                var highlightRt = _highlightImage.rectTransform;
                var arf = highlightRt.GetComponent<AspectRatioFitter>();
                if (arf == null) arf = highlightRt.gameObject.AddComponent<AspectRatioFitter>();
                arf.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
                arf.aspectRatio = 1f;
            }

            if (_cellContainer != null)
            {
                _cellContainer.anchorMin = new Vector2(0.5f, 0f);
                _cellContainer.anchorMax = new Vector2(0.5f, 1f);
                _cellContainer.sizeDelta = Vector2.zero;
            }

            _cells = new ZoneCellView[_visibleCount];
            for (int i = 0; i < _visibleCount; i++)
            {
                var cell = Instantiate(_cellPrefab, _cellContainer);
                cell.name = "ui_zone_cell";
                cell.EnableMaskable();

                var cellRt = cell.GetComponent<RectTransform>();

                var cellArf = cellRt.GetComponent<AspectRatioFitter>();
                if (cellArf != null) Destroy(cellArf);

                cellRt.anchorMin = new Vector2(0f, 0.5f);
                cellRt.anchorMax = new Vector2(0f, 0.5f);
                cellRt.pivot = new Vector2(0f, 0.5f);

                _cells[i] = cell;
            }
        }

        private void MeasureAndSizeBar()
        {
            if (_highlightImage != null)
                _cellWidth = _highlightImage.rectTransform.rect.width;

            if (_cellWidth <= 0)
                _cellWidth = GetComponent<RectTransform>().rect.height;

            if (_cellWidth <= 0)
                _cellWidth = 140f;

            var barRt = GetComponent<RectTransform>();
            barRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _visibleCount * _cellWidth);

            for (int i = 0; i < _cells.Length; i++)
            {
                var cellRt = _cells[i].GetComponent<RectTransform>();
                cellRt.sizeDelta = new Vector2(_cellWidth, _cellWidth);
            }
        }

        private void LayoutCells(int centerZone)
        {
            int half = _visibleCount / 2;
            _leftmostZone = centerZone - half;

            for (int i = 0; i < _visibleCount; i++)
            {
                int zone = _leftmostZone + i;
                PositionCell(i);
                BindCell(i, zone);
            }

            SnapContainer();
        }

        private void PositionCell(int index)
        {
            var rt = _cells[index].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(index * _cellWidth, 0f);
        }

        private void BindCell(int index, int zone)
        {
            if (zone < 1)
            {
                _cells[index].gameObject.SetActive(false);
                return;
            }

            _cells[index].gameObject.SetActive(true);
            ZoneType type = _zoneRules.GetZoneType(zone);
            bool isCurrent = zone == _currentZone;
            _cells[index].Set(zone, type, _colors, isCurrent);
        }

        private void UpdateHighlight()
        {
            if (_colors == null || _zoneRules == null) return;

            ZoneType type = _zoneRules.GetZoneType(_currentZone);

            if (_highlightGradient != null)
            {
                _highlightGradient.sprite = GetHighlightSprite(type);
                _highlightGradient.color = Color.white;
            }

            for (int i = 0; i < _visibleCount; i++)
            {
                int zone = _leftmostZone + i;
                if (zone >= 1)
                {
                    ZoneType cellType = _zoneRules.GetZoneType(zone);
                    bool isCurrent = zone == _currentZone;
                    _cells[i].Set(zone, cellType, _colors, isCurrent);
                }
            }
        }

        private Sprite GetHighlightSprite(ZoneType type)
        {
            return type switch
            {
                ZoneType.Safe => _highlightSafe != null ? _highlightSafe : _highlightNormal,
                ZoneType.Super => _highlightSuper != null ? _highlightSuper : _highlightNormal,
                _ => _highlightNormal
            };
        }

        private void OnSlideComplete()
        {
            RecycleCells(_pendingRecycleSteps);
            SnapContainer();
            UpdateHighlight();
        }

        private void RecycleCells(int steps)
        {
            for (int s = 0; s < steps; s++)
            {
                var first = _cells[0];

                for (int i = 0; i < _cells.Length - 1; i++)
                    _cells[i] = _cells[i + 1];

                _cells[_cells.Length - 1] = first;

                _leftmostZone++;
                int newZone = _leftmostZone + _visibleCount - 1;
                BindCell(_cells.Length - 1, newZone);
            }

            for (int i = 0; i < _visibleCount; i++)
                PositionCell(i);
        }

        private void SnapContainer()
        {
            int half = _visibleCount / 2;
            float offset = -(half + 0.5f) * _cellWidth;
            _cellContainer.anchoredPosition = new Vector2(offset, 0f);
        }
    }
}
