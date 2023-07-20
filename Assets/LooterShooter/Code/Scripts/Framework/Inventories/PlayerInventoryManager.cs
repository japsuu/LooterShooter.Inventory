using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LooterShooter.Framework.Clothing;
using LooterShooter.Framework.Inventories.Items;
using LooterShooter.Framework.Saving;
using LooterShooter.Tools;
using UnityEngine;

namespace LooterShooter.Framework.Inventories
{
    /// <summary>
    /// Controls the player's <see cref="SpatialInventory"/>s.
    /// </summary>
    [DefaultExecutionOrder(-101)]
    public class PlayerInventoryManager : MonoBehaviour
    {
        // Singleton.
        public static PlayerInventoryManager Singleton;

        // Events.
        public static event Action<SpatialInventory> BaseInventoryChanged;
        public static event Action<ClothingType, ClothingInventory> EquippedClothesInventoryChanged;

        [Header("Base Inventory")]
        [SerializeField] private string _baseInventoryName = "Underwear";
        [SerializeField, Min(0)] private int _baseInventoryWidth = 4;
        [SerializeField, Min(0)] private int _baseInventoryHeight = 3;

        public SpatialInventory BaseInventory { get; private set; }
        public SortedDictionary<ClothingType, ClothingInventory> ClothingInventories { get; private set; }


        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
            
            ClothingInventories = new(new EnumValueComparer<ClothingType>());
        }


        private void Start()
        {
            PlayerSaveData saveData = SaveSystem.Singleton.GetLocalPlayerSaveData();
            if (saveData == null)
            {
                // Add new base inventory.
                BaseInventory = GetNewBaseInventory();

                BaseInventoryChanged?.Invoke(BaseInventory);
            }
            else
            {
                BaseInventory = saveData.BaseInventory;
                BaseInventoryChanged?.Invoke(BaseInventory);

                if (saveData.EquippedClothes == null)
                    return;

                foreach (ClothingInventory clothingInventory in saveData.EquippedClothes)
                {
                    ClothingItemData clothData = clothingInventory.ClothingItem.ItemData as ClothingItemData;

                    if (clothData == null)
                    {
                        Logger.Write(
                            LogLevel.ERROR,
                            $"Tried equipping saved clothes ({clothingInventory.ClothingItem.ItemData.ItemName}) that was not a ClothingItem.");
                        continue;
                    }

                    ClothingInventories[clothData.ClothingType] = clothingInventory;

                    PlayerClothingManager.Singleton.RequestEquipClothes(clothingInventory.ClothingItem);

                    EquippedClothesInventoryChanged?.Invoke(clothData.ClothingType, clothingInventory);
                }
            }
        }


        private void OnEnable()
        {
            PlayerClothingManager.EquippedClothesChanged += OnEquippedClothesChanged;
        }


        private void OnDisable()
        {
            PlayerClothingManager.EquippedClothesChanged -= OnEquippedClothesChanged;
        }


