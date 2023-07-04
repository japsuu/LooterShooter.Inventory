﻿using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Inventories
{
    public interface IInventory
    {
        public string Name { get; }
        
        
        public void RequestMoveItem(Vector2Int boundsPosition, Vector2Int newPos, ItemRotation newRotation, IInventory targetInventory);

        
        public bool IsPositionInsideInventory(Vector2Int position);

        
        public bool TryGetItemAtPosition(Vector2Int position, out InventoryItem item);

        
        public bool IsValidItemBounds(InventoryBounds itemBounds, InventoryBounds? existingBoundsToIgnore = null);


        public InventoryItem TransferExistingInventoryItem(InventoryItem existingItem, InventoryBounds bounds, ItemRotation rotation);
    }
}