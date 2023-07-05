using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using InventorySystem.Inventories;
using InventorySystem.Inventories.Serialization;
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

        private Dictionary<string, SpatialInventory> _loadedInventories;
        private Dictionary<string, SpatialInventory> _spatialInventoriesToSave;
        
        
        [UnityEditor.MenuItem("Looter Shooter/Open Save Folder")]
        private static void OpenSaveFolder()
        {
            Process.Start(Application.persistentDataPath);
        }


        public static void RegisterInventoryDestruction(SpatialInventory spatialInventory)
        {
            
        }


        public SpatialInventory GetSpatialInventoryByName(string inventoryName, int width, int height)
        {
            if (TryLoadSpatialInventoryByName(inventoryName, out SpatialInventory inventory))
            {
                if(inventory.Bounds.Width != width || inventory.Bounds.Height != height)
                    Logger.Log(
                        LogLevel.WARN,
                        $"Loaded {nameof(SpatialInventory)} with unexpected size " +
                        $"({inventory.Bounds.Width}x{inventory.Bounds.Height}, " +
                        $"expected {width}x{height}), has it been modified externally?");
                
            }
            else
            {
                inventory = new(inventoryName, width, height);
            }

            //NOTE: Server code:
            RegisterSpatialInventoryForSaving(inventory, inventory.Name);
            
            return inventory;
        }


        private bool TryLoadSpatialInventoryByName(string inventoryName, out SpatialInventory result)
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


        private void RegisterSpatialInventoryForSaving(SpatialInventory toSave, string inventoryName)
        {
            if (!_spatialInventoriesToSave.TryAdd(inventoryName, toSave))
            {
                Logger.Log(
                    LogLevel.ERROR,
                    nameof(Persistence), 
                    $"Cannot register inventory '{inventoryName}' for saving, as an existing inventory with the same name is already registered.");
            }
        }


        private void LoadInventories()
        {
            string json = ReadJsonFromFile(INVENTORY_SAVE_FILE_NAME);

            if (string.IsNullOrEmpty(json))
            {
                Logger.Log(LogLevel.INFO, "No Inventory save-file found.");
                return;
            }
            
            List<SpatialInventory> deserializedData = JsonConvert.DeserializeObject<List<SpatialInventory>>(json);

            if (deserializedData == null)
                throw new InvalidDataException("Inventory save-file is corrupted and cannot be loaded.");

            _loadedInventories = new Dictionary<string, SpatialInventory>();

            foreach (SpatialInventory inventory in deserializedData)
            {
                _loadedInventories.Add(inventory.Name, inventory);
            }
        }


        private void SaveInventories()
        {
            List<SpatialInventory> toSave = new();

            foreach (KeyValuePair<string,SpatialInventory> pair in _spatialInventoriesToSave)
            {
                if(pair.Value == null)
                {
                    Logger.Log(LogLevel.WARN, $"Cannot save inventory '{pair.Key}', because it is null.");
                    return;
                }
                
                toSave.Add(pair.Value);
            }

            string json = JsonConvert.SerializeObject(toSave, Formatting.Indented, new SpatialInventoryConverter());
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
    }
}