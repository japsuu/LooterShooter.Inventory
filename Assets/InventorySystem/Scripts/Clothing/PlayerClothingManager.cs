using System;
using System.Collections.Generic;
using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Clothing
{
    /// <summary>
    /// Controls equipping and removing <see cref="ClothingItem"/>s.
    /// Talks to <see cref="PlayerInventoryManager"/> to add inventories for the equipped clothes.
    /// </summary>
    [RequireComponent(typeof(PlayerInventoryManager))]
    public class PlayerClothingManager : MonoBehaviour
    {
        public static PlayerClothingManager Singleton;
        
        [SerializeField] private List<ClothingItem> _startingClothes;

        private PlayerInventoryManager _playerInventoryManager;
        private Dictionary<ClothingType, ClothingItem> _equippedClothingItems;

        public bool HasAnyClothesEquipped => _equippedClothingItems.Count > 0;


        private void Awake()
        {
            Singleton = this;
            
            _playerInventoryManager = GetComponent<PlayerInventoryManager>();
            _equippedClothingItems = new();
        }


        private void Start()
        {
            if(HasAnyClothesEquipped || _playerInventoryManager.HasSomethingInInventory)
                return;
            
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
                    _playerInventoryManager.AddInventory(clothes.Type.ToString(), inventoryWidth, inventoryHeight);
                
                return true;
            }

            Debug.LogWarning($"Already have clothes of type {clothes.Type} equipped!");
            return false;
        }


        public bool TryRemoveClothes(ClothingType type)
        {
            if (_equippedClothingItems.Remove(type, out ClothingItem clothes))
            {
                _playerInventoryManager.RemoveInventory(clothes.Type.ToString());
                return true;
            }

            Debug.LogWarning($"Cannot remove clothes of type {clothes.Type}: there's nothing equipped!");
            return false;
        }
    }
}