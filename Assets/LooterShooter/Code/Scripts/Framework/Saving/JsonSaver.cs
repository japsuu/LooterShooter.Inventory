using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace LooterShooter.Framework.Saving
{
    /// <summary>
    /// Saves/loads the given type to/from the given file when <see cref="SaveData"/> or <see cref="LoadData"/> is called.
    /// </summary>
    /// <typeparam name="T">Type of data to save.</typeparam>
    public abstract class JsonSaver<T>
    {
        private readonly string _saveFilePath;


        protected JsonSaver(string saveFilePath)
        {
            _saveFilePath = saveFilePath;
        }


        protected void SaveData(T data, params JsonConverter[] converters)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented, converters);
            WriteJsonToFile(json);
        }


        protected T LoadData()
        {
            string json = ReadJsonFromFile();

            if (string.IsNullOrEmpty(json))
            {
                Logger.Write(LogLevel.INFO, $"No save-file for type {typeof(T)} found.");
                return default;
            }
            
            T deserializedData = JsonConvert.DeserializeObject<T>(json);

            if (deserializedData == null)
                throw new InvalidDataException($"{typeof(T)} save-file cannot be deserialized (corrupted..?) and wont be loaded.");

            return deserializedData;
        }

        
        private void WriteJsonToFile(string json)
        {
            FileStream fileStream = new(_saveFilePath, FileMode.Create);

            using StreamWriter writer = new(fileStream);
            writer.Write(json);
        }
        

        [CanBeNull]
        private string ReadJsonFromFile()
        {
            if (!File.Exists(_saveFilePath))
                return null;
            
            using StreamReader reader = new(_saveFilePath);
            string json = reader.ReadToEnd();
            return json;
        }
    }
}