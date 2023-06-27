using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using InventorySystem.Inventories.Spatial.Items;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Spatial.Rendering
{
    [RequireComponent(typeof(Image))]
    public class InventorySlotsRenderer : MonoBehaviour, IInventoryEntityDropTarget
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

        public bool AcceptsEntity(InventoryEntity entity)
        {
            return _inventoryRenderer.TargetSpatialInventory.IsBoundsValid(entity.GetBounds(), item);
        }


        public void OnEntityDropped(InventoryEntity entity)
        {
            throw new System.NotImplementedException();
        }


        public void OnEntityLifted(InventoryEntity entity)
        {
            throw new System.NotImplementedException();
        }


        public bool TryReceiveItem(IInventoryEntityDropTarget fromTarget, Vector2Int fromPosition, Vector2Int toPosition, ItemRotation toRotation)
        {
            return _inventoryRenderer.TargetSpatialInventory.TryMoveItem(fromPosition, toPosition, toRotation, fromTarget.Inventory);
        }
    }
}