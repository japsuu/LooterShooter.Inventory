using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using InventorySystem.Inventories;
using InventorySystem.Inventories.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace InventorySystem.Saving
{
    public class InventorySaver
    {
        private readonly string _saveFileName;
        private Dictionary<string, SpatialInventory> _loadedInventories;
        private Dictionary<string, SpatialInventory> _spatialInventoriesToSave;


        public InventorySaver(string saveFileName)
        {
            _saveFileName = saveFileName;
            
            _loadedInventories = new();
            _spatialInventoriesToSave = new();
            LoadInventories();
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
            AddSpatialInventoryForSaving(inventory);
            
            return inventory;
        }


        private bool TryLoadSpatialInventoryByName(string inventoryName, out SpatialInventory result)
        {
            return _loadedInventories.TryGetValue(inventoryName, out result);
        }


        private void AddSpatialInventoryForSaving(SpatialInventory toSave)
        {
            if (!_spatialInventoriesToSave.TryAdd(toSave.Name, toSave))
            {
                Logger.Log(
                    LogLevel.ERROR,
                    nameof(SaveSystem), 
                    $"Cannot register inventory '{toSave.Name}' for saving, as an existing inventory with the same name is already registered.");
            }
        }


        public void RemoveSpatialInventoryFromSaving(SpatialInventory spatialInventory)
        {
            _spatialInventoriesToSave.Remove(spatialInventory.Name);
            _loadedInventories.Remove(spatialInventory.Name);
        }


        private void LoadInventories()
        {
            string json = ReadJsonFromFile(_saveFileName);

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
            WriteJsonToFile(_saveFileName, json);
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


#if UNITY_EDITOR
        [UnityEditor.MenuItem("Looter Shooter/Open Save Folder")]
        private static void OpenSaveFolder()
        {
            Process.Start(Application.persistentDataPath);
        }
#endif
    }
}