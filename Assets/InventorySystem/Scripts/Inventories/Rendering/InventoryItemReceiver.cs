using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    public abstract class InventoryItemReceiver : MonoBehaviour
    {
        public abstract RectTransform FloaterParentRectTransform { get; }


        public abstract bool CanDropFloater(Floater floater);


        /// <summary>
        /// Called when a floater is dropped on top of this object.
        /// This is where you need to transfer it between inventories.
        /// </summary>
        public abstract void HandleDroppedFloater(Floater floater);
    }
}