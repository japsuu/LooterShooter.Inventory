using InventorySystem.Inventories.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class InventoryEntity : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // Serialized fields.
        [SerializeField] private RectTransform _contentsRoot;
        [SerializeField] private Image _itemImage;
        
        // Private fields
        [SerializeField] private Vector2Int _position;
        [SerializeField] private ItemRotation _rotation;
        private InventoryItem _data;
        private Inventory _containingInventory;
        private RectTransform _rectTransform;
        private CanvasGroup _draggingCanvasGroup;
        private Canvas _canvas;
        private float _slotSize;
        private bool _isUserDragging;


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _draggingCanvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponentInParent<Canvas>();
        }


        private void Update()
        {
            if (!_isUserDragging)
                return;
            
            if (Input.GetKeyDown(KeyCode.R))
                UpdateRotation(_rotation.NextRotation());
        }


        public void Initialize(InventoryItem item, Inventory inventory, float slotSize)
        {
            _containingInventory = inventory;
            _data = item;
            _slotSize = slotSize;
            
            _itemImage.sprite = _data.Item.Sprite;
            
            SetVisualsFromData();
        }


        private void SetVisualsFromData()
        {
            UpdatePosition(_data.Position);

            UpdateRotation(_data.Rotation);

            UpdateScale(_data.Width, _data.Height, _data.Item.InventorySizeX, _data.Item.InventorySizeY);
        }


        private void UpdatePosition(Vector2Int newPosition)
        {
            // Set the new position.
            _position = newPosition;
            
            float posX = _position.x * _slotSize;
            float posY = -(_position.y * _slotSize);
            _rectTransform.anchoredPosition3D = new Vector3(posX, posY, 0);
        }


        private void UpdateRotation(ItemRotation newRotation)
        {
            // Set the new rotation.
            _rotation = newRotation;
            
            // Image rotation:
            float rotation = _rotation.AsDegrees();
            _contentsRoot.localRotation = Quaternion.Euler(0f, 0f, rotation);

            bool rotated = _rotation is ItemRotation.DEG_90 or ItemRotation.DEG_270;
            
            if(rotated)
                UpdateScale(_data.Width, _data.Height, _data.Item.InventorySizeX, _data.Item.InventorySizeY);
            else
                UpdateScale(_data.Height, _data.Width, _data.Item.InventorySizeX, _data.Item.InventorySizeY);
        }


        private void UpdateScale(float rootWidth, float rootHeight, float contentsSizeX, float contentsSizeY)
        {
            float rotatedWidth = rootWidth * _slotSize;
            float rotatedHeight = rootHeight * _slotSize;
            
            float itemWidth = contentsSizeX * _slotSize;
            float itemHeight = contentsSizeY * _slotSize;
            
            // Root object size:
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rotatedWidth);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rotatedHeight);

            // Image size:
            _contentsRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemWidth);
            _contentsRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            // Set the item as the active drag item and bring it to the front.
            _draggingCanvasGroup.blocksRaycasts = false;
            _rectTransform.SetAsLastSibling();
            _isUserDragging = true;
            
            // offsets from top-left corner: print(_rectTransform.position);
            // offsets from top-left corner: print(eventData.position);
        }
        

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
        

        public void OnEndDrag(PointerEventData eventData)
        {
            _isUserDragging = false;
            _draggingCanvasGroup.blocksRaycasts = true;

            Vector2 dragEndPosition = _rectTransform.anchoredPosition;
            
            // Move back to original position.
            float posX = _position.x * _slotSize;
            float posY = -(_position.y * _slotSize);
            _rectTransform.anchoredPosition3D = new Vector3(posX, posY, 0);
            
            // Request the inventory to move this entity.
            //TODO: Get the inventory below cursor.
            RequestMove(GetInventoryGridPosition(dragEndPosition), _containingInventory);
        }


        private void RequestMove(Vector2Int newPosition, Inventory newInventory)
        {
            print($"Request:{_position} -> {newPosition}");
            if (_containingInventory.TryMoveItem(_position, newPosition, _rotation, newInventory))
                _containingInventory = newInventory;
            
            // Either data didn't change and contains the position where the dragging started,
            // or it now contains the new position.
            SetVisualsFromData();
        }


        private Vector2Int GetInventoryGridPosition(Vector2 position)
        {
            int x = Mathf.RoundToInt(position.x / _slotSize);
            int y = -Mathf.RoundToInt(position.y / _slotSize);
            return new Vector2Int(x, y);
        }
        

        private Vector2 SnapPositionToGrid(Vector2 position)
        {
            float x = Mathf.Round(position.x / _slotSize) * _slotSize;
            float y = Mathf.Round(position.y / _slotSize) * _slotSize;
            return new Vector2(x, y);
        }
    }
}