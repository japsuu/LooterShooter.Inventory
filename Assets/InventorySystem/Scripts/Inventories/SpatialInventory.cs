using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;
using Newtonsoft.Json;
using UnityEngine;

namespace InventorySystem.Inventories
{
    [JsonConverter(typeof(SpatialInventoryConverter))]
    public class SpatialInventory : IInventory
    {
        // Events.
        public event Action<AddItemEventArgs> AddedItem;
        public event Action<MoveItemEventArgs> MovedItem;
        public event Action<RemoveItemEventArgs> RemovedItem;
        
        // Private fields.
        private readonly InventoryBounds _inventoryBounds;
        private readonly InventoryItem[] _contents;

        // Public fields.
        public InventoryBounds Bounds => _inventoryBounds;
        public string Name { get; }


        /// <summary>
        /// Either loads the save-file with the same name, or creates a new one using the defaults if no save-file is found.
        /// </summary>
        public SpatialInventory(string inventoryName, int defaultWidthCells, int defaultHeightCells)
        {
            Name = inventoryName;
            _inventoryBounds = new InventoryBounds(Vector2Int.zero, defaultWidthCells, defaultHeightCells);
            _contents = new InventoryItem[defaultWidthCells * defaultHeightCells];
        }


        ~SpatialInventory()
        {
            Persistence.RegisterInventoryDestruction(this);
        }


        public void AddItems(IEnumerable<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                if(IsItemBoundsValid(item.Bounds))
                    AddInventoryItem(item);
                else
                    Logger.Log(
                        LogLevel.WARN,
                        $"{nameof(SpatialInventory)}: {Name}",
                        $"Could not add item '{item.Metadata.ItemData.Name}' as it has invalid bounds.");
            }
        }


        // public JsonSerializableSpatialInventory Serialize()
        // {
        //     List<JsonSerializableInventoryItem> items = (from item in _contents where item != null select item.Serialize()).ToList();
        //     return new JsonSerializableSpatialInventory(Name, Bounds.Width, Bounds.Height, items.ToArray());
        // }


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
        public List<InventoryItem> RequestAddItems(ItemMetadata metadata, int count)
        {
            //NOTE: Would not work in reality, since we cannot expect the server to immediately respond with changed data.
            return TryAddItems(metadata, count);
        }


        //NOTE: This should only be called by the server.
        public List<InventoryItem> RequestRemoveItems(ItemData data, int count)
        {
            //NOTE: Would not work in reality, since we cannot expect the server to immediately respond with changed data.
            return TryRemoveItems(data, count);
        }


