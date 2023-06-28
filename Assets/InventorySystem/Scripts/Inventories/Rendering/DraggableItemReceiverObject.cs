using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem.Inventories.Rendering
{
    public abstract class DraggableItemReceiverObject : MonoBehaviour, IEndDragHandler
    {
        public void OnEndDrag(PointerEventData eventData)
        {
            DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            
            if(draggedItem == null)
                return;
            
            HandleDroppedDraggableItem(draggedItem);
        }
        
        
        public abstract bool CanDropDraggableItem(DraggableItem draggableItem);
        
        
        /// <summary>
        /// Called when a floater is dropped on top of this object.
        /// This is where you need to transfer it between inventories.
        /// </summary>
        protected abstract void HandleDroppedDraggableItem(DraggableItem draggableItem);
    }
}