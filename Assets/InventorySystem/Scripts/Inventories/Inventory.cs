using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Inventories
{
    public class Inventory
    {
        public readonly struct AddItemEventArgs
        {
            public readonly InventoryItem AddedItem;


            public AddItemEventArgs(InventoryItem addedItem)
            {
                AddedItem = addedItem;
            }
        }
        
        public readonly struct MoveItemEventArgs
        {
            public readonly InventoryItem OldItem;
            public readonly InventoryItem NewItem;


            public MoveItemEventArgs(InventoryItem oldItem, InventoryItem newItem)
            {
                OldItem = oldItem;
                NewItem = newItem;
            }
        }
        
        public readonly struct RemoveItemEventArgs
        {
            public readonly InventoryItem RemovedItem;


            public RemoveItemEventArgs(InventoryItem removedItem)
            {
                RemovedItem = removedItem;
            }
        }
        
        // Events.
        public event Action<AddItemEventArgs> AddedItem;
        public event Action<MoveItemEventArgs> MovedItem;
        public event Action<RemoveItemEventArgs> RemovedItem;
        
        // Private fields.
        private readonly InventoryBounds _inventoryBounds;
        private readonly InventoryItem[] _contents;

        // private statics.
        private static readonly Dictionary<string, Inventory> CurrentlyLoadedInventories = new();

        // Public fields.
        public InventoryBounds Bounds => _inventoryBounds;
        public readonly string Name;


        /// <summary>
        /// Either loads the save-file with the same name, or creates a new one using the defaults if no save-file is found.
        /// </summary>
        public Inventory(string inventoryName, int defaultWidthCells, int defaultHeightCells)
        {
            if (Persistence.Singleton.TryLoadSavedInventoryByName(inventoryName, out JsonSerializableInventory savedInventory))
            {
                Name = savedInventory.InventoryName;
                _inventoryBounds = new InventoryBounds(Vector2Int.zero, savedInventory.WidthCells, savedInventory.HeightCells);
                _contents = new InventoryItem[savedInventory.WidthCells * savedInventory.HeightCells];

                foreach (JsonSerializableInventoryItem serializedItem in savedInventory.Contents)
                {
                    if (TryDeserializeItem(serializedItem, out InventoryItem item))
                    {
                        AddInventoryItem(item);
                    }
                }
            }
            else
            {
                Name = inventoryName;
                _inventoryBounds = new InventoryBounds(Vector2Int.zero, defaultWidthCells, defaultHeightCells);
                _contents = new InventoryItem[defaultWidthCells * defaultHeightCells];
            }

            //NOTE: Server code:
            Persistence.Singleton.RegisterInventoryForSaving(this, Name);

            CurrentlyLoadedInventories.Add(Name, this);
        }


        private bool TryDeserializeItem(JsonSerializableInventoryItem jsonSerializableItem, out InventoryItem inventoryItem)
        {
            bool validItemData = TryGetItemData(jsonSerializableItem.ItemDataId, out ItemData data);

            Vector2Int position = new(jsonSerializableItem.PositionX, jsonSerializableItem.PositionY);
            int widthCells = jsonSerializableItem.Rotation.ShouldFlipWidthAndHeight() ? data.InventorySizeY : data.InventorySizeX;
            int heightCells = jsonSerializableItem.Rotation.ShouldFlipWidthAndHeight() ? data.InventorySizeX : data.InventorySizeY;
            
            InventoryBounds bounds = new(position, widthCells, heightCells);

            if (validItemData)
            {
                inventoryItem = new InventoryItem(this, data, bounds, jsonSerializableItem.Rotation);
            }
            else
            {
                inventoryItem = null;
                Logger.Log(LogLevel.WARN, $"Could not deserialize an item with ID '{jsonSerializableItem.ItemDataId}' in Inventory '{Name}'");
            }

            return validItemData;
        }


        private static bool TryGetItemData(int itemDataId, out ItemData data)
        {
            bool success = ItemDatabase.Singleton.TryGetItemById(itemDataId, out data);

            if (!success)
            {
                Logger.Log(LogLevel.FATAL, $"Invalid {nameof(JsonSerializableInventoryItem)}; cannot get reference to {nameof(ItemData)} with ID {itemDataId}");
            }
            
            return success;
        }


        ~Inventory()
        {
            CurrentlyLoadedInventories.Remove(Name);
        }


        public static bool TryGetInventoryByName(string name, out Inventory inventory)
        {
            return CurrentlyLoadedInventories.TryGetValue(name, out inventory);
        }


        public JsonSerializableInventory Serialize()
        {
            return new JsonSerializableInventory(Name, Bounds.Width, Bounds.Height, SerializeAllItems().ToArray());
        }


        private IEnumerable<JsonSerializableInventoryItem> SerializeAllItems()
        {
            foreach (InventoryItem item in _contents)
            {
                if (item != null)
                    yield return item.Serialize();
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
                if (inventoryItem.ItemDataReference == itemData)
                    results.Add(inventoryItem);
            }

            return results;
        }


        /// <returns>Snapshots of the <see cref="itemData"/>s that were added.</returns>
        public List<InventoryItem> TryAddItems(ItemData itemData, int count)
        {
            List<InventoryItem> results = new();

            for (int i = 0; i < count; i++)
            {
                InventoryItem newItem = CreateNewInventoryItem(itemData);
            
                if (newItem == null)
                {
                    Logger.Log(LogLevel.DEBUG, $"{nameof(Inventory)}: {Name}", "Not enough space in the inventory!");
                    return results;
                }
            
                AddInventoryItem(newItem);
                results.Add(newItem);
            }

            return results;
        }

        
        /// <returns>Snapshots of the <see cref="itemData"/>s that were removed.</returns>
        public List<InventoryItem> TryRemoveItems(ItemData itemData, int count)
        {
            List<InventoryItem> results = new();
            
            foreach (InventoryItem inventoryItem in _contents)
            {
                if(itemData == null)
                    continue;

                if (inventoryItem.ItemDataReference != itemData)
                    continue;
                
                if (results.Count == count)
                    return results;
                
                RemoveInventoryItem(inventoryItem);
                results.Add(inventoryItem);
            }

            return results;
        }


        public void RequestMoveItem(Vector2Int oldPosition, Vector2Int newPosition, ItemRotation newRotation,
            Inventory targetInventory)
        {
            bool success = TryMoveItem(oldPosition, newPosition, newRotation, targetInventory);

            if (success)
            {
                //TODO: Notify client.
            }
            else
            {
                //TODO: Notify client.
            }
        }


        private bool TryMoveItem(Vector2Int oldPosition, Vector2Int newPosition, ItemRotation newRotation, Inventory targetInventory)
        {
            if (targetInventory == null)
            {
                Logger.Log(LogLevel.WARN, $"{nameof(Inventory)}: {Name}", "TargetInventory was null!");
                return false;
            }
            
            if (!_inventoryBounds.Contains(oldPosition) || !targetInventory._inventoryBounds.Contains(newPosition))
            {
                Logger.Log(LogLevel.DEBUG, $"{nameof(Inventory)}: {Name}", "Invalid from/to position!");
                return false;
            }
            
            // Ensure moved item exists.
            int fromIndex = PositionToIndex(oldPosition);
            InventoryItem movedItem = _contents[fromIndex];
            if (movedItem == null)
            {
                Logger.Log(LogLevel.WARN, $"{nameof(Inventory)}: {Name}", "Moved item does not exist!");
                return false;
            }
            
            if (oldPosition == newPosition && movedItem.RotationInInventory == newRotation && targetInventory == this)
                return false;
            
            int toIndex = targetInventory.PositionToIndex(newPosition);

            if (toIndex < 0 || toIndex >= targetInventory._contents.Length)
            {
                return false;
            }
            
            InventoryItem blockingItem = targetInventory._contents[toIndex];
            if (blockingItem != null && blockingItem != movedItem)
            {
                //TODO: Implement item swapping (swap the two items with each other if possible)
                Logger.Log(LogLevel.WARN, $"{nameof(Inventory)}: {Name}", "Item swapping not yet implemented!");
                return false;
            }

            bool flipWidthAndHeight = newRotation.ShouldFlipWidthAndHeight();
            InventoryBounds newBounds = flipWidthAndHeight ?
                new InventoryBounds(newPosition, movedItem.ItemDataReference.InventorySizeY, movedItem.ItemDataReference.InventorySizeX) :
                new InventoryBounds(newPosition, movedItem.ItemDataReference.InventorySizeX, movedItem.ItemDataReference.InventorySizeY);

            if (!targetInventory.IsBoundsValid(newBounds, movedItem.Bounds))
                return false;

            MoveInventoryItem(movedItem, targetInventory, newBounds, newRotation);

            Logger.Log(LogLevel.DEBUG, $"{nameof(Inventory)}: {Name}", $"Moved '{movedItem.ItemDataReference.Name}' to {targetInventory.Name}!");
            return true;
        }
        
        
        /// <returns>If given item is inside the inventory and does not overlap with any other item.</returns>
        public bool IsBoundsValid(InventoryBounds itemBounds, InventoryBounds? existingBoundsToIgnore = null)
        {
            // Check if the item fits within the inventory bounds.
            if (!_inventoryBounds.Contains(itemBounds))
                return false;

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


        private InventoryItem CreateNewInventoryItem(ItemData itemData)
        {
            foreach (Vector2Int position in _inventoryBounds.AllPositionsWithin())
            {
                InventoryBounds itemBounds = new(position, itemData.InventorySizeX, itemData.InventorySizeY);
                InventoryBounds itemBoundsRotated = new(position, itemData.InventorySizeY, itemData.InventorySizeX);
                
                if (IsBoundsValid(itemBounds))
                    return new InventoryItem(this, itemData, itemBounds, ItemRotation.DEG_0);
                
                if (IsBoundsValid(itemBoundsRotated))
                    return new InventoryItem(this, itemData, itemBoundsRotated, ItemRotation.DEG_90);
            }

            return null;
        }


        private void AddInventoryItem(InventoryItem inventoryItem)
        {
            Logger.Log(LogLevel.DEBUG, $"Inventory {Name} added {inventoryItem.ItemDataReference.Name}@{inventoryItem.Bounds.Position}");
            _contents[PositionToIndex(inventoryItem.Bounds.Position)] = inventoryItem;
            
            AddedItem?.Invoke(new AddItemEventArgs(inventoryItem));
        }


        private void RemoveInventoryItem(InventoryItem inventoryItem)
        {
            _contents[PositionToIndex(inventoryItem.Bounds.Position)] = null;
            
            RemovedItem?.Invoke(new RemoveItemEventArgs(inventoryItem));
        }


        private void MoveInventoryItem(InventoryItem oldInventoryItem, Inventory newInventory, InventoryBounds newBounds, ItemRotation newRotation)
        {
            Vector2Int oldPos = oldInventoryItem.Bounds.Position;
            Vector2Int newPos = newBounds.Position;
            int oldIndex = PositionToIndex(oldPos);
            int newIndex = newInventory.PositionToIndex(newPos);
            
            InventoryItem newInventoryItem = new(newInventory, oldInventoryItem.ItemDataReference, newBounds, newRotation);
            
            _contents[oldIndex] = null;
            newInventory._contents[newIndex] = newInventoryItem;

            MovedItem?.Invoke(new MoveItemEventArgs(oldInventoryItem, newInventoryItem));
        }


        private InventoryItem GetInventoryItemAt(Vector2Int pos) => _contents[PositionToIndex(pos)];
        
        private int PositionToIndex(Vector2Int pos) => pos.y * _inventoryBounds.Width + pos.x;
        
        private Vector2Int IndexToPosition(int index) => new(index % _inventoryBounds.Width, index / _inventoryBounds.Width);

        public override string ToString()
        {
            return $"Inventory '{Name}' ({Bounds.Width}x{Bounds.Height})";
        }
    }
}