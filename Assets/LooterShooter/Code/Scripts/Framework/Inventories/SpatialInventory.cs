using System;
using System.Collections.Generic;
using System.Linq;
using LooterShooter.Framework.Inventories.Items;
using LooterShooter.Framework.Inventories.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace LooterShooter.Framework.Inventories
{
    [JsonConverter(typeof(SpatialInventoryConverter))]
    public class SpatialInventory : IInventory
    {
        // Events.
        public event Action<AddItemEventArgs> AddedItem;
        public event Action<RemoveItemEventArgs> RemovedItem;
        
        // Private fields.
        private readonly InventoryBounds _inventoryBounds;
        private readonly InventoryItem[] _contents;

        // Public fields
        public InventoryBounds Bounds => _inventoryBounds;
        public string Name { get; }


        /// <summary>
        /// Either loads the save-file with the same name, or creates a new one using the defaults if no save-file is found.
        /// </summary>
        public SpatialInventory(string inventoryName, int widthCells, int heightCells)
        {
            Name = inventoryName;
            _inventoryBounds = new InventoryBounds(widthCells, heightCells);
            _contents = new InventoryItem[widthCells * heightCells];
        }


        public void AddItems(IEnumerable<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                if(IsItemBoundsValid(item.Bounds))
                    AddItem(item);
                else
                    Logger.Write(
                        LogLevel.WARN,
                        $"{nameof(SpatialInventory)}: {Name}",
                        $"Could not add item '{item.Metadata.ItemData.ItemName}' as it has invalid bounds.");
            }
        }


        public IEnumerable<InventoryItem> GetAllItems()
        {
            return _contents.Where(item => item != null);
        }


        public IEnumerable<InventoryItem> GetAllItemsOfType(ItemData itemData)
        {
            List<InventoryItem> results = new();
            foreach (InventoryItem inventoryItem in _contents)
            {
                if (inventoryItem.Metadata.ItemData == itemData)
                    results.Add(inventoryItem);
            }

            return results;
        }


        //NOTE: This should only be called by the server.
        public List<InventoryItem> AddItems(ItemMetadata metadata, int count)
        {
            return TryAddItems(metadata, count);
        }


        //NOTE: This should only be called by the server.
        public List<InventoryItem> RemoveItems(ItemData data, int count)
        {
            return TryRemoveItems(data, count);
        }


        /// <returns>If given item is inside the inventory and does not overlap with any other item.</returns>
        public bool IsItemBoundsValid(InventoryBounds itemBounds, InventoryBounds? existingBoundsToIgnore = null)
        {
            // Check if the item fits within the inventory bounds.
            if (!_inventoryBounds.Contains(itemBounds))
            {
                return false;
            }

            // Check if there are any overlapping items.
            foreach (InventoryItem item in _contents)
            {
                if(item == null)
                    continue;
                
                if(item.Bounds == existingBoundsToIgnore)
                    continue;

                if (!itemBounds.OverlapsWith(item.Bounds))
                    continue;
                
                return false;
            }

            return true;
        }


        /// <returns><see cref="InventoryItem"/>s of the <see cref="ItemData"/>s that were added.</returns>
        private List<InventoryItem> TryAddItems(ItemMetadata metadata, int count)
        {
            List<InventoryItem> results = new();
            
            if (metadata == null || metadata.ItemData == null)
            {
                Logger.Write(LogLevel.WARN, $"{nameof(SpatialInventory)}: {Name}", "Tried to add item with NULL ItemData.");
                return results;
            }


            for (int i = 0; i < count; i++)
            {
                if (!TryCreateNewInventoryItem(metadata, out InventoryItem newInventoryItem))
                {
                    Logger.Write(LogLevel.DEBUG, $"{nameof(SpatialInventory)}: {Name}", "Not enough space in the inventory!");
                    return results;
                }
            
                AddItem(newInventoryItem);
                results.Add(newInventoryItem);
            }

            return results;
        }

        
        /// <returns><see cref="InventoryItem"/>s of the <see cref="ItemData"/>s that were removed.</returns>
        private List<InventoryItem> TryRemoveItems(ItemData data, int count)
        {
            List<InventoryItem> results = new();
            
            foreach (InventoryItem inventoryItem in _contents)
            {
                if(data == null)
                    continue;

                if (inventoryItem.Metadata.ItemData != data)
                    continue;
                
                if (results.Count == count)
                    return results;
                
                RemoveItem(inventoryItem.Bounds.Position);
                results.Add(inventoryItem);
            }

            return results;
        }


        private bool TryCreateNewInventoryItem(ItemMetadata metadata, out InventoryItem createdInventoryItem)
        {
            foreach (Vector2Int position in _inventoryBounds.AllPositionsWithin())
            {
                if (TryCreateNewInventoryItem(metadata, position, InventoryItemRotation.DEG_0, null, out createdInventoryItem))
                    return true;
                
                if (TryCreateNewInventoryItem(metadata, position, InventoryItemRotation.DEG_90, null, out createdInventoryItem))
                    return true;
            }

            createdInventoryItem = null;
            return false;
        }


        public bool TryCreateNewInventoryItem(ItemMetadata metadata, Vector2Int position, InventoryItemRotation rotation, InventoryBounds? boundsToIgnore, out InventoryItem createdInventoryItem)
        {
            InventoryBounds createdBounds = new(metadata.ItemData, position, rotation);

            if (!IsItemBoundsValid(createdBounds, boundsToIgnore))
            {
                createdInventoryItem = null;
                return false;
            }

            createdInventoryItem = new InventoryItem(metadata, createdBounds, rotation, this);
            return true;
        }


        public void AddItem(InventoryItem item)
        {
            _contents[PositionToIndex(item.Bounds.Position)] = item;
            
            Logger.Write(LogLevel.DEBUG, $"Inventory '{Name}' added {item.Metadata.ItemData.ItemName}@{item.Bounds.Position}");
            
            AddedItem?.Invoke(new AddItemEventArgs(item));
        }
        
        
        public void RemoveItem(Vector2Int itemPosition)
        {
            InventoryItem removedItem = _contents[PositionToIndex(itemPosition)];
            _contents[PositionToIndex(itemPosition)] = null;
            
            Logger.Write(LogLevel.DEBUG, $"Inventory '{Name}' removed {removedItem.Metadata.ItemData.ItemName}@{itemPosition}");
            
            RemovedItem?.Invoke(new RemoveItemEventArgs(removedItem));
        }
        
        
        private int PositionToIndex(Vector2Int pos) => pos.y * _inventoryBounds.Width + pos.x;

        
        public override string ToString()
        {
            return $"Inventory '{Name}' ({Bounds.Width}x{Bounds.Height})";
        }
    }
}