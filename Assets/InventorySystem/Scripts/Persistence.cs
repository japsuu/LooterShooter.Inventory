using System.Collections.Generic;
using System.IO;
using InventorySystem.Inventories;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace InventorySystem
{
    [DefaultExecutionOrder(-1000)]
    public class Persistence : MonoBehaviour
    {
        public static Persistence Singleton;
        
        private const string INVENTORY_SAVE_FILE_NAME = "inventory_snapshots.txt";

        private Dictionary<string, JsonSerializableSpatialInventory> _loadedInventories;
        private Dictionary<string, SpatialInventory> _spatialInventoriesToSave;


        public void RegisterSpatialInventoryForSaving(SpatialInventory toSave, string inventoryName)
        {
            if (!_spatialInventoriesToSave.TryAdd(inventoryName, toSave))
            {
                Logger.Log(
                    LogLevel.ERROR,
                    nameof(Persistence), 
                    $"Cannot register inventory '{inventoryName}' for saving, as an existing inventory with the same name is already registered.");
            }
        }


        public bool TryLoadSpatialInventoryByName(string inventoryName, out JsonSerializableSpatialInventory result)
        {
            return _loadedInventories.TryGetValue(inventoryName, out result);
        }


        private void Awake()
        {
            if (Singleton != null)
            {
                Logger.Log(LogLevel.ERROR, $"Multiple {nameof(Persistence)} found in scene!");
                return;
            }
            
            Singleton = this;
            
            _loadedInventories = new();
            _spatialInventoriesToSave = new();
            LoadInventories();
        }


        private void OnApplicationQuit()
        {
            SaveInventories();
        }


        private void LoadInventories()
        {
            string json = ReadJsonFromFile(INVENTORY_SAVE_FILE_NAME);

            if (string.IsNullOrEmpty(json))
            {
                Logger.Log(LogLevel.INFO, "No Inventory save-file found.");
                return;
            }
            
            List<JsonSerializableSpatialInventory> deserializedData = JsonConvert.DeserializeObject<List<JsonSerializableSpatialInventory>>(json);

            _loadedInventories = new Dictionary<string, JsonSerializableSpatialInventory>();

            foreach (JsonSerializableSpatialInventory inventory in deserializedData)
            {
                _loadedInventories.Add(inventory.InventoryName, inventory);
            }
        }


        private void SaveInventories()
        {
            List<JsonSerializableSpatialInventory> toSave = new();

            foreach (KeyValuePair<string,SpatialInventory> pair in _spatialInventoriesToSave)
            {
                if(pair.Value == null)
                {
                    Logger.Log(LogLevel.WARN, $"Cannot load inventory '{pair.Key}', because it is null.");
                    return;
                }
                
                toSave.Add(pair.Value.Serialize());
            }

            string json = JsonConvert.SerializeObject(toSave, Formatting.Indented);
            WriteJsonToFile(INVENTORY_SAVE_FILE_NAME, json);
        }

        private static void WriteJsonToFile(string fileName, string json)
        {
            string path = GetFilePath(fileName);
            FileStream fileStream = new(path, FileMode.Create);

            using StreamWriter writer = new(fileStream);
            writer.Write(json);
        }
        

        [CanBeNull]
        private static string ReadJsonFromFile(string fileName)
        {
            string path = GetFilePath(fileName);
            
            if (!File.Exists(path))
                return null;
            
            using StreamReader reader = new(path);
            string json = reader.ReadToEnd();
            return json;

        }
        

        private static string GetFilePath(string fileName)
        {
            return Application.persistentDataPath + "/" + fileName;
        }


        public static void RegisterInventoryDestruction(SpatialInventory spatialInventory)
        {
            
        }
    }
}