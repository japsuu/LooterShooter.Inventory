using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Inventories.Items
{
    [DefaultExecutionOrder(-2000)]
    public class ItemDatabase : MonoBehaviour
    {
        public static ItemDatabase Singleton;

        [SerializeField] private List<ItemData> _testingDatabase;

        private Dictionary<int, ItemData> _database;


        private void Awake()
        {
            Singleton = this;
            
            _database = new();

            foreach (ItemData item in _testingDatabase)
            {
                RegisterItem(item);
            }
        }


        public void RegisterItem(ItemData item)
        {
            _database.Add(item.HashId, item);
        }


        public bool TryGetItemById(int itemId, out ItemData item)
        {
            bool success = _database.TryGetValue(itemId, out item);

            if (!success)
            {
                Logger.Log(LogLevel.FATAL, $"Cannot get reference to {nameof(ItemData)} with ID {itemId}!");
            }
            
            return success;
        }
    }
}