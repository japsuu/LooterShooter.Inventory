/*using System.Collections.Generic;
using Newtonsoft.Json;

namespace LooterShooter.Framework.Saving
{
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
    }
}*/