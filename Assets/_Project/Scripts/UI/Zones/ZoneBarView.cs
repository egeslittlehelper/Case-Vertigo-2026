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
            LayoutCells(startZone);
            UpdateHighlight();
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
        }

        private void OnDisable()
        {
            _slideTween?.Kill();
            _slideTween = null;
        }

        private void BuildCells()
        {
            if (_cells != null) return;

            _cells = new ZoneCellView[_visibleCount];
            var prefabRt = _cellPrefab.GetComponent<RectTransform>();
            _cellWidth = prefabRt.rect.width > 0 ? prefabRt.rect.width : 100f;

            for (int i = 0; i < _visibleCount; i++)
            {
                var cell = Instantiate(_cellPrefab, _cellContainer);
                cell.name = "ui_zone_cell";
                cell.EnableMaskable();
                _cells[i] = cell;
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

            if (_highlightImage != null)
            {
                _highlightImage.sprite = GetHighlightSprite(type);
                _highlightImage.color = Color.white;
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
            float offset = -half * _cellWidth;
            _cellContainer.anchoredPosition = new Vector2(offset, 0f);
        }
    }
}
