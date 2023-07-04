using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InventorySystem.Inventories.Rendering
{
    public class InventoryTester : MonoBehaviour
    {
        [SerializeField] private List<ItemMetadata> _testItems;
        
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayerInventoryManager.Singleton.TryAddItems(_testItems[Random.Range(0, _testItems.Count)], 1);
            }
        }
    }
}