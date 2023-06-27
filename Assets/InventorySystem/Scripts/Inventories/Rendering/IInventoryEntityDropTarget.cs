using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Spatial.Rendering;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    /// <summary>
    /// Inherited by UI objects that can accept <see cref="InventoryEntity"/>s dropped on to them.
    /// </summary>
    public interface IInventoryEntityDropTarget
    {
        public RectTransform RectTransform { get; }
        
        
        public bool AcceptsEntity(InventoryEntity entity);


        public void OnEntityDropped(InventoryEntity entity);


        public void OnEntityLifted(InventoryEntity entity);
        
        
        //public bool TryReceiveItem(IItemDropTarget fromTarget, Vector2Int fromPosition, Vector2Int toPosition, ItemRotation toRotation);
    }
}