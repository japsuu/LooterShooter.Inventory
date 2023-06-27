using InventorySystem.Inventories.Spatial;
using InventorySystem.Inventories.Spatial.Rendering;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    public abstract class InventoryItemReceiver : MonoBehaviour
    {
        public abstract RectTransform FloaterParentRectTransform { get; }


        public abstract bool CanDropFloater(SpatialFloater floater);


        /// <summary>
        /// Called when a floater is dropped on top of this object.
        /// This is where you need to transfer it between inventories.
        /// </summary>
        public abstract void HandleDroppedFloater(SpatialFloater floater);
    }
}