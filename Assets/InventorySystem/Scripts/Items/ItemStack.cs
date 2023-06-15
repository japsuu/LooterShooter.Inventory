using System;
using UnityEngine;

namespace InventorySystem.Items
{
    /// <summary>
    /// Represents a certain amount of <see cref="ItemData"/>.
    /// </summary>
    [Serializable]
    public struct ItemStack
    {
        [SerializeField] private ItemData _item;
        [SerializeField, Min(1)] private int _count;
        
        public ItemData Item => _item;
        public int Count => _count;
        public bool IsEmpty => Count < 1;
        

        public ItemStack(ItemData item, int count = 1)
        {
            _item = item;
            _count = count;
        }


        public static ItemStack operator +(ItemStack a, ItemStack b)
        {
            return new ItemStack(a.Item, a.Count + b.Count);
        }


        public static ItemStack operator -(ItemStack a, ItemStack b)
        {
            return new ItemStack(a.Item, a.Count - b.Count);
        }


        public override string ToString()
        {
            return $"{Count} x {Item.Name}";
        }
    }
}