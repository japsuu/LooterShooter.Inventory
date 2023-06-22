using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Spatial;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(Image))]
    public class InventorySlotsRenderer : MonoBehaviour, IItemDropTarget
    {
        private Image _slotsImage;
        private SpatialInventoryRenderer _inventoryRenderer;
        
        public Inventory Inventory => _inventoryRenderer.TargetSpatialInventory;
        public RectTransform RectTransform => _inventoryRenderer.EntityRootTransform;


        private void Awake()
        {
            _slotsImage = GetComponent<Image>();
        }


        public void Initialize(SpatialInventoryRenderer spatialInventoryRenderer, float width, float height)
        {
            _inventoryRenderer = spatialInventoryRenderer;
            
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
        

        public bool AcceptsItem(InventoryItem item, InventoryBounds itemBounds)
        {
            return _inventoryRenderer.TargetSpatialInventory.IsBoundsValid(itemBounds, item);
        }


        public bool TryReceiveItem(IItemDropTarget fromTarget, Vector2Int fromPosition, Vector2Int toPosition, ItemRotation toRotation)
        {
            return _inventoryRenderer.TargetSpatialInventory.TryMoveItem(fromPosition, toPosition, toRotation, fromTarget.Inventory);
        }
    }
}