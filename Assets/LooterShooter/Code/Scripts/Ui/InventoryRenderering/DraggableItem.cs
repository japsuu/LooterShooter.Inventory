using JetBrains.Annotations;
using LooterShooter.Framework.Inventories;
using LooterShooter.Framework.Inventories.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LooterShooter.Ui.InventoryRenderering
{
    /// <summary>
    /// Visual representation of an InventoryItem, that can be dragged around.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // Serialized fields.
        [SerializeField] private RectTransform _contentsRoot;
        [SerializeField] private Image _itemImage;
        [SerializeField] private float _rotationSpeed = 20f;

        // Private fields: Initialization.
        private CanvasGroup _draggingCanvasGroup;

        // Private fields: Runtime.
        private DraggableItemDropTarget _belowDraggableReceiver;
        private Canvas _temporaryOverrideCanvas;
        private Vector2 _dragStartCursorOffset;
        private float _targetContentRotation;
        private bool _isUserDragging;
        private bool _destroyWhenDropped;

        public InventoryItem InventoryItem { get; private set; }
        public RectTransform RectTransform { get; private set; }
        public InventoryItemRotation Rotation { get; private set; }


        public void Initialize(InventoryItem inventoryItem, bool destroyWhenDropped)
        {
            RectTransform = GetComponent<RectTransform>();
            _draggingCanvasGroup = GetComponent<CanvasGroup>();
            
            _destroyWhenDropped = destroyWhenDropped;
            gameObject.name = $"{nameof(DraggableItem)}: {inventoryItem.Metadata.ItemData.ItemName}";
            InventoryItem = inventoryItem;

            _itemImage.sprite = inventoryItem.Metadata.ItemData.Sprite;

            ResetVisuals();

            // Rotate instantly to target rotation, so that items won't start spinning when opening the inventory :D.
            _contentsRoot.localRotation = Quaternion.Euler(0f, 0f, _targetContentRotation);
        }


        public Vector2 GetTopLeftCorner()
        {
            // Calculate the half width and half height of the RectTransform.
            Rect rect = RectTransform.rect;
            float rectHalfWidth = rect.width * 0.5f;
            float rectHalfHeight = rect.height * 0.5f;

            // Calculate the top-left corner relative to the positionRelativeTo RectTransform.
            Vector2 rectTopLeftCorner = (Vector2)RectTransform.position - new Vector2(rectHalfWidth, -rectHalfHeight);
            
            return rectTopLeftCorner;
        }


        public Vector2 GetTopLeftCornerRelativeToRect(RectTransform positionRelativeTo)
        {
            Vector2 relativePoint = InventoryUtilities.GetAnchoredPositionRelativeToRect(RectTransform.position, positionRelativeTo);

            // Calculate the half width and half height of the RectTransform.
            Rect rect = RectTransform.rect;
            float rectHalfWidth = rect.width * 0.5f;
            float rectHalfHeight = rect.height * 0.5f;

            // Calculate the top-left corner relative to the positionRelativeTo RectTransform.
            Vector2 relativeRectTopLeftCorner = relativePoint - new Vector2(rectHalfWidth, -rectHalfHeight);
            
            return relativeRectTopLeftCorner;
        }


        public InventoryBounds GetBounds(RectTransform positionRelativeTo)
        {
            // Get the top-left corner position and convert it to inventory grid position.
            Vector2 topLeftCorner = GetTopLeftCornerRelativeToRect(positionRelativeTo);
            Vector2Int inventoryGridPosition = InventoryUtilities.GetInventoryGridPosition(topLeftCorner);

            InventoryBounds bounds = new(InventoryItem.Metadata.ItemData, inventoryGridPosition, Rotation);
            return bounds;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            _draggingCanvasGroup.blocksRaycasts = false;
            _isUserDragging = true;

            _temporaryOverrideCanvas = gameObject.AddComponent<Canvas>();
            _temporaryOverrideCanvas.overrideSorting = true;
            _temporaryOverrideCanvas.sortingOrder = 9999;

            _dragStartCursorOffset = Input.mousePosition - RectTransform.position;

            // Update validator.
            UpdateHighlighterSize();
        }


        public void OnDrag(PointerEventData eventData) { }


        public void OnEndDrag(PointerEventData eventData)
        {
            _isUserDragging = false;
            _draggingCanvasGroup.blocksRaycasts = true;
            DraggableItemHighlighter.Singleton.Hide();

            if (_temporaryOverrideCanvas != null)
                Destroy(_temporaryOverrideCanvas);

            // Request the receiver to handle this entity.
            if (_belowDraggableReceiver != null)
                _belowDraggableReceiver.OnDroppedDraggableItem(this);

            ResetVisuals();

            if (_destroyWhenDropped)
            {
                Destroy(gameObject);
            }
        }


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
            
            MoveToMousePosition();

            RectTransform.SetAsLastSibling();

            if (!Input.GetKeyDown(KeyCode.R))
                return;

            UpdateRotation(Rotation.NextRotation());
            UpdateSize();

            // Update validator.
            UpdateHighlighterSize();
            UpdateHighlighterPosition();
        }


        private void ResetVisuals()
        {
            UpdateRotation(InventoryItem.RotationInInventory);
            UpdateSize();
            ResetPosition();
        }


        private void UpdateRotation(InventoryItemRotation newRotation)
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
            float rootWidth = itemWidth * InventoryUtilities.INVENTORY_SLOT_SIZE;
            float rootHeight = itemHeight * InventoryUtilities.INVENTORY_SLOT_SIZE;
            float contentsWidth = itemSizeX * InventoryUtilities.INVENTORY_SLOT_SIZE;
            float contentsHeight = itemSizeY * InventoryUtilities.INVENTORY_SLOT_SIZE;
            Vector2 rootNewSize = new(rootWidth, rootHeight);
            Vector2 contentsNewSize = new(contentsWidth, contentsHeight);

            // Adjust size.
            RectTransform.sizeDelta = rootNewSize;
            _contentsRoot.sizeDelta = contentsNewSize;
        }


        private void MoveToMousePosition()
        {
            RectTransform.position = (Vector2)Input.mousePosition - _dragStartCursorOffset;

            _belowDraggableReceiver = GetDraggableReceiverBelow();
            UpdateHighlighterPosition();
        }


        private Vector2 GetCenterPositionAsAnchoredPosition()
        {
            Vector2 sizeDelta = RectTransform.sizeDelta;
            float itemWidthUnits = sizeDelta.x;
            float itemHeightUnits = sizeDelta.y;

            Vector2 leftCornerPosition = new(
                InventoryItem.Bounds.Position.x * InventoryUtilities.INVENTORY_SLOT_SIZE,
                InventoryItem.Bounds.Position.y * -InventoryUtilities.INVENTORY_SLOT_SIZE);

            Vector2 centerPosition = leftCornerPosition + new Vector2(itemWidthUnits / 2, -itemHeightUnits / 2);

            return centerPosition;
        }


        private void ResetPosition()
        {
            RectTransform.anchoredPosition = GetCenterPositionAsAnchoredPosition();
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
        private DraggableItemDropTarget GetDraggableReceiverBelow()
        {
            DraggableItemDropTarget current = InventoryUtilities.GetFirstComponentBelow<DraggableItemDropTarget>(RectTransform.position);

            return current;

            /*  Below is an implementation to loop all cells outwards of the item for an receiver, starting from the center.
             
             int itemSizeX = InventoryItem.Metadata.ItemData.InventorySizeX;
            int itemSizeY = InventoryItem.Metadata.ItemData.InventorySizeY;
            const float halfCellSize = Utilities.INVENTORY_SLOT_SIZE / 2f;
            //int centerX = itemSizeX / 2;
            //int centerY = itemSizeY / 2;

            float currentPosX = RectTransform.position.x;
            float currentPosY = RectTransform.position.y;
            switch (itemSizeX % 2)
            {
                case 1 when itemSizeY % 2 == 1:
                {
                    // Item size is odd in both dimensions, only check the center cell.
                    Vector2 cellCenter = new(
                        currentPosX + halfCellSize,
                        currentPosY - halfCellSize);

                    DraggableItemReceiverObject current = Utilities.GetFirstComponentBelow<DraggableItemReceiverObject>(cellCenter);

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
                            currentPosX + halfCellSize,
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
                            currentPosY - halfCellSize);

                        DraggableItemReceiverObject current =
                            Utilities.GetFirstComponentBelow<DraggableItemReceiverObject>(cellCenter);

                        if (current != null)
                            return current;
                    }

                    break;
                }
            }

            return null;*/
        }
    }
}