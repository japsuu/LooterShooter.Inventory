using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Inventories
{
    public class InventoryX
    {
        // Events.
        public event Action<InventoryAddEventArgs> ItemMetadataAdded;
        public event Action<InventoryMetadataChangeEventArgs> ItemMetadataChanged;
        public event Action<InventoryRemoveEventArgs> ItemMetadataRemoved;

        // Constants.
        private const bool DEBUG_MODE = true;
        
        // Private fields.
        private readonly InventoryBounds _inventoryBounds;
        private readonly ItemMetadata[] _contents;
        
        // Public fields.
        public InventoryBounds Bounds => _inventoryBounds;


        public InventoryX(int width, int height)
        {
            _contents = new ItemMetadata[width * height];
            _inventoryBounds = new InventoryBounds(Vector2Int.zero, width, height);
        }


        public IEnumerable<ItemMetadata> GetItems() => _contents.Where(item => item != null);


        public int ContainsItem(ItemData itemData) => _contents.Count(metadata => metadata.ItemDataReference == itemData);

        
        public int TryAddItems(ItemData itemData, int count)
        {
            if(itemData == null)
                return 0;
            
            int addCount = 0;

            for (int i = 0; i < count; i++)
            {
                ItemMetadata metadata = CreateNewMetadataForItem(itemData);
            
                if (metadata == null)
                {
                    Debug("Not enough space in the inventory!");
                    return addCount;
                }
            
                AddItemMetadata(metadata);
                addCount++;
            }

            return addCount;
        }

        
        public List<ItemMetadata> TryRemoveItems(ItemData itemData, int count)
        {
            List <ItemMetadata> removedItems = new();
            if(itemData == null)
                return removedItems;
            
            foreach (ItemMetadata metadata in GetItems())
            {
                if (metadata.ItemDataReference != itemData)
                    continue;
                
                RemoveItemMetadata(metadata);
                removedItems.Add(metadata);
                
                if (removedItems.Count == count)
                    return removedItems;
            }

            return removedItems;
        }


        // public bool TryMoveItem(int oldIndex, int newIndex, ItemRotation newRotation) => TryMoveItem(IndexToPosition(oldIndex), this, IndexToPosition(newIndex), newRotation);
        // public bool TryMoveItem(int oldIndex, InventoryX newInventory, int newIndex, ItemRotation newRotation) => TryMoveItem(IndexToPosition(oldIndex), newInventory, IndexToPosition(newIndex), newRotation);


        public bool TryMoveItem(Vector2Int oldPosition, InventoryX newInventory, Vector2Int newPosition, ItemRotation newRotation)
        {
            // Ensure positions are valid.
            if (!Bounds.Contains(oldPosition) || !newInventory.Bounds.Contains(newPosition))
            {
                Debug("Invalid from/to position!");
                return false;
            }
            
            // Ensure moved item exists.
            ItemMetadata movedMetadata = GetMetadataAt(oldPosition);
            if (movedMetadata == null)
            {
                Debug("Moved item does not exist!");
                return false;
            }
            
            // Ensure some data has actually changed.
            if (oldPosition == newPosition && movedMetadata.Rotation == newRotation && newInventory == this)
                return false;
            
            ItemMetadata existingMetadata = newInventory.GetMetadataAt(newPosition);
            
            // Check that there isn't anything in the target location.
            if (existingMetadata != null && existingMetadata != movedMetadata)
            {
                Debug("Item swapping not yet implemented!");
                return false;
            }

            bool flipWidthAndHeight = newRotation.ShouldFlipWidthAndHeight();
            InventoryBounds newBounds = flipWidthAndHeight ?
                new InventoryBounds(newPosition, movedMetadata.ItemDataReference.InventorySizeY, movedMetadata.ItemDataReference.InventorySizeX) :
                new InventoryBounds(newPosition, movedMetadata.ItemDataReference.InventorySizeX, movedMetadata.ItemDataReference.InventorySizeY);

            if (!newInventory.IsBoundsValid(newBounds, oldPosition))
                return false;

            ItemMetadataSnapshot snapshotBefore = GetMetadataSnapshot(movedMetadata, this);
            
            SetMetadataAt(movedMetadata.Bounds.Position, null);
            newInventory.SetMetadataAt(newBounds.Position, movedMetadata);
            movedMetadata.UpdateBounds(newBounds, newRotation);
            
            ItemMetadataSnapshot snapshotAfter = GetMetadataSnapshot(movedMetadata, newInventory);

            InventoryMetadataChangeEventArgs args = new(snapshotBefore, snapshotAfter);
            
            ItemMetadataChanged?.Invoke(args);

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
            foreach (ItemMetadata item in GetItems())
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
        public bool IsBoundsValid(InventoryBounds itemBounds, Vector2Int positionToIgnore)
        {
            // Check if the item fits within the inventory bounds.
            if (!_inventoryBounds.Contains(itemBounds))
            {
                Debug("Bounds outside inventory!");
                return false;
            }

            // Check if there are any overlapping items.
            foreach (ItemMetadata item in GetItems())
            {
                if(item == null)
                    continue;
                
                if(item.Bounds.Position == positionToIgnore)
                    continue;
                
                if (itemBounds.OverlapsWith(item.Bounds))
                {
                    Debug($"Detected overlap with {item.ItemDataReference.Name}@{item.Bounds.Position}!");
                    return false;
                }
            }

            return true;
        }


        private ItemMetadata CreateNewMetadataForItem(ItemData itemData)
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


        private void AddItemMetadata(ItemMetadata data)
        {
            SetMetadataAt(data.Bounds.Position, data);

            InventoryAddEventArgs args = new(GetMetadataSnapshot(data, this));
            
            ItemMetadataAdded?.Invoke(args);
        }


        private void RemoveItemMetadata(ItemMetadata data)
        {
            SetMetadataAt(data.Bounds.Position, null);

            InventoryRemoveEventArgs args = new(GetMetadataSnapshot(data, this));
            
            ItemMetadataRemoved?.Invoke(args);
        }


        private static ItemMetadataSnapshot GetMetadataSnapshot(ItemMetadata metadata, InventoryX containingInventory)
        {
            return new ItemMetadataSnapshot(metadata.ItemDataReference, containingInventory, metadata.Bounds.Position);
        }


        private ItemMetadata GetMetadataAt(Vector2Int pos) => _contents[PositionToIndex(pos)];
        private void SetMetadataAt(Vector2Int pos, ItemMetadata metadata) => _contents[PositionToIndex(pos)] = metadata;
        
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