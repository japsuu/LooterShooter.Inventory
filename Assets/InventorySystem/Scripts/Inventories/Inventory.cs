using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Inventories
{
    public class Inventory
    {
        // Events.
        public event Action<Inventory, ItemMetadata> AddedItem;
        public event Action<Inventory, Inventory, ItemMetadata, Vector2Int, Vector2Int> MovedItem;
        public event Action<Inventory, ItemMetadata> RemovedItem;

        // Constants.
        private const bool DEBUG_MODE = false;
        
        // Private fields.
        private readonly InventoryBounds _inventoryBounds;
        private readonly ItemMetadata[] _contents;
        
        // Public fields.
        public InventoryBounds Bounds => _inventoryBounds;


        public Inventory(int width, int height)
        {
            _inventoryBounds = new InventoryBounds(Vector2Int.zero, width, height);
            _contents = new ItemMetadata[width * height];
        }


        public IEnumerable<ItemMetadata> GetItems() => _contents.Where(item => item != null);


        public int ContainsItem(ItemData itemData) => _contents.Count(inventoryItem => inventoryItem.ItemDataReference == itemData);

        
        public int TryAddItems(ItemData itemData, int count)
        {
            int addedCount = 0;

            for (int i = 0; i < count; i++)
            {
                ItemMetadata newItemMetadata = CreateNewInventoryItem(itemData);
            
                if (newItemMetadata == null)
                {
                    Debug("Not enough space in the inventory!");
                    return addedCount;
                }
            
                AddInventoryItem(newItemMetadata);
                addedCount++;
            }

            return addedCount;
        }

        
        /// <returns>How many of <see cref="item"/> were removed.</returns>
        public int TryRemoveItems(ItemData item, int count)
        {
            int removedCount = 0;
            foreach (ItemMetadata inventoryItem in _contents)
            {
                if(item == null)
                    continue;

                if (inventoryItem.ItemDataReference != item)
                    continue;
                
                RemoveInventoryItem(inventoryItem);
                removedCount++;
                
                if (removedCount == count)
                    return removedCount;
            }

            return removedCount;
        }


        public bool TryMoveItem(Vector2Int oldPosition, Vector2Int newPosition, ItemRotation newRotation, Inventory targetInventory)
        {
            if (targetInventory == null)
            {
                Debug("TargetInventory was null!");
                return false;
            }
            
            if (!_inventoryBounds.Contains(oldPosition) || !targetInventory._inventoryBounds.Contains(newPosition))
            {
                Debug("Invalid from/to position!");
                return false;
            }
            
            // Ensure moved item exists.
            int fromIndex = PositionToIndex(oldPosition);
            ItemMetadata movedItemMetadata = _contents[fromIndex];
            if (movedItemMetadata == null)
            {
                Debug("Moved item does not exist!");
                return false;
            }
            
            if (oldPosition == newPosition && movedItemMetadata.RotationInInventory == newRotation && targetInventory == this)
                return false;
            
            int toIndex = targetInventory.PositionToIndex(newPosition);

            if (toIndex < 0 || toIndex >= targetInventory._contents.Length)
                return false;
            
            ItemMetadata toItemMetadata = targetInventory._contents[toIndex];
            if (toItemMetadata != null && toItemMetadata != movedItemMetadata)
            {
                //TODO: Implement item swapping (swap the two items with each other if possible)
                Debug("Item swapping not yet implemented!");
                return false;
            }

            bool flipWidthAndHeight = newRotation.ShouldFlipWidthAndHeight();
            InventoryBounds newBounds = flipWidthAndHeight ?
                new InventoryBounds(newPosition, movedItemMetadata.ItemDataReference.InventorySizeY, movedItemMetadata.ItemDataReference.InventorySizeX) :
                new InventoryBounds(newPosition, movedItemMetadata.ItemDataReference.InventorySizeX, movedItemMetadata.ItemDataReference.InventorySizeY);

            if (!targetInventory.IsBoundsValid(newBounds, movedItemMetadata))
                return false;

            MoveInventoryItem(movedItemMetadata, targetInventory, newBounds, newRotation);

            Debug("Move success!");
            return true;
        }
        
        
        /// <returns>If given item is inside the inventory and does not overlap with any other item.</returns>
        public bool IsBoundsValid(InventoryBounds itemBounds)
        {
            // Check if the item fits within the inventory bounds.
            if (!_inventoryBounds.Contains(itemBounds))
                return false;

            // Check if there are any overlapping items.
            foreach (ItemMetadata item in _contents)
            {
                if(item == null)
                    continue;
                
                // if(item.Bounds == itemBounds)
                //     continue;
                
                if (itemBounds.OverlapsWith(item.Bounds))
                    return false;
            }

            return true;
        }
        
        
        /// <returns>If given item is inside the inventory and does not overlap with any other item.</returns>
        public bool IsBoundsValid(InventoryBounds itemBounds, ItemMetadata itemMetadataToIgnore)
        {
            // Check if the item fits within the inventory bounds.
            if (!_inventoryBounds.Contains(itemBounds))
            {
                Debug("Bounds outside inventory!");
                return false;
            }

            // Check if there are any overlapping items.
            foreach (ItemMetadata item in _contents)
            {
                if(item == null)
                    continue;
                
                if(item == itemMetadataToIgnore)
                    continue;
                
                if (itemBounds.OverlapsWith(item.Bounds))
                {
                    Debug($"Detected overlap with {item.ItemDataReference.Name}@{item.Bounds.Position}!");
                    return false;
                }
            }

            return true;
        }


        private ItemMetadata CreateNewInventoryItem(ItemData itemData)
        {
            foreach (Vector2Int position in _inventoryBounds.AllPositionsWithin())
            {
                InventoryBounds itemBounds = new(position, itemData.InventorySizeX, itemData.InventorySizeY);
                InventoryBounds itemBoundsRotated = new(position, itemData.InventorySizeY, itemData.InventorySizeX);
                
                if (IsBoundsValid(itemBounds))
                    return new ItemMetadata(itemData, itemBounds, ItemRotation.DEG_0);
                
                if (IsBoundsValid(itemBoundsRotated))
                    return new ItemMetadata(itemData, itemBoundsRotated, ItemRotation.DEG_90);
            }

            return null;
        }


        private void AddInventoryItem(ItemMetadata itemMetadata)
        {
            _contents[PositionToIndex(itemMetadata.Bounds.Position)] = itemMetadata;
            
            AddedItem?.Invoke(this, itemMetadata);
        }


        private void RemoveInventoryItem(ItemMetadata itemMetadata)
        {
            _contents[PositionToIndex(itemMetadata.Bounds.Position)] = null;
            
            RemovedItem?.Invoke(this, itemMetadata);
        }


        private void MoveInventoryItem(ItemMetadata itemMetadata, Inventory toInventory, InventoryBounds newBounds, ItemRotation newRotation)
        {
            Vector2Int oldPos = itemMetadata.Bounds.Position;
            Vector2Int newPos = newBounds.Position;
            int oldIndex = PositionToIndex(oldPos);
            int newIndex = toInventory.PositionToIndex(newPos);
            
            _contents[oldIndex] = null;
            toInventory._contents[newIndex] = itemMetadata;
            
            itemMetadata.UpdateBounds(newBounds, newRotation);
            
            MovedItem?.Invoke(this, toInventory, itemMetadata, oldPos, newPos);
        }


        private ItemMetadata GetInventoryItemAt(Vector2Int pos) => _contents[PositionToIndex(pos)];
        
        private int PositionToIndex(Vector2Int pos) => pos.y * _inventoryBounds.Width + pos.x;
        
        private Vector2Int IndexToPosition(int index) => new(index % _inventoryBounds.Width, index / _inventoryBounds.Width);

#pragma warning disable CS0162
        private static void Debug(string text)
        {
            if(DEBUG_MODE)
                UnityEngine.Debug.Log(text);
        }
    }
}