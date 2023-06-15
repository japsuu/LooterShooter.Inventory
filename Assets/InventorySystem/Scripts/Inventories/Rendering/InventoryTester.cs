using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    public class InventoryTester : MonoBehaviour
    {
        [SerializeField] private ItemData _testItem1;
        [SerializeField] private ItemData _testItem2;
        
        private readonly Inventory _inventory1 = new Inventory(6, 8);
        private readonly Inventory _inventory2 = new Inventory(8, 14);
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                InventoryRenderer.Singleton.RenderInventory(_inventory1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                InventoryRenderer.Singleton.RenderInventory(_inventory2);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                _inventory1.TryAddItem(Random.Range(0, 2) == 0 ? _testItem1 : _testItem2);
            }
        }
    }
}