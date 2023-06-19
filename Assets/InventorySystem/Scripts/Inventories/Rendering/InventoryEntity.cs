using System.Collections.Generic;
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
        private RectTransform _rectTransform;
        private CanvasGroup _draggingCanvasGroup;
        private Canvas _canvas;
        private Inventory _containingInventory;
        private Vector2Int _position;
        private ItemRotation _rotation;
        private float _targetContentRotation;
        private bool _isUserDragging;

        public InventoryItem Data { get; private set; }


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _draggingCanvasGroup = GetComponent<CanvasGroup>();
        }


        private void Update()
        {
            // Smoothly rotate the item to target rotation.
            _contentsRoot.localRotation = Quaternion.Slerp(_contentsRoot.localRotation, Quaternion.Euler(0f, 0f, _targetContentRotation), Time.deltaTime * _rotationSpeed);
            
            if (!_isUserDragging)
                return;
            
            _rectTransform.SetAsLastSibling();

            if (!Input.GetKeyDown(KeyCode.R))
                return;
            
            UpdateRotation(_rotation.NextRotation());
            UpdateSize();
                
            // Update validator.
            InventoryEntityPositionValidator.Singleton.SetTargetEntity(this, _rectTransform.sizeDelta.x, _rectTransform.sizeDelta.y);
            Vector3 snappedPosition = InventoryUtilities.SnapPositionToInventoryGrid(_rectTransform.anchoredPosition);
            InventoryEntityPositionValidator.Singleton.UpdateAnchoredPosition(snappedPosition, this);
        }


        public void Initialize(InventoryItem item, Inventory inventory)
        {
            _containingInventory = inventory;
            Data = item;
            
            _itemImage.sprite = Data.Item.Sprite;
            
            UpdateVisuals();

            // Rotate instantly to target rotation, so that items won't start spinning when opening the inventory :D.
            _contentsRoot.localRotation = Quaternion.Euler(0f, 0f, _targetContentRotation);
        }


        public Inventory GetInventoryBelowEntity()
        {
            print(RaycastMouse().Count);
            
            // TODO: BUG: Implement
            return _containingInventory;
        }


        private static InventoryRenderer RaycastMouse(Vector2 screenSpacePosition)
        {
            PointerEventData pointerData = new(EventSystem.current)
            {
                pointerId = -1,
                position = screenSpacePosition
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if(result.gameObject.)
            }
		
            return results;
        }


        public InventoryBounds GetBounds()
        {
            bool isRotated = _rotation.ShouldFlipWidthAndHeight();
            int itemSizeX = Data.Item.InventorySizeX;
            int itemSizeY = Data.Item.InventorySizeY;
            int itemWidth = isRotated ? itemSizeY : itemSizeX;
            int itemHeight = isRotated ? itemSizeX : itemSizeY;

            // Get the top-left corner position.
            Vector2 topLeftCorner = _rectTransform.anchoredPosition;
            
            InventoryBounds bounds = new(InventoryUtilities.GetInventoryGridPosition(topLeftCorner), itemWidth, itemHeight);
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
            bool isRotated = _rotation.ShouldFlipWidthAndHeight();
            int itemSizeX = Data.Item.InventorySizeX;
            int itemSizeY = Data.Item.InventorySizeY;
            int itemWidth = isRotated ? itemSizeY : itemSizeX;
            int itemHeight = isRotated ? itemSizeX : itemSizeY;
            
            // Calculate width as pixels.
            float rootWidth = itemWidth * InventoryUtilities.INVENTORY_SLOT_SIZE;
            float rootHeight = itemHeight * InventoryUtilities.INVENTORY_SLOT_SIZE;
            float contentsWidth = itemSizeX * InventoryUtilities.INVENTORY_SLOT_SIZE;
            float contentsHeight = itemSizeY * InventoryUtilities.INVENTORY_SLOT_SIZE;
            Vector2 rootNewSize = new(rootWidth, rootHeight);
            Vector2 contentsNewSize = new(contentsWidth, contentsHeight);
            
            // Adjust position to account for non-center pivot/anchor.
            // We need to do this because the object's pivot point is set to the top-left corner.
            // This basically means, that the pivot point will stay at the same point relative to the cursor.
            // With this change, the center of the rectTransform will stay at place relative to the cursor.
            Vector2 positionAdjustment = new Vector2(-(rootNewSize.x - _rectTransform.sizeDelta.x), rootNewSize.y - _rectTransform.sizeDelta.y) * 0.5f;

            // Adjust size.
            _rectTransform.sizeDelta = rootNewSize;
            _contentsRoot.sizeDelta = contentsNewSize;
            
            // Adjust position.
            _rectTransform.anchoredPosition += positionAdjustment;
            
            // I have left the old code for center pivot below.
            /*// Cache the original anchor and pivot points.
            Vector2 originalAnchorMin = RectTransform.anchorMin;
            Vector2 originalAnchorMax = RectTransform.anchorMax;
            Vector2 originalPivot = RectTransform.pivot;

            // Set the anchor and pivot points to the center.
            RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            RectTransform.pivot = new Vector2(0.5f, 0.5f);

            // Scale the rectTransforms.
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rootWidth);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rootHeight);
            _contentsRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentsWidth);
            _contentsRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentsHeight);

            // Restore the original anchor and pivot points.
            RectTransform.anchorMin = originalAnchorMin;
            RectTransform.anchorMax = originalAnchorMax;
            RectTransform.pivot = originalPivot;*/
        }


        private void UpdatePosition()
        {
            // Set the new position.
            _position = Data.Position;

            // Move to new position.
            float posX = _position.x * InventoryUtilities.INVENTORY_SLOT_SIZE;
            float posY = -(_position.y * InventoryUtilities.INVENTORY_SLOT_SIZE);
            _rectTransform.anchoredPosition = new Vector3(posX, posY, 0);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            _draggingCanvasGroup.blocksRaycasts = false;
            _isUserDragging = true;
            
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = 9999;

            // Update validator.
            InventoryEntityPositionValidator.Singleton.SetTargetEntity(this, _rectTransform.sizeDelta.x, _rectTransform.sizeDelta.y);
        }
        

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            
            // Update validator.
            Vector3 snappedPosition = InventoryUtilities.SnapPositionToInventoryGrid(_rectTransform.anchoredPosition);

            InventoryEntityPositionValidator.Singleton.UpdateAnchoredPosition(snappedPosition, this);
        }
        

        public void OnEndDrag(PointerEventData eventData)
        {
            _isUserDragging = false;
            _draggingCanvasGroup.blocksRaycasts = true;
            InventoryEntityPositionValidator.Singleton.Hide();

            if (_canvas != null)
                Destroy(_canvas);

            // Request the inventory to move this entity. TODO: Get the inventory below cursor.
            RequestMove(InventoryUtilities.GetInventoryGridPosition(_rectTransform.anchoredPosition), _containingInventory);
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
    }
}