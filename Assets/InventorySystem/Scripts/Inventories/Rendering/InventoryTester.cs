using System;
using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InventorySystem.Inventories.Rendering
{
    public class InventoryTester : MonoBehaviour
    {
        [SerializeField] private List<ItemData> _testItems;
        [SerializeField] private Vector2Int _inv1Size = new(6, 8);
        [SerializeField] private Vector2Int _inv2Size = new(10, 7);

        private Inventory _currentlyOpenedInventory;
        
        private Inventory _inventory1;
        private Inventory _inventory2;

        
        private void Start()
        {
            _inventory1 = new Inventory(_inv1Size.x, _inv1Size.y);
            _inventory2 = new Inventory(_inv2Size.x, _inv2Size.y);
            
            _currentlyOpenedInventory = _inventory1;
            
            InventoryRenderer.Singleton.RenderInventory(_currentlyOpenedInventory);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _currentlyOpenedInventory = _inventory1;
                
                InventoryRenderer.Singleton.RenderInventory(_currentlyOpenedInventory);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _currentlyOpenedInventory = _inventory2;
                
                InventoryRenderer.Singleton.RenderInventory(_currentlyOpenedInventory);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                _currentlyOpenedInventory.TryAddItem(_testItems[Random.Range(0, _testItems.Count)]);
            }
        }
    }
}