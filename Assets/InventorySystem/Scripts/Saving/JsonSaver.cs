using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace InventorySystem.Saving
{
    public class JsonSaver<T>
    {
        private readonly string _saveFileName;


        protected JsonSaver(string saveFileName)
        {
            _saveFileName = saveFileName;
        }


        protected void SaveData(T data, params JsonConverter[] converters)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented, converters);
            WriteJsonToFile(_saveFileName, json);
        }


        protected T LoadData()
        {
            string json = ReadJsonFromFile(_saveFileName);

            if (string.IsNullOrEmpty(json))
            {
                Logger.Out(LogLevel.INFO, $"No {typeof(T)} save-file found.");
                return default;
            }
            
            T deserializedData = JsonConvert.DeserializeObject<T>(json);

            if (deserializedData == null)
                throw new InvalidDataException($"{typeof(T)} save-file is corrupted and cannot be loaded.");

            return deserializedData;
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