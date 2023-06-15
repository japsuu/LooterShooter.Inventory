using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem.Inventories
{
    public class Inventory
    {
        // Events.
        public event Action<InventoryItem> AddedItem;
        public event Action<InventoryItem, Inventory, Vector2Int, Vector2Int> MovedItem;
        public event Action<InventoryItem> RemovedItem;

        // Constants.
        private const bool DEBUG_MODE = true;
        
        // Private fields.
        private readonly InventoryBounds _inventoryBounds;
        private readonly InventoryItem[] _contents;
        
        // Public fields.
        public IEnumerable<InventoryItem> Contents => _contents;
        public InventoryBounds Bounds => _inventoryBounds;


        public Inventory(int width, int height)
        {
            _inventoryBounds = new InventoryBounds(Vector2Int.zero, width, height);
            _contents = new InventoryItem[width * height];
        }


        public int ContainsItem(ItemData itemData) => _contents.Count(inventoryItem => inventoryItem.Item == itemData);

        
        public bool TryAddItem(ItemData itemData)
        {
            InventoryItem newItem = CreateNewInventoryItem(itemData);
            
            if (newItem == null)
            {
                Debug("Not enough space in the inventory!");
                return false;
            }
            
            AddInventoryItem(newItem);

            return true;
        }

        
        /// <returns>How many of <see cref="item"/> were removed.</returns>
        public int TryRemoveItems(ItemData item, int count)
        {
            int removedCount = 0;
            foreach (InventoryItem inventoryItem in _contents)
            {
                if(item == null)
                    continue;

                if (inventoryItem.Item != item)
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
            if (oldPosition == newPosition)
                return false;
            
            if (!_inventoryBounds.Contains(oldPosition) || !targetInventory._inventoryBounds.Contains(newPosition))
            {
                Debug("Invalid item indexes!");
                return false;
            }
            
            // Ensure moved item exists.
            int fromIndex = PositionToIndex(oldPosition);
            InventoryItem movedItem = _contents[fromIndex];
            if (movedItem == null)
            {
                Debug("Moved item does not exist!");
                return false;
            }
            
            int toIndex = targetInventory.PositionToIndex(newPosition);

            if (toIndex < 0 || toIndex >= targetInventory._contents.Length)
                return false;
            
            InventoryItem toItem = targetInventory._contents[toIndex];
            if (toItem != null)
            {
                //TODO: Implement item swapping (swap the two items with each other if possible)
                Debug("Item swapping not yet implemented!");
                return false;
            }

            InventoryBounds newBounds = newRotation == ItemRotation.DEG_0 ?
                new InventoryBounds(newPosition, movedItem.Item.InventoryWidth, movedItem.Item.InventoryHeight) :
                new InventoryBounds(newPosition, movedItem.Item.InventoryHeight, movedItem.Item.InventoryWidth);

            if (!targetInventory.IsBoundsValid(newBounds, movedItem))
                return false;

            MoveInventoryItem(movedItem, targetInventory, newBounds, newRotation);

            return true;
        }


        private InventoryItem CreateNewInventoryItem(ItemData itemData)
        {
            foreach (Vector2Int position in _inventoryBounds.AllPositionsWithin())
            {
                InventoryBounds itemBounds = new(position, itemData.InventoryWidth, itemData.InventoryHeight);
                InventoryBounds itemBoundsRotated = new(position, itemData.InventoryHeight, itemData.InventoryWidth);
                
                if (IsBoundsValid(itemBounds))
                    return new InventoryItem(itemData, itemBounds, ItemRotation.DEG_0);
                
                if (IsBoundsValid(itemBoundsRotated))
                    return new InventoryItem(itemData, itemBoundsRotated, ItemRotation.DEG_90);
            }

            return null;
        }
        
        
        /// <returns>If given item is inside the inventory and does not overlap with any other item.</returns>
        private bool IsBoundsValid(InventoryBounds itemBounds)
        {
            // Check if the item fits within the inventory bounds.
            if (!_inventoryBounds.Contains(itemBounds))
                return false;

            // Check if there are any overlapping items.
            foreach (InventoryItem item in _contents)
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
        private bool IsBoundsValid(InventoryBounds itemBounds, InventoryItem itemToIgnore)
        {
            // Check if the item fits within the inventory bounds.
            if (!_inventoryBounds.Contains(itemBounds))
            {
                Debug("Bounds outside inventory!");
                return false;
            }

            // Check if there are any overlapping items.
            foreach (InventoryItem item in _contents)
            {
                if(item == null)
                    continue;
                
                if(item == itemToIgnore)
                    continue;
                
                if (itemBounds.OverlapsWith(item.Bounds))
                {
                    Debug($"Detected overlap with {item.Item.Name}@{item.Position}!");
                    return false;
                }
            }

            return true;
        }


        private void AddInventoryItem(InventoryItem item)
        {
            _contents[PositionToIndex(item.Bounds.Position)] = item;
            
            AddedItem?.Invoke(item);
        }


        private void RemoveInventoryItem(InventoryItem item)
        {
            _contents[PositionToIndex(item.Bounds.Position)] = null;
            
            RemovedItem?.Invoke(item);
        }


        private void MoveInventoryItem(InventoryItem item, Inventory toInventory, InventoryBounds newBounds, ItemRotation newRotation)
        {
            Vector2Int oldPos = item.Bounds.Position;
            Vector2Int newPos = newBounds.Position;
            int oldIndex = PositionToIndex(oldPos);
            int newIndex = toInventory.PositionToIndex(newPos);
            
            _contents[oldIndex] = null;
            toInventory._contents[newIndex] = item;
            
            item.UpdateBounds(newBounds, newRotation);
            
            MovedItem?.Invoke(item, toInventory, oldPos, newPos);
        }


        private InventoryItem GetInventoryItemAt(Vector2Int pos) => _contents[PositionToIndex(pos)];
        
        private int PositionToIndex(Vector2Int pos) => pos.y * _inventoryBounds.Width + pos.x;
        
        private Vector2Int IndexToPosition(int index) => new(index % _inventoryBounds.Width, index / _inventoryBounds.Width);


        private static void Debug(string text)
        {
            if(DEBUG_MODE)
                UnityEngine.Debug.Log(text);
        }
    }
}