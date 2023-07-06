using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class DraggableItemHighlighter : MonoBehaviour
    {
        public static DraggableItemHighlighter Singleton;
        
        [SerializeField] private Color _validPositionColor = new(0f, 1f, 0f, 0.4f);
        [SerializeField] private Color _invalidPositionColor = new(1f, 0f, 0f, 0.4f);
        [SerializeField] private bool _showAlsoWhenNoDropTarget;
        
        private RectTransform _rectTransform;
        private Image _validatorImage;


        private void Awake()
        {
            if (Singleton != null)
            {
                Debug.LogError($"Multiple {nameof(DraggableItemHighlighter)} in scene!");
                return;
            }

            Singleton = this;
            
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
        {//BUG: Check usages
            _validatorImage.enabled = true;
            
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }


        public void UpdatePosition(DraggableItem draggableItem, DraggableItemReceiverObject belowReceiver)
        {
            Vector2 position;
            if (belowReceiver != null)
            {
                if (belowReceiver.DoSnapHighlighterToGrid)
                {
                    Vector2 relativeAnchoredPos = Utilities.GetAnchoredPositionRelativeToRect(draggableItem.RectTransform.position, belowReceiver.RectTransform);
                    Vector2 snappedPos = Utilities.SnapPositionToInventoryGrid(relativeAnchoredPos);
                    Vector2 screenSpacePos = belowReceiver.RectTransform.GetScreenSpacePosition(snappedPos);

                    position = screenSpacePos;
                }
                else
                {
                    position = draggableItem.RectTransform.position;
                }
            }
            else
            {
                if (_showAlsoWhenNoDropTarget)
                {
                    Vector2 snappedPosition = Utilities.SnapPositionToInventoryGrid(draggableItem.RectTransform.anchoredPosition);
                    position = ((RectTransform)draggableItem.RectTransform.parent).GetScreenSpacePosition(snappedPosition);
                }
                else
                {
                    Hide();
                    return;
                }
            }
            
            _validatorImage.enabled = true;
            //_rectTransform.anchoredPosition = anchoredPosition;
            _rectTransform.position = position;

            bool isValidPosition = belowReceiver.CanDropDraggableItem(draggableItem);
            _validatorImage.color = isValidPosition ? _validPositionColor : _invalidPositionColor;
        }
    }
}