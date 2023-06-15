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
        private InventoryBounds _inventoryBounds;
        private readonly InventoryItem[] _contents;
        
        // Public fields.
        public IEnumerable<InventoryItem> Contents => _contents;
        public InventoryBounds Bounds => _inventoryBounds;


        public Inventory(int width, int height)
        {
            _inventoryBounds = new InventoryBounds(Vector2Int.zero, InventoryItemRotation.DEG_0, width, height);
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

        
        /// <summary>
        /// Tries to remove a specified amount of the specified item from this inventory.
        /// </summary>
        /// <returns>How many of the given item were removed.</returns>
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


        public bool TryMoveItem(Vector2Int fromPos, Vector2Int toPos, Inventory toInventory)
        {
            if (!_inventoryBounds.Contains(fromPos) || !toInventory._inventoryBounds.Contains(toPos))
            {
                Debug("Invalid item indexes!");
                return false;
            }
            
            // Ensure moved item exists.
            int fromIndex = PositionToIndex(fromPos);
            InventoryItem movedItem = _contents[fromIndex];
            if (movedItem == null)
            {
                Debug("Moved item does not exist!");
                return false;
            }
            
            int toIndex = toInventory.PositionToIndex(toPos);
            InventoryItem toItem = toInventory._contents[toIndex];
            if (toItem != null)
            {
                //TODO: Implement item swapping (swap the two items with each other if possible)
                Debug("Item swapping not yet implemented!");
                return false;
            }

            InventoryBounds newBounds = new InventoryBounds(toPos, movedItem.Bounds.CurrentRotation, movedItem.Bounds.Width, movedItem.Bounds.Height);

            if (toInventory.IsBoundsOverlappingSomething(newBounds))
            {
                Debug("New item position is overlapping something!");
                return false;
            }
            
            MoveInventoryItem(movedItem, toInventory, newBounds);

            return true;
        }


        private InventoryItem CreateNewInventoryItem(ItemData itemData)
        {
            InventoryBounds itemBounds = new(Vector2Int.zero, InventoryItemRotation.DEG_0, itemData.InventoryWidth, itemData.InventoryHeight);
            
            foreach (Vector2Int position in _inventoryBounds.AllPositionsWithin())
            {
                itemBounds.RootPosition = position;

                foreach (InventoryItemRotation rotation in InventoryBounds.PossibleRotations)
                {
                    itemBounds.CurrentRotation = rotation;
                    if (IsBoundsOverlappingSomething(itemBounds))
                        continue;
                    
                    return new InventoryItem(itemData, itemBounds);
                }
            }

            return null;
        }
        
        
        private bool IsBoundsOverlappingSomething(InventoryBounds itemBounds)
        {
            // Check if the item fits within the inventory bounds.
            if (!itemBounds.IsContainedIn(_inventoryBounds))
                return false;
            
            Debug($"{itemBounds} is in inventory");

            // Check if there are any overlapping items.
            foreach (InventoryItem item in _contents)
            {
                if(item == null)
                    continue;
                
                if (itemBounds.IntersectsWith(item.Bounds))
                    return true;
            }

            return false;
        }


        private void AddInventoryItem(InventoryItem item)
        {
            _contents[PositionToIndex(item.Bounds.RootPosition)] = item;
            
            AddedItem?.Invoke(item);
        }


        private void RemoveInventoryItem(InventoryItem item)
        {
            _contents[PositionToIndex(item.Bounds.RootPosition)] = null;
            
            RemovedItem?.Invoke(item);
        }


        private void MoveInventoryItem(InventoryItem item, Inventory toInventory, InventoryBounds newBounds)
        {
            Vector2Int oldPos = item.Bounds.RootPosition;
            Vector2Int newPos = newBounds.RootPosition;
            int oldIndex = PositionToIndex(oldPos);
            int newIndex = toInventory.PositionToIndex(newPos);
            
            _contents[oldIndex] = null;
            toInventory._contents[newIndex] = item;
            
            item.UpdateBounds(newBounds);
            
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