using System.Collections.Generic;
using InventorySystem.Inventories;
using UnityEngine;

namespace InventorySystem.Clothing
{
    /// <summary>
    /// Controls equipping and removing <see cref="ClothingItem"/>s.
    /// Talks to <see cref="InventoryManager"/> to add inventories for the equipped clothes.
    /// </summary>
    [RequireComponent(typeof(InventoryManager))]
    public class ClothingManager : MonoBehaviour
    {
        [SerializeField] private List<ClothingItem> _startingClothes;

        private InventoryManager _inventoryManager;
        private Dictionary<ClothingType, ClothingItem> _equippedClothingItems;


        private void Awake()
        {
            _inventoryManager = GetComponent<InventoryManager>();
            _equippedClothingItems = new();

            // Equip starting clothes.
            foreach (ClothingItem clothes in _startingClothes)
            {
                TryEquipClothes(clothes);
            }
        }


        public bool TryEquipClothes(ClothingItem clothes)
        {
            if (_equippedClothingItems.TryAdd(clothes.Type, clothes))
            {
                int inventoryWidth = clothes.ContainedInventoryWidth;
                int inventoryHeight = clothes.ContainedInventoryHeight;
                
                if(inventoryWidth > 0 && inventoryHeight > 0)
                    _inventoryManager.AddInventory(clothes.Type.ToString(), inventoryWidth, inventoryHeight);
                
                return true;
            }

            Debug.LogWarning($"Already have clothes of type {clothes.Type} equipped!");
            return false;
        }


        public bool TryRemoveClothes(ClothingType type)
        {
            if (_equippedClothingItems.Remove(type, out ClothingItem clothes))
            {
                _inventoryManager.RemoveInventory(clothes.Type.ToString());
                return true;
            }

            Debug.LogWarning($"Cannot remove clothes of type {clothes.Type}: there's nothing equipped!");
            return false;
        }
    }
}