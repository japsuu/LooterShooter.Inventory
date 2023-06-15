using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryEntity : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;
        
        public InventoryItem ItemReference;
        public Vector2Int Position;
        private float _slotSize;
        private RectTransform _rectTransform;


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }


        public void Initialize(InventoryItem item, float slotSize)
        {
            ItemReference = item;
            _slotSize = slotSize;
            Position = item.Bounds.RootPosition;
            _itemImage.sprite = item.Item.Sprite;
            
            UpdatePositionAndSize();
        }


        public void UpdatePositionAndSize()
        {
            // Update position.
            float posX = Position.x * _slotSize + _slotSize / 2f * ItemReference.Bounds.Width;
            float posY = -(Position.y * _slotSize + _slotSize / 2f * ItemReference.Bounds.Width);
            _rectTransform.anchoredPosition3D = new Vector3(posX, posY, 0);
            
            // Update size.
            float size = ItemReference.Bounds.Width * _slotSize;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }
    }
}