using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Spatial.Items;
using UnityEngine;

namespace InventorySystem.Inventories.Spatial
{
    public class SpatialInventory : Inventory
    {
        // Events.
        public event Action<SpatialInventory, InventoryData> AddedItem;
        public event Action<SpatialInventory, SpatialInventory, InventoryData, Vector2Int, Vector2Int> MovedItem;
        public event Action<SpatialInventory, InventoryData> RemovedItem;

        // Constants.
        private const bool DEBUG_MODE = false;
        
        // Private fields.
        private readonly InventoryBounds _inventoryBounds;
        private readonly SpatialInventoryData[] _contents;
        
        // Public fields.
        public InventoryBounds Bounds => _inventoryBounds;


        public SpatialInventory(int width, int height)
        {
            _contents = new SpatialInventoryData[width * height];
            _inventoryBounds = new InventoryBounds(Vector2Int.zero, width, height);
        }


        public override IEnumerable<InventoryData> GetItems() => _contents.Where(item => item != null);


        public override int ContainsItem(ItemData itemData) => _contents.Count(inventoryItem => inventoryItem.Item == itemData);

        
        public override bool TryAddItem(ItemData itemData)
        {
            SpatialInventoryData newData = CreateNewInventoryItem(itemData);
            
            if (newData == null)
            {
                Debug("Not enough space in the inventory!");
                return false;
            }
            
            AddInventoryItem(newData);

            return true;
        }

        
        public bool TryAddItem(ItemData itemData, Vector2Int position)
        {
            //BUG: Implement
            SpatialInventoryData newData = CreateNewInventoryItem(itemData);
            
            if (newData == null)
            {
                Debug("Not enough space in the inventory!");
                return false;
            }
            
            AddInventoryItem(newData);

            return true;
        }

        
        /// <returns>How many of <see cref="item"/> were removed.</returns>
        public override int TryRemoveItems(ItemData item, int count)
        {
            if(item == null)
                return 0;
            
            int removedCount = 0;
            foreach (SpatialInventoryData inventoryItem in _contents)
            {
                if (inventoryItem.Item != item)
                    continue;
                
                RemoveInventoryItem(inventoryItem);
                removedCount++;
                
                if (removedCount == count)
                    return removedCount;
            }

            return removedCount;
        }

        
        public SpatialInventoryData TryRemoveItem(Vector2Int position)
        {
            //BUG: Implement
            int removedCount = 0;
            foreach (SpatialInventoryData inventoryItem in _contents)
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
            SpatialInventoryData movedData = _contents[fromIndex];
            if (movedData == null)
            {
                Debug("Moved item does not exist!");
                return false;
            }
            
            if (oldPosition == newPosition && movedData.Rotation == newRotation && targetInventory == this)
                return false;
            
            int toIndex = targetInventory.PositionToIndex(newPosition);

            if (toIndex < 0 || toIndex >= targetInventory._contents.Length)
                return false;
            
            SpatialInventoryData toData = targetInventory._contents[toIndex];
            if (toData != null && toData != movedData)
            {
                //TODO: Implement item swapping (swap the two items with each other if possible)
                Debug("Item swapping not yet implemented!");
                return false;
            }

            bool flipWidthAndHeight = newRotation.ShouldFlipWidthAndHeight();
            InventoryBounds newBounds = flipWidthAndHeight ?
                new InventoryBounds(newPosition, movedData.Item.InventorySizeY, movedData.Item.InventorySizeX) :
                new InventoryBounds(newPosition, movedData.Item.InventorySizeX, movedData.Item.InventorySizeY);

            if (!targetInventory.IsBoundsValid(newBounds, movedData))
                return false;

            Vector2Int oldPos = movedData.Bounds.Position;
            Vector2Int newPos = newBounds.Position;
            int oldIndex = PositionToIndex(oldPos);
            int newIndex = targetInventory.PositionToIndex(newPos);
            
            _contents[oldIndex] = null;
            targetInventory._contents[newIndex] = movedData;
            
            movedData.UpdateBounds(newBounds, newRotation);
            
            MovedItem?.Invoke(this, targetInventory, movedData, oldPos, newPos);

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
            foreach (SpatialInventoryData item in _contents)
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
        public bool IsBoundsValid(InventoryBounds itemBounds, InventoryData dataToIgnore)
        {
            // Check if the item fits within the inventory bounds.
            if (!_inventoryBounds.Contains(itemBounds))
            {
                Debug("Bounds outside inventory!");
                return false;
            }

            // Check if there are any overlapping items.
            foreach (SpatialInventoryData item in _contents)
            {
                if(item == null)
                    continue;
                
                if(item == dataToIgnore)
                    continue;
                
                if (itemBounds.OverlapsWith(item.Bounds))
                {
                    Debug($"Detected overlap with {item.Item.Name}@{item.Position}!");
                    return false;
                }
            }

            return true;
        }


        private SpatialInventoryData CreateNewInventoryItem(ItemData itemData)
        {
            foreach (Vector2Int position in _inventoryBounds.AllPositionsWithin())
            {
                InventoryBounds itemBounds = new(position, itemData.InventorySizeX, itemData.InventorySizeY);
                InventoryBounds itemBoundsRotated = new(position, itemData.InventorySizeY, itemData.InventorySizeX);
                
                if (IsBoundsValid(itemBounds))
                    return new SpatialInventoryData(itemData, itemBounds, ItemRotation.DEG_0);
                
                if (IsBoundsValid(itemBoundsRotated))
                    return new SpatialInventoryData(itemData, itemBoundsRotated, ItemRotation.DEG_90);
            }

            return null;
        }


        private void AddInventoryItem(SpatialInventoryData data)
        {
            _contents[PositionToIndex(data.Bounds.Position)] = data;
            
            AddedItem?.Invoke(this, data);
        }


        private void RemoveInventoryItem(SpatialInventoryData data)
        {
            _contents[PositionToIndex(data.Bounds.Position)] = null;
            
            RemovedItem?.Invoke(this, data);
        }


        private SpatialInventoryData GetInventoryItemAt(Vector2Int pos) => _contents[PositionToIndex(pos)];
        
        
        //TODO: Use InventoryItem.InventoryIndex instead of this.
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