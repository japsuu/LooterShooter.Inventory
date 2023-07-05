using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine.EventSystems;

namespace InventorySystem.InventorySlots
{
    public class WeaponSlot : TypeRestrictedItemSlot, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private DraggableItem _draggedItem;
        
        
        protected override ItemType[] ItemTypeRestrictions => new[]
        {
            ItemType.WEAPON
        };
        
        
        protected override void OnItemRemoved()
        {
            throw new System.NotImplementedException();
        }


        protected override void OnItemAdded()
        {
            
        }

        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(AssignedItem == null)
                return;
            
            _draggedItem = Instantiate(PrefabReferences.Singleton.DraggableItemPrefab, RectTransform);

            _draggedItem.Initialize(AssignedItem);
            
            _draggedItem.OnBeginDrag(eventData);
        }


        public void OnDrag(PointerEventData eventData)
        {
            if(_draggedItem == null)
                return;
            
            _draggedItem.OnDrag(eventData);
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if(_draggedItem == null)
                return;
            
            _draggedItem.OnEndDrag(eventData);
        }
    }
}