using System;
using System.Collections.Generic;
using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Clothing
{
    /// <summary>
    /// Controls equipping and removing <see cref="ClothingItemData"/>s.
    /// Talks to <see cref="PlayerInventoryManager"/> to add inventories for the equipped clothes.
    /// </summary>
    public class PlayerClothingManager : MonoBehaviour
    {
        public static PlayerClothingManager Singleton;

        public static event Action<ClothingType, ItemMetadata> EquippedClothesChanged;

        private Dictionary<ClothingType, ItemMetadata> _equippedClothingItems;

        
        private void Awake()
        {
            Singleton = this;
            
            _equippedClothingItems = new();
        }


        // public bool HasAnyClothesEquipped => _equippedClothingItems.Count > 0;
        // private void Start()
        // {
        //     if(HasAnyClothesEquipped || _playerInventoryManager.HasSomethingInInventory)
        //         return;
        //     
        //     // Equip starting clothes.
        //     foreach (ClothingItem clothes in _startingClothes)
        //     {
        //         TryEquipClothes(clothes);
        //     }
        // }


        public void RequestEquipClothes(ItemMetadata clothes)
        {
            if(EquipClothes(clothes, out ClothingType type))
                EquippedClothesChanged?.Invoke(type, _equippedClothingItems[type]);
        }


        public void RequestRemoveClothes(ClothingType type)
        {
            if(RemoveClothes(type))
                EquippedClothesChanged?.Invoke(type, null);
        }


        private bool EquipClothes(ItemMetadata itemData, out ClothingType type)
        {
            ClothingItemData clothingData = itemData.ItemData as ClothingItemData;

            if (clothingData == null)
            {
                Logger.Out(LogLevel.ERROR, $"Tried to equip clothes from item ({itemData.ItemData.ItemName}) that wasn't an clothing.");
                type = ClothingType.Hat;
                return false;
            }

            type = clothingData.ClothingType;
            
            // If we already have clothes of the same type equipped, remove them first.
            if (_equippedClothingItems.TryGetValue(clothingData.ClothingType, out ItemMetadata equippedClothingItem))
            {
                Logger.Out(LogLevel.DEBUG, $"Already have clothes of type {clothingData.ClothingType} equipped ({equippedClothingItem.ItemData.ItemName}), removing them first.");
                if(!RemoveClothes(clothingData.ClothingType))
                {
                    Logger.Out(LogLevel.ERROR, "Unknown error happened with removing existing clothes!");
                    return false;
                }
            }

            _equippedClothingItems[clothingData.ClothingType] = itemData;
            
            return true;
        }


        private bool RemoveClothes(ClothingType type)
        {
            if (_equippedClothingItems.Remove(type))
                return true;
            
            Logger.Out(LogLevel.WARN, $"Cannot remove clothes of type {type}: there's nothing equipped!");
            return false;
        }
    }
}