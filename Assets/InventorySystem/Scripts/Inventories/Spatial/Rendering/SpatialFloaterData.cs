using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Spatial.Items;
using UnityEngine;

namespace InventorySystem.Inventories.Spatial.Rendering
{
    public class SpatialFloaterData
    {
        /// <summary>
        /// Which item this entity represents.
        /// </summary>
        public readonly InventoryItem<SpatialInventory> InventoryItem;
        
        /// <summary>
        /// Item's width in cells.
        /// </summary>
        public readonly int ItemWidth;
        
        /// <summary>
        /// Item's height in cells.
        /// </summary>
        public readonly int ItemHeight;
        
        /// <summary>
        /// Position in the current inventory grid.
        /// </summary>
        public Vector2Int Position { get; private set; }
        
        /// <summary>
        /// Rotation in the current inventory grid.
        /// </summary>
        public ItemRotation Rotation { get; private set; }
        
        /// <summary>
        /// Size on the X-axis.
        /// </summary>
        public int SizeX { get; private set; }
        
        /// <summary>
        /// Size on the Y-axis.
        /// </summary>
        public int SizeY { get; private set; }


        public SpatialFloaterData(InventoryItem<SpatialInventory> inventoryItem)
        {
            InventoryItem = inventoryItem;
            ItemWidth = inventoryItem.Item.InventorySizeX;
            ItemHeight = inventoryItem.Item.InventorySizeY;
            
            UpdateScale();
        }


        public void UpdatePosition(Vector2Int newPos)
        {
            Position = newPos;
        }


        public void UpdateRotation(ItemRotation newRot)
        {
            Rotation = newRot;
            UpdateScale();
        }


        private void UpdateScale()
        {
            if (Rotation.ShouldFlipWidthAndHeight())
            {
                SizeX = ItemHeight;
                SizeY = ItemWidth;
            }
            else
            {
                SizeX = ItemWidth;
                SizeY = ItemHeight;
            }
        }
    }
}