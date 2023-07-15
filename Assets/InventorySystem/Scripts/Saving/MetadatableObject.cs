using System.Collections.Generic;
using Newtonsoft.Json;

namespace InventorySystem.Saving
{
    public abstract class MetadatableObject<T> : IMetadatable<T>
    {
        public T Data { get; private set; }

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
}