using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class DraggableItemReceiverObject : MonoBehaviour
    {
        public RectTransform RectTransform { get; private set; }


        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }


        public void OnEndDrag(DraggableItem draggedItem)
        {
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