using System;
using InventorySystem.Inventories.Items;
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
        [SerializeField] private RectTransform _contentsRoot;
        [SerializeField] private Image _itemImage;
        [SerializeField] private float _rotationSpeed = 20f;
        
        // Private fields
        private Vector2Int _position;
        private ItemRotation _rotation;
        private Vector2 _rootSize;
        private Inventory _containingInventory;
        private CanvasGroup _draggingCanvasGroup;
        private Vector3[] _rectTransformCorners;
        private Vector2 _validatorPosition;
        private Canvas _canvas;
        private float _slotSize;
        private float _targetContentRotation;
        private bool _isUserDragging;

        public RectTransform RectTransform { get; private set; }
        public InventoryItem Data { get; private set; }


        private void Awake()
        {
            _rectTransformCorners = new Vector3[4];
            RectTransform = GetComponent<RectTransform>();
            _draggingCanvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponentInParent<Canvas>();
        }


        private void Update()
        {
            // Smoothly rotate the item to target rotation.
            _contentsRoot.localRotation = Quaternion.Slerp(_contentsRoot.localRotation, Quaternion.Euler(0f, 0f, _targetContentRotation), Time.deltaTime * _rotationSpeed);
            
            if (!_isUserDragging)
                return;
            
            RectTransform.SetAsLastSibling();

            if (!Input.GetKeyDown(KeyCode.R))
                return;
            
            UpdateRotation(_rotation.NextRotation());
            UpdateSize();
                
            // Update validator.
            InventoryRenderer.Validator.SetTargetEntity(this, RectTransform.sizeDelta.x, RectTransform.sizeDelta.y);
        }


        public void Initialize(InventoryItem item, Inventory inventory, float slotSize)
        {
            _containingInventory = inventory;
            Data = item;
            _slotSize = slotSize;
            
            _itemImage.sprite = Data.Item.Sprite;
            
            UpdateVisuals();

            // Rotate instantly to target rotation, so that items won't start spinning when opening the inventory :D.
            _contentsRoot.localRotation = Quaternion.Euler(0f, 0f, _targetContentRotation);
        }


        public Inventory GetInventoryBelowCursor()
        {
            // TODO: BUG: Implement
            return _containingInventory;
        }


        public InventoryBounds GetBounds()
        {
            bool isRotated = _rotation is ItemRotation.DEG_90 or ItemRotation.DEG_270;
            int itemSizeX = Data.Item.InventorySizeX;
            int itemSizeY = Data.Item.InventorySizeY;
            int itemWidth = isRotated ? itemSizeY : itemSizeX;
            int itemHeight = isRotated ? itemSizeX : itemSizeY;

            // Get the top-left corner position.
            Vector2 topLeftCorner = GetTopLeftCornerPosition();
            
            InventoryBounds bounds = new(GetInventoryGridPosition(topLeftCorner), itemWidth, itemHeight);
            return bounds;
        }


        private void UpdateVisuals()
        {
            UpdateRotation(Data.Rotation);
            UpdateSize();
            UpdatePosition();
        }


        private void UpdateRotation(ItemRotation newRotation)
        {
            // Set the new rotation.
            _rotation = newRotation;
            
            // Image rotation:
            _targetContentRotation = _rotation.AsDegrees();
        }


        private void UpdateSize()
        {
            // Root object size:
            // If the object is rotated, we need to flip width and height.
            bool isRotated = _rotation is ItemRotation.DEG_90 or ItemRotation.DEG_270;
            int itemSizeX = Data.Item.InventorySizeX;
            int itemSizeY = Data.Item.InventorySizeY;
            int itemWidth = isRotated ? itemSizeY : itemSizeX;
            int itemHeight = isRotated ? itemSizeX : itemSizeY;
            
            // Calculate width as pixels.
            float rootWidth = itemWidth * _slotSize;
            float rootHeight = itemHeight * _slotSize;
            float contentsWidth = itemSizeX * _slotSize;
            float contentsHeight = itemSizeY * _slotSize;

            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rootWidth);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rootHeight);

            _contentsRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentsWidth);
            _contentsRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentsHeight);
        }


        private void UpdatePosition()
        {
            // Set the new position.
            _position = Data.Position;

            // Move to new position.
            float posX = _position.x * _slotSize;
            float posY = -(_position.y * _slotSize);
            RectTransform.anchoredPosition = new Vector3(posX, posY, 0);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            // Set the item as the active drag item and bring it to the front.
            _draggingCanvasGroup.blocksRaycasts = false;
            _isUserDragging = true;
            
            // Update validator.
            InventoryRenderer.Validator.SetTargetEntity(this, RectTransform.sizeDelta.x, RectTransform.sizeDelta.y);
        }
        

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            
            // Update validator.
            Vector3 snappedPosition = SnapPositionToGrid(RectTransform.anchoredPosition);

            // TODO: Get the inventory below cursor.
            InventoryRenderer.Validator.UpdateAnchoredPosition(snappedPosition, this);
        }
        

        public void OnEndDrag(PointerEventData eventData)
        {
            _isUserDragging = false;
            _draggingCanvasGroup.blocksRaycasts = true;
            InventoryRenderer.Validator.Hide();

            // Get the top-left corner position.
            Vector2 topLeftCorner = GetTopLeftCornerPosition();

            // Request the inventory to move this entity. TODO: Get the inventory below cursor.
            RequestMove(GetInventoryGridPosition(topLeftCorner), _containingInventory);
        }


        private Vector2 GetTopLeftCornerPosition()
        {
            // Below is the code to get the top-left corner when the pivot is set to the center of the object.
            // RectTransform.GetLocalCorners(_rectTransformCorners);
            // Vector2 cornerPos = new(
            //     RectTransform.anchoredPosition.x + _rectTransformCorners[0].x,
            //     RectTransform.anchoredPosition.y - _rectTransformCorners[0].y);
            return RectTransform.anchoredPosition;
        }


        private void RequestMove(Vector2Int newPosition, Inventory newInventory)
        {
            // print($"Request:{_position} -> {newPosition}");
            if (_containingInventory.TryMoveItem(_position, newPosition, _rotation, newInventory))
                _containingInventory = newInventory;
            
            // Either data didn't change and contains the position where the dragging started,
            // or it now contains the new position.
            UpdateVisuals();
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