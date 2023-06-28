using InventorySystem.Inventories.Items;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(Image))]
    public class InventoryGridItemReceiver : InventoryItemReceiver
    {
        private Image _slotsImage;
        private SpatialInventoryRenderer _inventoryRenderer;
        
        public override RectTransform FloaterParentRectTransform => _inventoryRenderer.FloaterParentRectTransform;


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


        public override bool CanDropFloater(Floater floater)
        {
            // Accept floater if their bounds do not collide with anything.
            return _inventoryRenderer.TargetInventoryX.IsBoundsValid(floater.GetBounds(FloaterParentRectTransform), floater.FloaterData.Metadata.PositionInInventory);
        }


        public override void HandleDroppedFloater(Floater floater)
        {
            if(!CanDropFloater(floater))
                return;

            FloaterData floaterData = floater.FloaterData;
            ItemMetadataSnapshot snapshot = floaterData.Metadata;

            Vector2Int currentPosition = snapshot.PositionInInventory;
            InventoryX targetInventory = _inventoryRenderer.TargetInventoryX;
            Vector2Int targetPosition = floater.GetGridPosition(FloaterParentRectTransform);
            ItemRotation targetRotation = floaterData.Rotation;
            
            snapshot.ContainingInventory.TryMoveItem(currentPosition, targetInventory, targetPosition, targetRotation);
        }
    }
}