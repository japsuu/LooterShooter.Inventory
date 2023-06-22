using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Spatial;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    /// <summary>
    /// Inherited by UI objects that can accept items dropped on to them.
    /// </summary>
    public interface IItemDropTarget
    {
        public Inventory Inventory { get; }
        
        public RectTransform RectTransform { get; }
        
        
        public bool AcceptsItem(InventoryItem item, InventoryBounds itemBounds);
        
        
        public bool TryReceiveItem(IItemDropTarget fromTarget, Vector2Int fromPosition, Vector2Int toPosition, ItemRotation toRotation);
    }
}