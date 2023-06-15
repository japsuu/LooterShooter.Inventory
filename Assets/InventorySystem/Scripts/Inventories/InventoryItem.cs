using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem.Inventories
{
    public class InventoryItem
    {
        public ItemData Item { get; }
        public Vector2Int Position { get; private set; }
        public ItemRotation Rotation { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public InventoryBounds Bounds { get; private set; }
        
        public InventoryItem(ItemData item, InventoryBounds bounds, ItemRotation rotation)
        {
            Item = item;
            Bounds = bounds;
            Position = bounds.Position;
            Width = bounds.Width;
            Height = bounds.Height;
            
            Rotation = rotation;
        }


        public void UpdateBounds(InventoryBounds bounds, ItemRotation rotation)
        {
            Bounds = bounds;
            Position = bounds.Position;
            Rotation = rotation;
            Width = bounds.Width;
            Height = bounds.Height;
        }
    }
}