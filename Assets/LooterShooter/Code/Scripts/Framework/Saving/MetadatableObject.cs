using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LooterShooter.Framework.Saving
{
    [Serializable]
    public abstract class MetadatableObject : IMetadatable
    {
        [JsonProperty("metadata")]
        private Dictionary<string, object> _assignedMetadata;


        protected MetadatableObject(Dictionary<string, object> initialMetadata)
        {
            _assignedMetadata = initialMetadata ?? new();
        }


        public Dictionary<string, object> AssignedMetadata => _assignedMetadata;


        public void SetMetadata(string key, object value) => _assignedMetadata[key] = value;


        /// <param name="key">Address of the metadata.</param>
        /// <param name="value">Returned metadata</param>
        /// <typeparam name="T">Expected type of metadata</typeparam>
        /// <returns>True if metadata was found and is of expected type, otherwise false.</returns>
        public bool TryGetMetadata<T>(string key, out T value)
        {
            bool hasMetadata = _assignedMetadata.TryGetValue(key, out object deserializedValue);
            if (!hasMetadata)
            {
                value = default;
                return false;
            }

            try
            {
                value = (T)deserializedValue;
            }
            catch (Exception)
            {
                Logger.Write(LogLevel.ERROR, $"Cannot get metadata with key {key} as the returned data is not of the expected type {typeof(T)}.");
                value = default;
                return false;
            }
            
            return true;
        }
    }
    
    
    public interface IMetadatable
    {
        public void SetMetadata(string key, object value);


        public bool TryGetMetadata<T>(string key, out T value);
    }
    
    
    /*
     NOTE: Below is the generic MetadatableObject implementation.
     NOTE: Currently not in use because I have no idea how generics behave with json serialization, nor do I currently want to find out.
     
     public abstract class MetadatableObject<T> : IMetadatable<T>
    {
        public T Data { get; }

        [JsonProperty("metadata")]
        private Dictionary<string, string> _metadata;


        public MetadatableObject(T data)
        {
            Data = data;
            _metadata = new();
        }
        

        public void SetMetadata(string key, string value)
        {
            _metadata[key] = value;
        }


        public bool TryGetMetadata(string key, out string value) => _metadata.TryGetValue(key, out value);
    }
    
    
    public interface IMetadatable<T>
    {
        public T Data { get; }


        public void SetMetadata(string key, string value);


        public bool TryGetMetadata(string key, out string value);
    }*/
}