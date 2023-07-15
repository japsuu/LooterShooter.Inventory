using System;
using System.Collections.Generic;
using InventorySystem.Clothing;
using InventorySystem.Inventories.Items;
using JetBrains.Annotations;
using UnityEngine;

namespace InventorySystem.Inventories
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

        // Serialized fields.
        // [Header("References")]

        // [SerializeField] private ClothingInventoryRenderer _clothingInventoryRendererPrefab;
        //[SerializeField] private RectTransform _inventoryRenderersRoot;

        // [Header("Base Inventory")]
        // [SerializeField] private string _baseInventoryName = "Pockets";
        // [SerializeField, Min(0)] private int _baseInventoryWidth = 4;
        // [SerializeField, Min(0)] private int _baseInventoryHeight = 3;
        // [SerializeField] private List<ItemData> _startingItems;

        // Privates.
        //private SpatialInventory _baseInventory;                                    //BUG: Save this.
        private SortedDictionary<ClothingType, ClothingInventory> _clothingInventories;   //BUG: Save this.

        // public bool HasSomethingInInventory => GetAllItems().Count > 0;


        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
            
            _clothingInventories = new(new EnumValueComparer<ClothingType>());
            
            // Destroy all children >:).
            // for (int i = _inventoryRenderersRoot.childCount - 1; i >= 0; i--)
            // {
            //     Destroy(_inventoryRenderersRoot.GetChild(i).gameObject);
            // }
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
            bool hasExistingInventory = _clothingInventories.Remove(type, out ClothingInventory existingInventory);
            
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
                    Logger.Log(LogLevel.WARN, $"{nameof(SpatialInventory)}: {gameObject.name}", $"NotImplemented: DropItem ({item.Metadata.ItemData.ItemName})");
                }
                EquippedClothesInventoryChanged?.Invoke(type, null);
                return;
            }

            ClothingItemData clothingData = newClothesItemMetadata.ItemData as ClothingItemData;

            if (clothingData == null)
            {
                Logger.Log(LogLevel.ERROR, $"Tried to add inventory for equipped clothes from item ({newClothesItemMetadata.ItemData.ItemName}) that wasn't an clothing.");
                return;
            }

            // Try adding the new inventory if required.
            if (clothingData.ContainedInventoryWidth <= 0 || clothingData.ContainedInventoryHeight <= 0)
            {
                EquippedClothesInventoryChanged?.Invoke(type, null);
                return;
            }
            
            ClothingInventory newInventory = GetClothingInventory(newClothesItemMetadata);

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
                    Logger.Log(LogLevel.WARN, $"{nameof(SpatialInventory)}: {gameObject.name}", "NotImplemented: DropItem.");
                }
            }

            _clothingInventories[type] = newInventory;
            EquippedClothesInventoryChanged?.Invoke(type, newInventory);
        }

        
        public List<InventoryItem> TryAddItems(ItemMetadata metadata, int count)
        {
            List<InventoryItem> results = new();
            foreach (ClothingInventory clothes in _clothingInventories.Values)
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
            foreach (ClothingInventory clothes in _clothingInventories.Values)
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
            foreach (ClothingInventory clothes in _clothingInventories.Values)
            {
                results.AddRange(clothes.Inventory.GetAllItemsOfType(itemData));
            }

            return results;
        }


        public List<InventoryItem> GetAllItems()
        {
            List<InventoryItem> results = new();
            foreach (ClothingInventory clothes in _clothingInventories.Values)
            {
                results.AddRange(clothes.Inventory.GetAllItems());
            }

            return results;
        }


        // private void Start()
        // {
        //     // Add base inventory.
        //     AddClothingInventory(_baseInventoryName, _baseInventoryWidth, _baseInventoryHeight);
        //
        //     foreach (ItemData startingItem in _startingItems)
        //     {
        //         TryAddItems(new ItemMetadata<ItemData>(startingItem), 1);
        //     }
        // }


        private ClothingInventory GetClothingInventory(ItemMetadata clothingItemMetadata)
        {
            //SpatialInventory spatialInventory = SaveSystem.Singleton.GetSpatialInventoryByName(inventoryName, width, height);

            ClothingItemData clothingData = (ClothingItemData)clothingItemMetadata.ItemData;
            SpatialInventory inventory = new($"clothes_{clothingData.ClothingType.ToString()}", clothingData.ContainedInventoryWidth, clothingData.ContainedInventoryHeight);
            return new ClothingInventory(clothingItemMetadata, inventory);
        }
    }
}