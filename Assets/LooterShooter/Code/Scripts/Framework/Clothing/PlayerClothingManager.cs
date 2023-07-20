using System;
using System.Collections.Generic;
using LooterShooter.Framework.Inventories;
using LooterShooter.Framework.Inventories.Items;

namespace LooterShooter.Framework.Clothing
{
    /// <summary>
    /// Controls equipping and removing clothes.
    /// Has an <see cref="EquippedClothesChanged"/> event, that is subscribed by <see cref="PlayerInventoryManager"/> to add inventories for the equipped clothes.
    /// </summary>
    public class PlayerClothingManager : SingletonBehaviour<PlayerClothingManager>
    {
        public static event Action<ClothingType, ItemMetadata> EquippedClothesChanged;

        private Dictionary<ClothingType, ItemMetadata> _equippedClothingItems;

        
        private void Awake()
        {
            _equippedClothingItems = new();
        }


        public void RequestEquipClothes(ItemMetadata clothes)
        {
            if(TryEquipClothes(clothes, out ClothingType type))
                EquippedClothesChanged?.Invoke(type, _equippedClothingItems[type]);
        }


        public void RequestRemoveClothes(ClothingType type)
        {
            if(TryRemoveClothes(type))
                EquippedClothesChanged?.Invoke(type, null);
        }


        private bool TryEquipClothes(ItemMetadata itemData, out ClothingType type)
        {
            ClothingItemData clothingData = itemData.ItemData as ClothingItemData;

            if (clothingData == null)
            {
                Logger.Write(LogLevel.ERROR, $"Tried to equip clothes from item ({itemData.ItemData.ItemName}) that wasn't an clothing.");
                type = ClothingType.Hat;
                return false;
            }

            type = clothingData.ClothingType;
            
            // If we already have clothes of the same type equipped, remove them first.
            if (_equippedClothingItems.TryGetValue(clothingData.ClothingType, out ItemMetadata equippedClothingItem))
            {
                Logger.Write(LogLevel.DEBUG, $"Already have clothes of type {clothingData.ClothingType} equipped ({equippedClothingItem.ItemData.ItemName}), removing them first.");
                if(!TryRemoveClothes(clothingData.ClothingType))
                {
                    Logger.Write(LogLevel.ERROR, "Unknown error happened with removing existing clothes!");
                    return false;
                }
            }

            _equippedClothingItems[clothingData.ClothingType] = itemData;
            
            return true;
        }


        private bool TryRemoveClothes(ClothingType type)
        {
            if (_equippedClothingItems.Remove(type))
                return true;
            
            Logger.Write(LogLevel.WARN, $"Cannot remove clothes of type {type}: there's nothing equipped!");
            return false;
        }
    }
}