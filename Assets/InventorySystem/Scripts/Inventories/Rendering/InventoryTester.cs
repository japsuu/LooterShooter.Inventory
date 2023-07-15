using System.Collections.Generic;
using System.Linq;
using InventorySystem.Clothing;
using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    public class InventoryTester : MonoBehaviour
    {
        [System.Serializable]
        private class SpawnEntry
        {
            public KeyCode Key;
            public List<ItemData> ItemsToSpawn;
            public List<ClothingItemData> ClothesToEquip;
        }

        [SerializeField] private List<SpawnEntry> _entries;


        private void Update()
        {
            foreach (SpawnEntry entry in _entries.Where(entry => Input.GetKeyDown(entry.Key)))
            {
                foreach (ClothingItemData itemData in entry.ClothesToEquip)
                {
                    PlayerClothingManager.Singleton.RequestEquipClothes(new ItemMetadata(itemData));
                }
                
                foreach (ItemData itemData in entry.ItemsToSpawn)
                {
                    PlayerInventoryManager.Singleton.TryAddItems(new ItemMetadata(itemData), 1);
                }
            }
        }
    }
}