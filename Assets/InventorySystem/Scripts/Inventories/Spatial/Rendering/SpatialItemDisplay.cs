using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Spatial.Items;
using UnityEngine;

namespace InventorySystem.Inventories.Spatial.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    public class SpatialItemDisplay : MonoBehaviour
    {
        private RectTransform _contentsRoot;
        private RectTransform _rectTransform;


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }


        public void Initialize(Vector2Int inventoryPosition, ItemRotation inventoryRotation, InventoryItem<> data)
        {
            // Move to correct position, assuming pivot is set to the left-top corner.
            _rectTransform.anchoredPosition = new Vector2(inventoryPosition.x, -inventoryPosition.y) * Utilities.INVENTORY_SLOT_SIZE;
            
            // Rotate image content.
            _contentsRoot.localRotation = Quaternion.Euler(0f, 0f, inventoryRotation.AsDegrees());
            
            // Scale root object.
            // If the object is rotated, we need to flip width and height.
            bool isRotated = inventoryRotation.ShouldFlipWidthAndHeight();
            int itemSizeX = data.Item.InventorySizeX;
            int itemSizeY = data.Item.InventorySizeY;
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
    }
}