using InventorySystem.Inventories.Items;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class Floater : MonoBehaviour
    {
        // Serialized fields.BUG: Should contain a SpatialItemDisplay.
        [SerializeField] private RectTransform _contentsRoot;
        [SerializeField] private Image _itemImage;
        [SerializeField] private float _rotationSpeed = 20f;

        // Private fields: Initialization.
        private FloaterData _floaterData;
        private InventoryItemReceiver _containingDropTarget;
        private RectTransform _rectTransform;
        private CanvasGroup _draggingCanvasGroup;
        // Private fields: Runtime.
        private InventoryItemReceiver _belowDropTarget;
        private Canvas _temporaryOverrideCanvas;
        private Vector2Int _position;
        private Vector2 _dragStartCursorPosition;
        private Vector2 _dragStartObjectPosition;
        private float _targetContentRotation;
        private bool _isUserDragging;
        private readonly Vector3[] _rectCornersArray = new Vector3[4];

        public FloaterData FloaterData => _floaterData;  //BUG: Update


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


        public void Initialize(ItemMetadata inventoryItem, InventoryItemReceiver containingDropTarget)
        {
            _floaterData = new FloaterData(inventoryItem);
            _containingDropTarget = containingDropTarget;
            
            _itemImage.sprite = _floaterData.Metadata.ItemDataReference.Sprite;
            
            
            UpdateVisuals();

            // Rotate instantly to target rotation, so that items won't start spinning when opening the inventory :D.
            _contentsRoot.localRotation = Quaternion.Euler(0f, 0f, _targetContentRotation);
        }


        public InventoryBounds GetBounds(RectTransform rectPositionRelativeTo)
        {
            bool isRotated = FloaterData.Rotation.ShouldFlipWidthAndHeight();
            int itemSizeX = FloaterData.Metadata.ItemDataReference.InventorySizeX;
            int itemSizeY = FloaterData.Metadata.ItemDataReference.InventorySizeY;
            int itemWidth = isRotated ? itemSizeY : itemSizeX;
            int itemHeight = isRotated ? itemSizeX : itemSizeY;

            // Get the top-left corner position.
            Vector2 topLeftCorner = GetAnchoredPositionRelativeToRect(rectPositionRelativeTo);
            
            InventoryBounds bounds = new(Utilities.GetInventoryGridPosition(topLeftCorner), itemWidth, itemHeight);
            return bounds;
        }
        
        
        public Vector2 GetAnchoredPositionRelativeToRect(RectTransform relativeTo)
        {
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, _rectTransform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(relativeTo, screenP, null, out Vector2 relativePoint);

            return relativePoint;
        }
        
        /// <summary>
        /// Position in the inventory grid relative to the given RectTransform.
        /// </summary>
        public Vector2Int GetGridPosition(RectTransform rectPositionRelativeTo)
        {
            Vector2 topLeftCorner = GetAnchoredPositionRelativeToRect(rectPositionRelativeTo);
            return Utilities.GetInventoryGridPosition(topLeftCorner);
        }


        public bool IsBoundsValid()
        {
            if (_belowDropTarget == null)
                return false;
            
            return _belowDropTarget.CanDropFloater(this);
        }


        private void UpdateVisuals()
        {
            UpdateRotation(_floaterData.Rotation);
            UpdateSize();
            UpdatePosition(_floaterData.Metadata.Bounds.Position);
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
            int itemSizeX = _floaterData.Metadata.ItemData.InventorySizeX;
            int itemSizeY = _floaterData.Metadata.ItemData.InventorySizeY;
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

        private void UpdatePosition(Vector2Int newPosition)
        {
            // Set the new position.
            _position = newPosition;

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
            _belowDropTarget = GetDropTargetBelow();
            UpdateValidatorPosition();
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            _isUserDragging = false;
            _draggingCanvasGroup.blocksRaycasts = true;
            FloaterValidator.Singleton.Hide();

            if (_temporaryOverrideCanvas != null)
                Destroy(_temporaryOverrideCanvas);

            // Request the inventory to move this entity.
            RequestMove();
        }


        private void UpdateValidatorSize()
        {
            Vector2 sizeDelta = _rectTransform.sizeDelta;
            FloaterValidator.Singleton.UpdateSize(sizeDelta.x, sizeDelta.y);
        }


        private void UpdateValidatorPosition()
        {
            if (_belowDropTarget != null)
            {
                Vector2 relativeAnchoredPos = GetAnchoredPositionRelativeToRect(_belowDropTarget.FloaterParentRectTransform);
                Vector2 snappedPos = Utilities.SnapPositionToInventoryGrid(relativeAnchoredPos);
                Vector2 screenSpacePos = _belowDropTarget.FloaterParentRectTransform.GetScreenSpacePosition(snappedPos);

                FloaterValidator.Singleton.UpdatePosition(screenSpacePos, this);
            }
            else
            {
                Vector2 snappedPosition = Utilities.SnapPositionToInventoryGrid(_rectTransform.anchoredPosition);
                FloaterValidator.Singleton.UpdatePosition(
                    ((RectTransform)_rectTransform.parent).GetScreenSpacePosition(snappedPosition), this);
            }
        }


        private void RequestMove()
        {
            if (_belowDropTarget != null)
            {
                Vector2 relativeAnchoredPosition = GetAnchoredPositionRelativeToRect(_belowDropTarget.FloaterParentRectTransform);
                Vector2Int newPosition = Utilities.GetInventoryGridPosition(relativeAnchoredPosition);
                // print($"Request:{_position} -> {newPosition}");

                if (_belowDropTarget.TryReceiveItem(_containingDropTarget, _position, newPosition, _rotation))
                {
                    _containingDropTarget = _belowDropTarget;
                }
            }
            
            UpdateVisuals();
        }


        [CanBeNull]
        private InventoryItemReceiver GetDropTargetBelow()
        {
            // Raycast all corners to check if they have the same inventory.
            _rectTransform.GetWorldCorners(_rectCornersArray);
            
            foreach (Vector3 corner in _rectCornersArray)
            {
                InventoryItemReceiver current = Utilities.GetFirstComponentBelow<InventoryItemReceiver>(corner);

                if (current != null)
                    return current;
            }

            return null;
        }


        //[CanBeNull]
        //private SpatialInventory GetInventoryBelow() => _belowRenderer == null ? null : _belowRenderer.TargetSpatialInventory;
    }
}