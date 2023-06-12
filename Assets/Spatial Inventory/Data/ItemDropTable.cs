using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spatial_Inventory.Data
{
    [CreateAssetMenu(fileName = "DropTable_", menuName = "Prototyping/Item Drop Table Asset", order = 0)]
    public class ItemDropTable : ScriptableObject
    {
        [Serializable]
        private class DropTableEntry
        {
            public ItemData Item;
            
            [Range(0f, 100f)]
            public float SelectionChance = 1;
            
            [Min(1)]
            public int CountMin = 1;
            
            [Min(1)]
            public int CountMax = 1;
        }

        [SerializeField] private List<DropTableEntry> _entries;


        /// <param name="rolls">Count of "chances" we give for items to spawn.</param>
        /// <returns>Random items from the drop table.</returns>
        public IEnumerable<ItemStack> GetRandomItems(int rolls)
        {
            List<ItemStack> drops = new();

            // Execute all the rolls.
            for (int i = 0; i < rolls; i++)
            {
                // Select a random entry.
                int index = Random.Range(0, _entries.Count);
                DropTableEntry entry = _entries[index];
                
                // Skip if stars don't align.
                if(Random.Range(0f, 100f) > entry.SelectionChance)
                    continue;

                // Select the stack size.
                int itemCount = Random.Range(entry.CountMin, entry.CountMax);
                
                if(itemCount == 0)
                    continue;
                
                drops.Add(new ItemStack(entry.Item, itemCount));
            }

            return drops;
        }
        
        
        /*// Below is the old "complex" drop table code.
        // Removed because "KISS".
        // Might come back to this later.
        [Serializable]
        private class DropTableEntry
        {
            public ItemData Item;
            [Range(1f, 1000f)]
            public float SelectionChance = 1;
            public int CountMin = 1;
            public int CountMax = 1;
        }

        [SerializeField] private List<DropTableEntry> _entries;


        public IEnumerable<ItemStack> GetDrops(int minCount, int maxCount)
        {
            // NOTE: Due to how scriptableObjects work, we "re-generate" a new drop table each time drops are requested.
            // NOTE: This should not be used in production.
            WeightedRandomBag<DropTableEntry> table = new();

            foreach (DropTableEntry entry in _entries)
            {
                table.AddEntry(entry, entry.SelectionChance);
            }
            
            List<ItemStack> drops = new();

            int stackCount = Random.Range(minCount, maxCount);

            for (int i = 0; i < stackCount; i++)
            {
                DropTableEntry entry = table.GetRandom();

                int itemCount = Random.Range(entry.CountMin, entry.CountMax);
                
                if(itemCount == 0)
                    continue;
                
                drops.Add(new ItemStack(entry.Item, itemCount));
            }

            return drops;
        }*/
    }
}