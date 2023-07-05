using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InventorySystem.Inventories.Rendering
{
    public class InventoryTester : MonoBehaviour
    {
        [SerializeField] private List<ItemData> _testItems;
        
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ItemData itemData = _testItems[Random.Range(0, _testItems.Count)];
                PlayerInventoryManager.Singleton.TryAddItems(new ItemMetadata(itemData), 1);
            }
        }
    }
}