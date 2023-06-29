using System;
using InventorySystem.Inventories.Items;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // Serialized fields.
        [SerializeField] private RectTransform _contentsRoot;
        [SerializeField] private Image _itemImage;
        [SerializeField] private float _rotationSpeed = 20f;
        [SerializeField] private bool _showValidatorWhenDropTargetMissing;
        
        // Private fields: Initialization.
        private ItemMetadata _itemReference;
        private Inventory _containingInventory;
        private RectTransform _rectTransform;
        private CanvasGroup _draggingCanvasGroup;
        
        // Private fields: Runtime.
        private InventoryRenderer _belowInventoryRenderer;
        private Canvas _temporaryOverrideCanvas;
        private Vector2Int _position;
        private ItemRotation _rotation;
        private Vector2 _dragStartCursorPosition;
        private Vector2 _dragStartObjectPosition;
        private float _targetContentRotation;
        private bool _isUserDragging;

        public ItemMetadata ItemReference => _itemReference;


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
            UpdateValidatorSize();
            UpdateValidatorPosition();
        }


        public void Initialize(ItemMetadata itemMetadata, Inventory inventory)
        {
            _containingInventory = inventory;
            _itemReference = itemMetadata;
            
            _itemImage.sprite = _itemReference.ItemDataReference.Sprite;
            
            UpdateVisuals();

            // Rotate instantly to target rotation, so that items won't start spinning when opening the inventory :D.
            _contentsRoot.localRotation = Quaternion.Euler(0f, 0f, _targetContentRotation);
        }


        private InventoryBounds GetBounds(RectTransform positionRelativeTo)
        {
            bool isRotated = _rotation.ShouldFlipWidthAndHeight();
            int itemSizeX = _itemReference.ItemDataReference.InventorySizeX;
            int itemSizeY = _itemReference.ItemDataReference.InventorySizeY;
            int itemWidth = isRotated ? itemSizeY : itemSizeX;
            int itemHeight = isRotated ? itemSizeX : itemSizeY;

            // Get the top-left corner position.
            Vector2 topLeftCorner = GetAnchoredPositionRelativeToRect(positionRelativeTo);
            
            InventoryBounds bounds = new(Utilities.GetInventoryGridPosition(topLeftCorner), itemWidth, itemHeight);
            return bounds;
        }


        public bool IsBoundsValid()
        {
            if (_belowInventoryRenderer == null)
                return false;
            
            return _belowInventoryRenderer.TargetInventory.IsBoundsValid(GetBounds(_belowInventoryRenderer.EntityRootTransform), _itemReference);
        }


        private void UpdateVisuals()
        {
            UpdateRotation(_itemReference.RotationInInventory);
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
            int itemSizeX = _itemReference.ItemDataReference.InventorySizeX;
            int itemSizeY = _itemReference.ItemDataReference.InventorySizeY;
            int itemWidth = isRotated ? itemSizeY : itemSizeX;
            int itemHeight = isRotated ? itemSizeX : itemSizeY;
            
            // Calculate width as pixels.
            float rootWidth = itemWidth * Utilities.INVENTORY_SLOT_SIZE;
            float rootHeight = itemHeight * Utilities.INVENTORY_SLOT_SIZE;
            float contentsWidth = itemSizeX * Utilities.INVENTORY_SLOT_SIZE;
            float contentsHeight = itemSizeY * Utilities.INVENTORY_SLOT_SIZE;
            Vector2 rootNewSize = new(rootWidth, rootHeight);
            Vector2 contentsNewSize = new(contentsWidth, contentsHeight);
            
            // Adjust position to account for non-center pivot/anchor.
            // We need to do this because the object's pivot point is set to the top-left corner.
            // This basically means, that the pivot point will stay at the same point relative to the cursor.
            // With this change, the center of the rectTransform will stay at place relative to the cursor.
            Vector2 sizeDelta = _rectTransform.sizeDelta;
            Vector2 positionAdjustment = new Vector2(-(rootNewSize.x - sizeDelta.x), rootNewSize.y - sizeDelta.y) * 0.5f;

            // Adjust size.
            sizeDelta = rootNewSize;
            _rectTransform.sizeDelta = sizeDelta;
            _contentsRoot.sizeDelta = contentsNewSize;
            
            // Adjust position.
            _rectTransform.anchoredPosition += positionAdjustment;
        }

        private void UpdatePosition()
        {
            // Set the new position.
            _position = _itemReference.Bounds.Position;

            // Move to new position.
            float posX = _position.x * Utilities.INVENTORY_SLOT_SIZE;
            float posY = -(_position.y * Utilities.INVENTORY_SLOT_SIZE);
            _rectTransform.anchoredPosition = new Vector3(posX, posY, 0);
        }

        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _draggingCanvasGroup.blocksRaycasts = false;
            _isUserDragging = true;
            
            _temporaryOverrideCanvas = gameObject.AddComponent<Canvas>();
            _temporaryOverrideCanvas.overrideSorting = true;
            _temporaryOverrideCanvas.sortingOrder = 9999;

            _dragStartCursorPosition = eventData.position;
            _dragStartObjectPosition = _rectTransform.position;

            // Update validator.
            UpdateValidatorSize();
        }


        public void OnDrag(PointerEventData eventData)
        {
            //_rectTransform.anchoredPosition += eventData.delta / _temporaryOverrideCanvas.scaleFactor;

            //WARN: Because OnDrag is only called when the cursor moves, the Entity might "lag behind" when scrolling etc.
            //WARN: Tried calling this in Update, but that fucks up the rotation offset. TODO: Fix later.
            MoveToMousePosition();
        }

        
        private void MoveToMousePosition()
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 offset = currentMousePosition - _dragStartCursorPosition;
            Vector2 targetObjectPosition = _dragStartObjectPosition + offset;
            _rectTransform.position = targetObjectPosition;
            _belowInventoryRenderer = GetInventoryRendererBelow();
            UpdateValidatorPosition();
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            _isUserDragging = false;
            _draggingCanvasGroup.blocksRaycasts = true;
            DraggableItemHighlighter.Singleton.Hide();

            if (_temporaryOverrideCanvas != null)
                Destroy(_temporaryOverrideCanvas);

            // Request the inventory to move this entity.
            RequestMove();
        }


        private void UpdateValidatorSize()
        {
            Vector2 sizeDelta = _rectTransform.sizeDelta;
            DraggableItemHighlighter.Singleton.UpdateSize(sizeDelta.x, sizeDelta.y);
        }


        private void UpdateValidatorPosition()
        {
            if (_belowInventoryRenderer != null)
            {
                Vector2 relativeAnchoredPos = GetAnchoredPositionRelativeToRect(_belowInventoryRenderer.EntityRootTransform);
                Vector2 snappedPos = Utilities.SnapPositionToInventoryGrid(relativeAnchoredPos);
                Vector2 screenSpacePos = _belowInventoryRenderer.EntityRootTransform.GetScreenSpacePosition(snappedPos);

                DraggableItemHighlighter.Singleton.UpdatePosition(screenSpacePos, this);
            }
            else
            {
                if (_showValidatorWhenDropTargetMissing)
                {
                    Vector2 snappedPosition = Utilities.SnapPositionToInventoryGrid(_rectTransform.anchoredPosition);
                    DraggableItemHighlighter.Singleton.UpdatePosition(
                        ((RectTransform)_rectTransform.parent).GetScreenSpacePosition(snappedPosition), this);
                }
                else
                {
                    DraggableItemHighlighter.Singleton.Hide();
                }
            }
        }


        private void RequestMove()
        {
            if (_belowInventoryRenderer != null)
            {
                Vector2 relativeAnchoredPosition = GetAnchoredPositionRelativeToRect(_belowInventoryRenderer.EntityRootTransform);
                Vector2Int newPosition = Utilities.GetInventoryGridPosition(relativeAnchoredPosition);
                // print($"Request:{_position} -> {newPosition}");

                Inventory newInventory = GetInventoryBelow();
                if (_containingInventory.TryMoveItem(_position, newPosition, _rotation, newInventory))
                {
                    _containingInventory = newInventory;
                }
            }
            
            UpdateVisuals();
        }


        [CanBeNull]
        private InventoryRenderer GetInventoryRendererBelow()
        {
            // Raycast all cells to check if they have the same inventory.
            int itemSizeX = _itemReference.ItemDataReference.InventorySizeX;
            int itemSizeY = _itemReference.ItemDataReference.InventorySizeY;
            const float halfCellSize = Utilities.INVENTORY_SLOT_SIZE / 2f;

            for (int y = 0; y < itemSizeY; y++)
            {
                for (int x = 0; x < itemSizeX; x++)
                {
                    Vector2 cellCenter = new(
                        _rectTransform.position.x + halfCellSize + x * Utilities.INVENTORY_SLOT_SIZE,
                        _rectTransform.position.y - (halfCellSize + y * Utilities.INVENTORY_SLOT_SIZE));
                    
                    InventoryGrid current = Utilities.GetFirstComponentBelow<InventoryGrid>(cellCenter);

                    if (current != null)
                        return current.InventoryRenderer;
                }
            }

            return null;
        }


        [CanBeNull]
        private Inventory GetInventoryBelow() => _belowInventoryRenderer == null ? null : _belowInventoryRenderer.TargetInventory;


        private Vector2 GetAnchoredPositionRelativeToRect(RectTransform relativeTo)
        {
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, _rectTransform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(relativeTo, screenP, null, out Vector2 relativePoint);

            return relativePoint;
        }
    }
}