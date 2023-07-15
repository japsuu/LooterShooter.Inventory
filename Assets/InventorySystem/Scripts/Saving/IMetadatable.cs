namespace InventorySystem.Saving
{
    public interface IMetadatable<T>
    {
        public T Data { get; }


        public void SetMetadata(string key, string value);


        public bool TryGetMetadata(string key, out string value);
    }
}