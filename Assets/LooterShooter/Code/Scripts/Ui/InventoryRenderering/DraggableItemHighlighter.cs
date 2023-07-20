using LooterShooter.Framework;
using LooterShooter.Framework.Inventories;
using LooterShooter.Tools.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LooterShooter.Ui.InventoryRenderering
{
    /// <summary>
    /// Highlights the area below a <see cref="DraggableItem"/> to show if the item can be dropped there.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class DraggableItemHighlighter : SingletonBehaviour<DraggableItemHighlighter>
    {
        [SerializeField] private Color _validPositionColor = new(0f, 1f, 0f, 0.4f);
        [SerializeField] private Color _invalidPositionColor = new(1f, 0f, 0f, 0.4f);
        [SerializeField] private bool _showAlsoWhenNoDropTarget;
        
        private RectTransform _rectTransform;
        private Image _validatorImage;


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _validatorImage = GetComponent<Image>();
            Hide();
        }


        private void LateUpdate()
        {
            _rectTransform.SetAsLastSibling();
        }


        public void Hide() => _validatorImage.enabled = false;


        public void UpdateSize(float width, float height)
        {
            _validatorImage.enabled = true;
            
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }


        public void UpdatePosition(DraggableItem draggableItem, DraggableItemDropTarget belowReceiver)
        {
            Vector2 position;
            if (belowReceiver != null)
            {
                if (belowReceiver.DoSnapHighlighterToGrid)
                {
                    Vector2 relativeTopLeftPosition = draggableItem.GetTopLeftCornerRelativeToRect(belowReceiver.RectTransform);
                    Vector2 snappedPos = InventoryUtilities.SnapPositionToInventoryGrid(relativeTopLeftPosition);
                    Vector2 screenSpacePos = belowReceiver.RectTransform.GetScreenSpacePosition(snappedPos);

                    position = screenSpacePos;
                }
                else
                {
                    Vector2 relativeTopLeftPosition = draggableItem.GetTopLeftCornerRelativeToRect(belowReceiver.RectTransform);
                    Vector2 screenSpacePos = belowReceiver.RectTransform.GetScreenSpacePosition(relativeTopLeftPosition);
                    position = screenSpacePos;
                }
            }
            else
            {
                if (_showAlsoWhenNoDropTarget)
                {
                    Vector2 snappedPosition = InventoryUtilities.SnapPositionToInventoryGrid(draggableItem.RectTransform.anchoredPosition);
                    position = ((RectTransform)draggableItem.RectTransform.parent).GetScreenSpacePosition(snappedPosition);
                }
                else
                {
                    Hide();
                    return;
                }
            }
            
            _validatorImage.enabled = true;
            _rectTransform.position = position;

            bool isValidPosition = belowReceiver.CanDropDraggableItem(draggableItem);
            _validatorImage.color = isValidPosition ? _validPositionColor : _invalidPositionColor;
        }
    }
}