        private void OnEquippedClothesChanged(ClothingType type, [CanBeNull]ItemMetadata newClothesItemMetadata)
        {
            // Check if an inventory for the given clothing item has already been created.
            if(ClothingInventories.TryGetValue(type, out ClothingInventory existingInventory))
            {
                if (existingInventory.ClothingItem == newClothesItemMetadata)
                {
                    Logger.Write(LogLevel.DEBUG, $"Inventory already added for ClothingItem '{existingInventory.ClothingItem.ItemData.ItemName}', skipping add.");
                    return;
                }
            }
            
            bool hasExistingInventory = ClothingInventories.Remove(type, out existingInventory);
            
            if (newClothesItemMetadata == null)
            {
                // If there is no existing inventory, return.
                if (!hasExistingInventory)
                    return;
                
                // Empty the contents of the inventory to other inventories.                    
                foreach (InventoryItem item in existingInventory.Inventory.GetAllItems())
                {
                    // Try adding the item to other inventories.
                    if (TryAddItems(item.Metadata, 1).Count >= 1)
                        continue;

                    //TODO: Drop items to ground or something?
                    Logger.Write(LogLevel.WARN, $"{nameof(SpatialInventory)}: {gameObject.name}", $"NotImplemented: DropItem ({item.Metadata.ItemData.ItemName})");
                }
                EquippedClothesInventoryChanged?.Invoke(type, null);
                return;
            }

            ClothingItemData clothingData = newClothesItemMetadata.ItemData as ClothingItemData;

            if (clothingData == null)
            {
                Logger.Write(LogLevel.ERROR, $"Tried to add inventory for equipped clothes from item ({newClothesItemMetadata.ItemData.ItemName}) that wasn't an clothing.");
                return;
            }

            // Try adding the new inventory if required.
            if (clothingData.ContainedInventoryWidth <= 0 || clothingData.ContainedInventoryHeight <= 0)
            {
                EquippedClothesInventoryChanged?.Invoke(type, null);
                return;
            }
            
            ClothingInventory newInventory = GetNewClothingInventory(newClothesItemMetadata);

            // If we replaced an inventory.
            if (hasExistingInventory)
            {
                // Empty the contents of the inventory to other inventories.                    
                foreach (InventoryItem item in existingInventory.Inventory.GetAllItems())
                {
                    int itemsAddedToReplacingInventory = newInventory.Inventory.AddItems(item.Metadata, 1).Count;
                    if (itemsAddedToReplacingInventory >= 1)
                        continue;

                    int itemsAddedToOtherInventories = TryAddItems(item.Metadata, 1).Count;
                    if (itemsAddedToOtherInventories >= 1)
                        continue;

                    //TODO: Drop items to ground or something?
                    Logger.Write(LogLevel.WARN, $"{nameof(SpatialInventory)}: {gameObject.name}", "NotImplemented: DropItem.");
                }
            }

            ClothingInventories[type] = newInventory;
            EquippedClothesInventoryChanged?.Invoke(type, newInventory);
        }

        
        public List<InventoryItem> TryAddItems(ItemMetadata metadata, int count)
        {
            List<InventoryItem> results = new();
            
            results.AddRange(BaseInventory.AddItems(metadata, count));
            
            foreach (ClothingInventory clothes in ClothingInventories.Values)
            {
                if (results.Count == count)
                    return results;
                
                results.AddRange(clothes.Inventory.AddItems(metadata, count));
            }

            return results;
        }


        /// <summary>
        /// Tries to remove the given amount of defined itemData.
        /// We're using pure <see cref="ItemData"/> instead of <see cref="ItemMetadata"/>, since when removing items we probably do not care about the metadata of an item.
        /// </summary>
        /// <returns>Removed items.</returns>
        public List<InventoryItem> TryRemoveItems(ItemData data, int count)
        {
            List<InventoryItem> results = new();
            
            results.AddRange(BaseInventory.RemoveItems(data, count));
            
            foreach (ClothingInventory clothes in ClothingInventories.Values)
            {
                if (results.Count == count)
                    return results;

                results.AddRange(clothes.Inventory.RemoveItems(data, count));
            }

            return results;
        }


        public List<InventoryItem> GetAllItemsOfType(ItemData itemData)
        {
            List<InventoryItem> results = new();
            
            results.AddRange(BaseInventory.GetAllItemsOfType(itemData));

            foreach (ClothingInventory clothes in ClothingInventories.Values)
            {
                results.AddRange(clothes.Inventory.GetAllItemsOfType(itemData));
            }

            return results;
        }


        public List<InventoryItem> GetAllItems()
        {
            List<InventoryItem> results = new();
            
            results.AddRange(BaseInventory.GetAllItems());

            foreach (ClothingInventory clothes in ClothingInventories.Values)
            {
                results.AddRange(clothes.Inventory.GetAllItems());
            }

            return results;
        }


        private static ClothingInventory GetNewClothingInventory(ItemMetadata clothingItemMetadata)
        {
            ClothingItemData clothingData = (ClothingItemData)clothingItemMetadata.ItemData;
            SpatialInventory inventory = new($"clothes_{clothingData.ClothingType.ToString()}", clothingData.ContainedInventoryWidth, clothingData.ContainedInventoryHeight);
            return new ClothingInventory(clothingItemMetadata, inventory);
        }


        private SpatialInventory GetNewBaseInventory()
        {
            if (_baseInventoryWidth <= 0 || _baseInventoryHeight <= 0)
                return null;
            
            return new SpatialInventory(_baseInventoryName, _baseInventoryWidth, _baseInventoryHeight);

        }
    }
}