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
        //[SerializeField] private Vector2Int _inv1Size = new(6, 8);
        //[SerializeField] private Vector2Int _inv2Size = new(10, 7);

        
        private void Start()
        {
            //PlayerInventoryManager.Singleton.AddInventory("Test 1", _inv1Size.x, _inv1Size.y);
            //PlayerInventoryManager.Singleton.AddInventory("Test 2", _inv2Size.x, _inv2Size.y);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayerInventoryManager.Singleton.TryAddItem(_testItems[Random.Range(0, _testItems.Count)]);
            }
        }
    }
}