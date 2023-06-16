using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Inventories
{
    /// <summary>
    /// Controls multiple child <see cref="Inventory"/>s.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        [Header("Base Inventory")]
        [SerializeField] private string _baseInventoryName = "Pockets";
        [SerializeField, Min(1)] private int _baseInventoryWidth = 8;
        [SerializeField, Min(1)] private int _baseInventoryHeight = 4;
        
        private Dictionary<string, Inventory> _inventories;


        private void Start()
        {
            _inventories.Add(_baseInventoryName, new Inventory(_baseInventoryWidth, _baseInventoryHeight));
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                //TODO: Notify UI to draw inventories.
            }
        }


        public void AddInventory(string inventoryName, int width, int height)
        {
            Inventory inventory = new(width, height);
            _inventories.Add(inventoryName, inventory);
            
        }
        
        public void RemoveInventory(string inventoryName)
        {
            if (_inventories.Remove(inventoryName, out Inventory inventory))
            {
                //TODO: Drop items to ground or something?
                Debug.LogError("NotImplemented: I have no idea what to do with the items from the inventory you just removed!");
            }
        }
    }
}