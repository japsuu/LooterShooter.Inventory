using System.Collections.Generic;
using System.Linq;
using LooterShooter.Framework.Clothing;
using LooterShooter.Framework.Inventories;
using LooterShooter.Framework.Inventories.Items;
using UnityEngine;

namespace LooterShooter
{
    /// <summary>
    /// For testing inventory functionality.
    /// </summary>
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
                    PlayerClothingManager.Singleton.RequestEquipClothes(new ItemMetadata(itemData, null));
                }
                
                foreach (ItemData itemData in entry.ItemsToSpawn)
                {
                    PlayerInventoryManager.Singleton.TryAddItems(new ItemMetadata(itemData, null), 1);
                }
            }
        }
    }
}