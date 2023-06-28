using System;
using InventorySystem.Clothing;
using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem.EquipmentSlots
{
    public class EquipmentSlot : MonoBehaviour, IItemSlotObject
    {
        [SerializeField] private KeyCode _selectKey = KeyCode.Alpha1;
        [SerializeField] private ItemType _itemType;


        private void Update()
        {
            if (Input.GetKeyDown(_selectKey))
            {
                Selected();
            }
        }


        private void Selected()
        {
            
        }


        public bool IsBoundsValid(InventoryBounds itemBounds, ItemMetadata thisItemMetadata)
        {
            return true;
        }


        /*private bool IsItemAllowed(InventoryItem item)
        {
            if (_itemType == ItemType.Nothing)
                return false;
            
            if (_itemType == ItemType.Everything)
                return true;

            ItemType itemType = DetermineItemType(item);

            return _itemType == itemType;
        }


        private ItemType DetermineItemType(InventoryItem thisItem)
        {
            ItemData data = thisItem.Item;
            if (data is ClothingItem)
                return ItemType.Clothing;
        }*/
    }
}