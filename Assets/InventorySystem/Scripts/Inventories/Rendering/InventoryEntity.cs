using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class InventoryEntity : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // Serialized fields.
        [SerializeField] private Image _itemImage;
        
        // Public fields.
        public InventoryItem ItemReference;
        public Vector2Int Position;

        // Private fields
        private Inventory _containingInventory;
        private RectTransform _rectTransform;
        private CanvasGroup _draggingCanvasGroup;
        private Canvas _canvas;
        private float _slotSize;


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _draggingCanvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponentInParent<Canvas>();
        }


        public void Initialize(InventoryItem item, Inventory inventory, float slotSize)
        {
            _containingInventory = inventory;
            ItemReference = item;
            _slotSize = slotSize;
            _itemImage.sprite = item.Item.Sprite;
            
            UpdatePositionAndVisuals();
        }


        public void UpdatePositionAndVisuals()
        {
            Position = ItemReference.Position;
            
            int itemWidth = ItemReference.Width;
            int itemHeight = ItemReference.Height;
            
            // Update position.
            float posX = Position.x * _slotSize;
            float posY = -(Position.y * _slotSize);
            _rectTransform.anchoredPosition3D = new Vector3(posX, posY, 0);
            
            // Update size.
            float sizeX = itemWidth * _slotSize;
            float sizeY = itemHeight * _slotSize;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeX);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeY);
            
            // Update image rotation.
            bool isRotated = ItemReference.Rotation is ItemRotation.DEG_90 or ItemRotation.DEG_270;
            float rotation = ItemReference.Rotation.AsDegrees();
            _itemImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            _itemImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, isRotated ? sizeY : sizeX);
            _itemImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, isRotated? sizeX : sizeY);
        }
        
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Set the item as the active drag item and bring it to the front.
            _draggingCanvasGroup.blocksRaycasts = false;
            _rectTransform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _draggingCanvasGroup.blocksRaycasts = true;

            Vector2 dragEndPosition = _rectTransform.anchoredPosition;
            
            // Move back to original position.
            float posX = Position.x * _slotSize;
            float posY = -(Position.y * _slotSize);
            _rectTransform.anchoredPosition3D = new Vector3(posX, posY, 0);
            
            // Request the inventory to move us.
            //TODO: Get the inventory below cursor.
            RequestMove(CalculateInventoryGridPosition(dragEndPosition), _containingInventory);
            
            //_rectTransform.anchoredPosition = CalculateSnappedPosition(_rectTransform.anchoredPosition);
        }


        private void RequestMove(Vector2Int newPosition, Inventory newInventory)
        {
            if (_containingInventory.TryMoveItem(Position, newPosition, ItemReference.Rotation, newInventory))
            {
                _containingInventory = newInventory;
            }
        }
        

        private Vector2 CalculateSnappedPosition(Vector2 position)
        {
            float x = Mathf.Round(position.x / _slotSize) * _slotSize;
            float y = Mathf.Round(position.y / _slotSize) * _slotSize;
            return new Vector2(x, y);
        }


        private Vector2Int CalculateInventoryGridPosition(Vector2 position)
        {
            int x = Mathf.RoundToInt(position.x / _slotSize);
            int y = -Mathf.RoundToInt(position.y / _slotSize);
            return new Vector2Int(x, y);
        }
    }
}