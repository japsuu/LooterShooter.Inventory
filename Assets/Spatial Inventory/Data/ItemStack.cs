using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Spatial_Inventory.Data
{
    [Serializable]
    public struct ItemStack
    {
        [SerializeField] private ItemData _item;
        [SerializeField] private int _count;
        
        public int Id => _item.HashId;
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


        // public ItemStack Copy()
        // {
        //     return new ItemStack(Item, Count);
        // }
    

        // public void AddItems(int count)
        // {
        //     _count += count;
        // }
        //
        //
        // public void RemoveItems(int count)
        // {
        //     _count -= count;
        // }


        // public void SpawnAsPickup(Vector2 position)
        // {
        //     ItemStackPickup pickup = Object.Instantiate(PrefabReferences.ItemPickupPrefab, position, Quaternion.identity);
        //     pickup.Initialize(this);
        // }


        public override string ToString()
        {
            return $"{Count} x {Item.Name}";
        }
    }
}