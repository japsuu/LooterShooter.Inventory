using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Inventories.Items
{
    [DefaultExecutionOrder(-2000)]
    public class ItemDatabase : MonoBehaviour
    {
        public static ItemDatabase Singleton;

        [SerializeField] private List<ItemData> _testingDatabase;

        private Dictionary<Guid, ItemData> _database;


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
            if (!_database.TryAdd(item.Guid, item))
            {
                Logger.Log(
                    LogLevel.ERROR,
                    _database.TryGetValue(item.Guid, out ItemData registeredItem)
                        ? $"{nameof(ItemData)} '{item.ItemName}' (GUID: {item.Guid}) will not get registered as it's GUID clashes with {nameof(ItemData)} '{registeredItem.ItemName}'."
                        : $"{nameof(ItemData)} '{item.ItemName}' (GUID: {item.Guid}) could not be registered.");
            }
        }


        public bool TryGetItemById(Guid itemGuid, out ItemData item)
        {
            bool success = _database.TryGetValue(itemGuid, out item);

            if (!success)
            {
                Logger.Log(LogLevel.FATAL, $"Cannot get reference to {nameof(ItemData)} with GUID {itemGuid}!");
            }
            
            return success;
        }
    }
}