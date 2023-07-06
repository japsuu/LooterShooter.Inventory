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

        // Private fields: Initialization.
        //private Inventory _containingInventory;
        private CanvasGroup _draggingCanvasGroup;

        // Private fields: Runtime.
        private DraggableItemReceiverObject _belowDraggableReceiver;
        private Canvas _temporaryOverrideCanvas;
        private Vector2Int _position;
        private Vector2 _dragStartCursorPosition;
        private Vector2 _dragStartObjectPosition;
        private float _targetContentRotation;
        private bool _isUserDragging;

        public InventoryItem InventoryItem { get; private set; }
        public RectTransform RectTransform { get; private set; }
        public ItemRotation Rotation { get; private set; }


        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            _draggingCanvasGroup = GetComponent<CanvasGroup>();
        }


        private void Update()
        {
            // Smoothly rotate the item to target rotation.
            _contentsRoot.localRotation = Quaternion.Slerp(
                _contentsRoot.localRotation, Quaternion.Euler(0f, 0f, _targetContentRotation),
                Time.deltaTime * _rotationSpeed);

            if (!_isUserDragging)
                return;

            RectTransform.SetAsLastSibling();

            if (!Input.GetKeyDown(KeyCode.R))
                return;

            UpdateRotation(Rotation.NextRotation());
            UpdateSize();

            // Update validator.
            UpdateHighlighterSize();
            UpdateHighlighterPosition();
        }


        public void Initialize(InventoryItem inventoryItem)
        {
            gameObject.name = $"{nameof(DraggableItem)}: {inventoryItem.Metadata.ItemData.Name}";
            InventoryItem = inventoryItem;

            _itemImage.sprite = inventoryItem.Metadata.ItemData.Sprite;

            ResetVisuals();

            // Rotate instantly to target rotation, so that items won't start spinning when opening the inventory :D.
            _contentsRoot.localRotation = Quaternion.Euler(0f, 0f, _targetContentRotation);
        }


        public InventoryBounds GetBounds(RectTransform positionRelativeTo)
        {
            // Get the top-left corner position and convert it to inventory grid position..
            Vector2 topLeftCorner =
                Utilities.GetAnchoredPositionRelativeToRect(RectTransform.position, positionRelativeTo);
            Vector2Int inventoryGridPosition = Utilities.GetInventoryGridPosition(topLeftCorner);

            InventoryBounds bounds = new(InventoryItem.Metadata.ItemData, inventoryGridPosition, Rotation);
            return bounds;
        }


        // public bool IsBoundsValid()
        // {
        //     if (_belowInventoryRenderer == null)
        //         return false;
        //     
        //     return _belowInventoryRenderer.TargetInventory.IsBoundsValid(GetBounds(_belowInventoryRenderer.EntityRootTransform), _itemReference);
        // }


        private void ResetVisuals()
        {
            UpdateRotation(InventoryItem.RotationInInventory);
            UpdateSize();
            UpdatePosition();
        }


        private void UpdateRotation(ItemRotation newRotation)
        {
            // Set the new rotation.
            Rotation = newRotation;

            // Image rotation:
            _targetContentRotation = Rotation.AsDegrees();
        }


        private void UpdateSize()
        {
            // Root object size:
            // If the object is rotated, we need to flip width and height.
            bool isRotated = Rotation.ShouldFlipWidthAndHeight();
            int itemSizeX = InventoryItem.Metadata.ItemData.InventorySizeX;
            int itemSizeY = InventoryItem.Metadata.ItemData.InventorySizeY;
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
            Vector2 sizeDelta = RectTransform.sizeDelta;
            Vector2 positionAdjustment =
                new Vector2(-(rootNewSize.x - sizeDelta.x), rootNewSize.y - sizeDelta.y) * 0.5f;

            // Adjust size.
            sizeDelta = rootNewSize;
            RectTransform.sizeDelta = sizeDelta;
            _contentsRoot.sizeDelta = contentsNewSize;

            // Adjust position.
            RectTransform.anchoredPosition += positionAdjustment;
        }


        private void UpdatePosition()
        {
            // Set the new position.
            _position = InventoryItem.Bounds.Position;

            // Move to new position.
            float posX = _position.x * Utilities.INVENTORY_SLOT_SIZE;
            float posY = -(_position.y * Utilities.INVENTORY_SLOT_SIZE);
            RectTransform.anchoredPosition = new Vector3(posX, posY, 0);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            _draggingCanvasGroup.blocksRaycasts = false;
            _isUserDragging = true;

            _temporaryOverrideCanvas = gameObject.AddComponent<Canvas>();
            _temporaryOverrideCanvas.overrideSorting = true;
            _temporaryOverrideCanvas.sortingOrder = 9999;

            _dragStartCursorPosition = eventData.position;
            _dragStartObjectPosition = RectTransform.position;

            // Update validator.
            UpdateHighlighterSize();
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
            RectTransform.position = targetObjectPosition;
            _belowDraggableReceiver = GetDraggableReceiverBelow();
            UpdateHighlighterPosition();
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            _isUserDragging = false;
            _draggingCanvasGroup.blocksRaycasts = true;
            DraggableItemHighlighter.Singleton.Hide();

            if (_temporaryOverrideCanvas != null)
                Destroy(_temporaryOverrideCanvas);

            // Request the inventory to move this entity.
            // RequestMove();
            // NOTE: This is now handled in DraggableItemReceiverObject.cs.

            if (_belowDraggableReceiver != null)
                _belowDraggableReceiver.OnEndDrag(this);

            ResetVisuals();

            //DraggableItemReceiverObject receiver = Utilities.GetFirstComponentBelow<DraggableItemReceiverObject>(RectTransform.position);
            //if(receiver != null)
            //    receiver.OnEndDrag(this);
        }


        private void UpdateHighlighterSize()
        {
            Vector2 sizeDelta = RectTransform.sizeDelta;
            DraggableItemHighlighter.Singleton.UpdateSize(sizeDelta.x, sizeDelta.y);
        }


        private void UpdateHighlighterPosition()
        {
            DraggableItemHighlighter.Singleton.UpdatePosition(this, _belowDraggableReceiver);
        }


        [CanBeNull]
        private DraggableItemReceiverObject GetDraggableReceiverBelow()
        {
            int itemSizeX = InventoryItem.Metadata.ItemData.InventorySizeX;
            int itemSizeY = InventoryItem.Metadata.ItemData.InventorySizeY;
            const float halfCellSize = Utilities.INVENTORY_SLOT_SIZE / 2f;
            int centerX = itemSizeX / 2;
            int centerY = itemSizeY / 2;

            float currentPosX = RectTransform.position.x;
            float currentPosY = RectTransform.position.y;
            switch (itemSizeX % 2)
            {
                case 1 when itemSizeY % 2 == 1:
                {
                    // Item size is odd in both dimensions, only check the center cell.
                    Vector2 cellCenter = new(
                        currentPosX + halfCellSize + centerX * Utilities.INVENTORY_SLOT_SIZE,
                        currentPosY - (halfCellSize + centerY * Utilities.INVENTORY_SLOT_SIZE));

                    DraggableItemReceiverObject current =
                        Utilities.GetFirstComponentBelow<DraggableItemReceiverObject>(cellCenter);

                    if (current != null)
                        return current;
                    break;
                }
                case 0 when itemSizeY % 2 == 0:
                {
                    // Item size is even in both dimensions, all cells are checked.
                    for (int y = 0; y < itemSizeY; y++)
                    for (int x = 0; x < itemSizeX; x++)
                    {
                        Vector2 cellCenter = new(
                            currentPosX + halfCellSize + x * Utilities.INVENTORY_SLOT_SIZE,
                            currentPosY - (halfCellSize + y * Utilities.INVENTORY_SLOT_SIZE));

                        DraggableItemReceiverObject current =
                            Utilities.GetFirstComponentBelow<DraggableItemReceiverObject>(cellCenter);

                        if (current != null)
                            return current;
                    }

                    break;
                }

                // Item size is odd in one dimension, middle cells are checked.
                case 1:
                {
                    // Check the middle column.
                    for (int y = 0; y < itemSizeY; y++)
                    {
                        Vector2 cellCenter = new(
                            currentPosX + halfCellSize + centerX * Utilities.INVENTORY_SLOT_SIZE,
                            currentPosY - (halfCellSize + y * Utilities.INVENTORY_SLOT_SIZE));

                        DraggableItemReceiverObject current =
                            Utilities.GetFirstComponentBelow<DraggableItemReceiverObject>(cellCenter);

                        if (current != null)
                            return current;
                    }

                    break;
                }
                default:
                {
                    // Check the middle row.
                    for (int x = 0; x < itemSizeX; x++)
                    {
                        Vector2 cellCenter = new(
                            currentPosX + halfCellSize + x * Utilities.INVENTORY_SLOT_SIZE,
                            currentPosY - (halfCellSize + centerY * Utilities.INVENTORY_SLOT_SIZE));

                        DraggableItemReceiverObject current =
                            Utilities.GetFirstComponentBelow<DraggableItemReceiverObject>(cellCenter);

                        if (current != null)
                            return current;
                    }

                    break;
                }
            }

            return null;
        }


        /* This variant checks all cells from the center outwards.
         
         private DraggableItemReceiverObject GetDraggableReceiverBelow()
        {
            int itemSizeX = InventoryItem.Metadata.ItemData.InventorySizeX;
            int itemSizeY = InventoryItem.Metadata.ItemData.InventorySizeY;
            const float halfCellSize = Utilities.INVENTORY_SLOT_SIZE / 2f;
            int centerX = itemSizeX / 2;
            int centerY = itemSizeY / 2;

            for (int distance = 0; distance <= Mathf.Max(centerX, centerY); distance++)
            {
                int startX = centerX - distance;
                int startY = centerY - distance;
                int endX = centerX + distance;
                int endY = centerY + distance;

                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        Vector2 cellCenter = new(
                            RectTransform.position.x + halfCellSize + x * Utilities.INVENTORY_SLOT_SIZE,
                            RectTransform.position.y - (halfCellSize + y * Utilities.INVENTORY_SLOT_SIZE));

                        DraggableItemReceiverObject current = Utilities.GetFirstComponentBelow<DraggableItemReceiverObject>(cellCenter);

                        if (current != null)
                            return current;
                    }
                }
            }

            return null;
        }*/

        /*  This variant checks all cells, looping from top-left to bottom-right.
         
         private DraggableItemReceiverObject GetDraggableReceiverBelow()
        {
            // Raycast all cells to check if they have the same inventory.
            int itemSizeX = InventoryItem.Metadata.ItemData.InventorySizeX;
            int itemSizeY = InventoryItem.Metadata.ItemData.InventorySizeY;
            const float halfCellSize = Utilities.INVENTORY_SLOT_SIZE / 2f;

            for (int y = 0; y < itemSizeY; y++)
            {
                for (int x = 0; x < itemSizeX; x++)
                {
                    Vector2 cellCenter = new(
                        RectTransform.position.x + halfCellSize + x * Utilities.INVENTORY_SLOT_SIZE,
                        RectTransform.position.y - (halfCellSize + y * Utilities.INVENTORY_SLOT_SIZE));
                    
                    DraggableItemReceiverObject current = Utilities.GetFirstComponentBelow<DraggableItemReceiverObject>(cellCenter);

                    if (current != null)
                        return current;
                }
            }

            return null;
        }*/
    }
}