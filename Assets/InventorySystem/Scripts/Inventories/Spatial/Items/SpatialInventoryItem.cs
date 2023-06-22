using InventorySystem.Inventories.Spatial;
using UnityEngine;

namespace InventorySystem.Inventories.Items
{
    public class SpatialInventoryItem : InventoryItem
    {
        public Vector2Int Position { get; private set; }
        public ItemRotation Rotation { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public InventoryBounds Bounds { get; private set; }


        public SpatialInventoryItem(ItemData item, InventoryBounds bounds, ItemRotation rotation) : base(item)
        {
            Bounds = bounds;
            Position = bounds.Position;
            Width = bounds.Width;
            Height = bounds.Height;
            
            Rotation = rotation;
        }


        //TODO: Update to only take in position and rotation. Rest can be calculated manually here.
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