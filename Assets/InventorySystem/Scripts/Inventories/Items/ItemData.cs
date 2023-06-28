using InventorySystem.EquipmentSlots;
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
        [SerializeField] private string _itemName = "MissingYes";
        [SerializeField] private string _itemDescription = "Missing Description";
        [FormerlySerializedAs("itemSprite")] [SerializeField] private Sprite _itemSprite;
        [SerializeField] private ItemType _itemType = ItemType.MATERIAL;
        [SerializeField, Min(1)] private int _inventorySizeX = 1;
        [SerializeField, Min(1)] private int _inventorySizeY = 1;

        public int HashId => GetHashCode();
        public string Name => _itemName;
        public Sprite Sprite => _itemSprite;
        public string Description => _itemDescription;
        public int InventorySizeX => _inventorySizeX;
        public int InventorySizeY => _inventorySizeY;
        public ItemType ItemType => _itemType;
    }
}