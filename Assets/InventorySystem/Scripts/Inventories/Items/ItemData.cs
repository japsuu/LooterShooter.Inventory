using UnityEngine;
using UnityEngine.Serialization;

namespace InventorySystem.Inventories.Items
{
    /// <summary>
    /// Data-asset for an item.
    /// </summary>
    [CreateAssetMenu(fileName = "ItData_", menuName = "Items/New Item", order = 0)]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private ItemType _itemType = ItemType.Material;
        [SerializeField] private string _itemName = "MissingYes";
        [SerializeField] private string _itemDescription = "Missing Description";
        [SerializeField] private Sprite _itemSprite;
        [SerializeField, Min(1)] private int _inventorySizeX = 1;
        [SerializeField, Min(1)] private int _inventorySizeY = 1;

        public string Type => _itemType.ToString().ToUpper();
        public int HashId => GetHashCode();
        public string Name => _itemName;
        public Sprite Sprite => _itemSprite;
        public string Description => _itemDescription;
        public int InventorySizeX => _inventorySizeX;
        public int InventorySizeY => _inventorySizeY;
    }
}