﻿using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Clothing
{
    public class ClothingItem : ItemData
    {
        [Header("Clothing Settings")]
        [SerializeField] private ClothingType _clothingType;
        [SerializeField, Min(0)] private int _containedInventoryWidth = 8;
        [SerializeField, Min(0)] private int _containedInventoryHeight = 4;

        public ClothingType Type => _clothingType;
        public int ContainedInventoryWidth => _containedInventoryWidth;
        public int ContainedInventoryHeight => _containedInventoryHeight;
    }
}