        public void RequestMoveItem(Vector2Int oldPosition, Vector2Int newPosition, ItemRotation newRotation, IInventory targetInventory)
        {
            bool success = TryMoveItem(oldPosition, newPosition, newRotation, targetInventory);
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


        public bool IsPositionInsideInventory(Vector2Int position) => _inventoryBounds.Contains(position);

        
        public bool TryGetItemAtPosition(Vector2Int position, out InventoryItem item)
        {
            int toIndex = PositionToIndex(position);

            if (toIndex < 0 || toIndex >= _contents.Length || _contents[toIndex] == null)
            {
                item = null;
                return false;
            }

            item = _contents[toIndex];
            return true;
        }


        /// <returns><see cref="InventoryItem"/>s of the <see cref="ItemData"/>s that were added.</returns>
        private List<InventoryItem> TryAddItems(ItemMetadata metadata, int count)
        {
            List<InventoryItem> results = new();
            
            if (metadata == null || metadata.ItemData == null)
            {
                Logger.Log(LogLevel.WARN, $"{nameof(SpatialInventory)}: {Name}", "Tried to add item with NULL ItemData.");
                return results;
            }


            for (int i = 0; i < count; i++)
            {
                InventoryItem newItem = CreateNewInventoryItemForThisInventory(metadata);
            
                if (newItem == null)
                {
                    Logger.Log(LogLevel.DEBUG, $"{nameof(SpatialInventory)}: {Name}", "Not enough space in the inventory!");
                    return results;
                }
            
                AddInventoryItem(newItem);
                results.Add(newItem);
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
                
                RemoveInventoryItem(inventoryItem);
                results.Add(inventoryItem);
            }

            return results;
        }


        private InventoryItem CreateNewInventoryItemForThisInventory(ItemMetadata metadata)
        {
            foreach (Vector2Int position in _inventoryBounds.AllPositionsWithin())
            {
                InventoryBounds itemBounds = new(position, metadata.ItemData.InventorySizeX, metadata.ItemData.InventorySizeY);
                InventoryBounds itemBoundsRotated = new(position, metadata.ItemData.InventorySizeY, metadata.ItemData.InventorySizeX);

                if (IsItemBoundsValid(itemBounds))
                {
                    InventoryItem newItem = new(metadata, itemBounds, ItemRotation.DEG_0, this);
                    return newItem;
                }
                
                if (IsItemBoundsValid(itemBoundsRotated))
                {
                    InventoryItem newItem = new(metadata, itemBoundsRotated, ItemRotation.DEG_90, this);
                    return newItem;
                }
            }

            return null;
        }


        private void AddInventoryItem(InventoryItem inventoryItem)
        {
            _contents[PositionToIndex(inventoryItem.Bounds.Position)] = inventoryItem;
            
            Logger.Log(LogLevel.DEBUG, $"Inventory '{Name}' added {inventoryItem.Metadata.ItemData.Name}@{inventoryItem.Bounds.Position}");
            
            AddedItem?.Invoke(new AddItemEventArgs(inventoryItem));
        }
        
        
        private bool TryMoveItem(Vector2Int oldItemPosition, Vector2Int newItemPosition, ItemRotation newRotation, IInventory targetInventory)
        {
            if (targetInventory == null)
            {
                Logger.Log(LogLevel.WARN, $"{nameof(SpatialInventory)}: {Name}", "TargetInventory was null!");
                return false;
            }
            
            if (!IsPositionInsideInventory(oldItemPosition) || !targetInventory.IsPositionInsideInventory(newItemPosition))
            {
                Logger.Log(LogLevel.DEBUG, $"{nameof(SpatialInventory)}: {Name}", "Invalid from/to position!");
                return false;
            }
            
            // Ensure moved item exists.
            int fromIndex = PositionToIndex(oldItemPosition);
            InventoryItem movedItem = _contents[fromIndex];
            if (movedItem == null)
            {
                Logger.Log(LogLevel.WARN, $"{nameof(SpatialInventory)}: {Name}", "Moved item does not exist!");
                return false;
            }
            
            if (oldItemPosition == newItemPosition && movedItem.RotationInInventory == newRotation && targetInventory == this)
                return false;
            
            if (targetInventory.TryGetItemAtPosition(newItemPosition, out InventoryItem blockingItem) && blockingItem != movedItem)
            {
                //TODO: Implement item swapping (swap the two items with each other if possible)
                Logger.Log(LogLevel.WARN, $"{nameof(SpatialInventory)}: {Name}", "Item swapping not yet implemented!");
                return false;
            }

            bool flipWidthAndHeight = newRotation.ShouldFlipWidthAndHeight();
            InventoryBounds newBounds = flipWidthAndHeight ?
                new InventoryBounds(newItemPosition, movedItem.Metadata.ItemData.InventorySizeY, movedItem.Metadata.ItemData.InventorySizeX) :
                new InventoryBounds(newItemPosition, movedItem.Metadata.ItemData.InventorySizeX, movedItem.Metadata.ItemData.InventorySizeY);
            
            if (!targetInventory.IsItemBoundsValid(newBounds, movedItem.Bounds))
            {
                Logger.Log(LogLevel.DEBUG, $"{nameof(SpatialInventory)}: {Name}", "Bounds are overlapping something.");
                return false;
            }

            MoveInventoryItem(movedItem, targetInventory, newBounds, newRotation);

            Logger.Log(LogLevel.DEBUG, $"{nameof(SpatialInventory)}: {Name}", $"Moved '{movedItem.Metadata.ItemData.Name}' to {targetInventory.Name}!");
            return true;
        }


        private void RemoveInventoryItem(InventoryItem inventoryItem)
        {
            _contents[PositionToIndex(inventoryItem.Bounds.Position)] = null;
            
            Logger.Log(LogLevel.DEBUG, $"Inventory '{Name}' removed {inventoryItem.Metadata.ItemData.Name}@{inventoryItem.Bounds.Position}");
            
            RemovedItem?.Invoke(new RemoveItemEventArgs(inventoryItem));
        }


        private void MoveInventoryItem(InventoryItem oldInventoryItem, IInventory newInventory, InventoryBounds newBounds, ItemRotation newRotation)
        {
            Vector2Int oldPos = oldInventoryItem.Bounds.Position;
            int oldIndex = PositionToIndex(oldPos);
            
            _contents[oldIndex] = null;
            
            InventoryItem newInventoryItem = newInventory.ReceiveExistingInventoryItem(oldInventoryItem, newBounds, newRotation);
            
            Logger.Log(LogLevel.DEBUG, $"Inventory '{Name}' moved {oldInventoryItem.Metadata.ItemData.Name}: {oldInventoryItem.Bounds.Position} -> {newBounds.Position}");

            MovedItem?.Invoke(new MoveItemEventArgs(oldInventoryItem, newInventoryItem));
        }


        public InventoryItem ReceiveExistingInventoryItem(InventoryItem existingItem, InventoryBounds bounds, ItemRotation rotation)
        {
            // Get the index of the position.
            int newIndex = PositionToIndex(bounds.Position);
            
            // Create a copy and assign it to the contents.
            InventoryItem newInventoryItem = new(existingItem.Metadata, bounds, rotation, this);
            _contents[newIndex] = newInventoryItem;
            return newInventoryItem;
        }
        
        
        private int PositionToIndex(Vector2Int pos) => pos.y * _inventoryBounds.Width + pos.x;
        
        
        //private Vector2Int IndexToPosition(int index) => new(index % _inventoryBounds.Width, index / _inventoryBounds.Width);


        //private InventoryItem GetInventoryItemAt(Vector2Int pos) => _contents[PositionToIndex(pos)];

        
        public override string ToString()
        {
            return $"Inventory '{Name}' ({Bounds.Width}x{Bounds.Height})";
        }
    }
}