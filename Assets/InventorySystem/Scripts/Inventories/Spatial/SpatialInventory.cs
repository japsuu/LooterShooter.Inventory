using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Inventories.Spatial
{
    public class SpatialInventory : Inventory
    {
        // Events.
        public event Action<SpatialInventory, InventoryItem> AddedItem;
        public event Action<SpatialInventory, SpatialInventory, InventoryItem, Vector2Int, Vector2Int> MovedItem;
        public event Action<SpatialInventory, InventoryItem> RemovedItem;

        // Constants.
        private const bool DEBUG_MODE = false;
        
        // Private fields.
        private readonly InventoryBounds _inventoryBounds;
        private readonly SpatialInventoryItem[] _contents;
        
        // Public fields.
        public InventoryBounds Bounds => _inventoryBounds;


        public SpatialInventory(int width, int height)
        {
            _contents = new SpatialInventoryItem[width * height];
            _inventoryBounds = new InventoryBounds(Vector2Int.zero, width, height);
        }


        public override IEnumerable<InventoryItem> GetItems() => _contents.Where(item => item != null);


        public override int ContainsItem(ItemData itemData) => _contents.Count(inventoryItem => inventoryItem.Item == itemData);

        
        public override bool TryAddItems(ItemData itemData, int count)
        {
            SpatialInventoryItem newItem = CreateNewInventoryItem(itemData);
            
            if (newItem == null)
            {
                Debug("Not enough space in the inventory!");
                return false;
            }
            
            AddInventoryItem(newItem);

            return true;
        }

        
        public bool TryAddItem(ItemData itemData, Vector2Int position)
        {
            //BUG: Implement
            SpatialInventoryItem newItem = CreateNewInventoryItem(itemData);
            
            if (newItem == null)
            {
                Debug("Not enough space in the inventory!");
                return false;
            }
            
            AddInventoryItem(newItem);

            return true;
        }

        
        /// <returns>How many of <see cref="item"/> were removed.</returns>
        public override int TryRemoveItems(ItemData item, int count)
        {
            int removedCount = 0;
            foreach (SpatialInventoryItem inventoryItem in _contents)
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

        
        public SpatialInventoryItem TryRemoveItem(Vector2Int position)
        {
            //BUG: Implement
            int removedCount = 0;
            foreach (SpatialInventoryItem inventoryItem in _contents)
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


        public bool TryMoveItem(Vector2Int oldPosition, Vector2Int newPosition, ItemRotation newRotation)
        {
            if (targetInventory == null)
            {
                Debug("TargetInventory was null!");
                return false;
            }

            if (targetInventory is SpatialInventory spatialInventory)
                return TryMoveToSpatialInventory(oldPosition, newPosition, newRotation, spatialInventory);

            //TODO: Handle non-spatial inventory
        }


        private bool TryMoveToSpatialInventory(Vector2Int oldPosition, Vector2Int newPosition, ItemRotation newRotation, SpatialInventory targetInventory)
        {
            if (!_inventoryBounds.Contains(oldPosition) || !targetInventory._inventoryBounds.Contains(newPosition))
            {
                Debug("Invalid from/to position!");
                return false;
            }
            
            // Ensure moved item exists.
            int fromIndex = PositionToIndex(oldPosition);
            SpatialInventoryItem movedItem = _contents[fromIndex];
            if (movedItem == null)
            {
                Debug("Moved item does not exist!");
                return false;
            }
            
            if (oldPosition == newPosition && movedItem.Rotation == newRotation && targetInventory == this)
                return false;
            
            int toIndex = targetInventory.PositionToIndex(newPosition);

            if (toIndex < 0 || toIndex >= targetInventory._contents.Length)
                return false;
            
            SpatialInventoryItem toItem = targetInventory._contents[toIndex];
            if (toItem != null && toItem != movedItem)
            {
                //TODO: Implement item swapping (swap the two items with each other if possible)
                Debug("Item swapping not yet implemented!");
                return false;
            }

            bool flipWidthAndHeight = newRotation.ShouldFlipWidthAndHeight();
            InventoryBounds newBounds = flipWidthAndHeight ?
                new InventoryBounds(newPosition, movedItem.Item.InventorySizeY, movedItem.Item.InventorySizeX) :
                new InventoryBounds(newPosition, movedItem.Item.InventorySizeX, movedItem.Item.InventorySizeY);

            if (!targetInventory.IsBoundsValid(newBounds, movedItem))
                return false;

            Vector2Int oldPos = movedItem.Bounds.Position;
            Vector2Int newPos = newBounds.Position;
            int oldIndex = PositionToIndex(oldPos);
            int newIndex = targetInventory.PositionToIndex(newPos);
            
            _contents[oldIndex] = null;
            targetInventory._contents[newIndex] = movedItem;
            
            movedItem.UpdateBounds(newBounds, newRotation);
            
            MovedItem?.Invoke(this, targetInventory, movedItem, oldPos, newPos);

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
            foreach (SpatialInventoryItem item in _contents)
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
        public bool IsBoundsValid(InventoryBounds itemBounds, InventoryItem itemToIgnore)
        {
            // Check if the item fits within the inventory bounds.
            if (!_inventoryBounds.Contains(itemBounds))
            {
                Debug("Bounds outside inventory!");
                return false;
            }

            // Check if there are any overlapping items.
            foreach (SpatialInventoryItem item in _contents)
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


        private SpatialInventoryItem CreateNewInventoryItem(ItemData itemData)
        {
            foreach (Vector2Int position in _inventoryBounds.AllPositionsWithin())
            {
                InventoryBounds itemBounds = new(position, itemData.InventorySizeX, itemData.InventorySizeY);
                InventoryBounds itemBoundsRotated = new(position, itemData.InventorySizeY, itemData.InventorySizeX);
                
                if (IsBoundsValid(itemBounds))
                    return new SpatialInventoryItem(itemData, itemBounds, ItemRotation.DEG_0);
                
                if (IsBoundsValid(itemBoundsRotated))
                    return new SpatialInventoryItem(itemData, itemBoundsRotated, ItemRotation.DEG_90);
            }

            return null;
        }


        private void AddInventoryItem(SpatialInventoryItem item)
        {
            _contents[PositionToIndex(item.Bounds.Position)] = item;
            
            AddedItem?.Invoke(this, item);
        }


        private void RemoveInventoryItem(SpatialInventoryItem item)
        {
            _contents[PositionToIndex(item.Bounds.Position)] = null;
            
            RemovedItem?.Invoke(this, item);
        }


        private SpatialInventoryItem GetInventoryItemAt(Vector2Int pos) => _contents[PositionToIndex(pos)];
        
        
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