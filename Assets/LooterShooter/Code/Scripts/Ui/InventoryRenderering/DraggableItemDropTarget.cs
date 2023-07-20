using UnityEngine;

namespace LooterShooter.Ui.InventoryRenderering
{
    /// <summary>
    /// Inherit to make the object able to accept <see cref="DraggableItem"/>s dropped on to it.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class DraggableItemDropTarget : MonoBehaviour
    {
        public RectTransform RectTransform { get; private set; }
        
        
        public abstract bool DoSnapHighlighterToGrid { get; }


        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }


        public void OnDroppedDraggableItem(DraggableItem draggedItem)
        {
            if(draggedItem == null)
                return;
            
            if(!CanDropDraggableItem(draggedItem))
                return;
            
            HandleDroppedDraggableItem(draggedItem);
        }
        
        
        /// <returns>If a specific <see cref="DraggableItem"/> can be dropped on to this object.</returns>
        public abstract bool CanDropDraggableItem(DraggableItem draggableItem);
        
        
        /// <summary>
        /// Called when a draggable item is dropped on top of this object.
        /// </summary>
        protected abstract void HandleDroppedDraggableItem(DraggableItem draggableItem);
    }
}