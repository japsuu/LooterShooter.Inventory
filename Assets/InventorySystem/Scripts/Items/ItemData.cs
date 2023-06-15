using UnityEngine;

namespace InventorySystem.Items
{
    /// <summary>
    /// Data-asset for an item.
    /// </summary>
    [CreateAssetMenu(fileName = "ItData_", menuName = "Items/New Item", order = 0)]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string _itemName = "MissingYes";
        [SerializeField] private string _itemDescription = "Missing Description";
        [SerializeField] private Sprite itemSprite;
        [SerializeField, Min(1)] private int _inventoryWidth = 1;
        [SerializeField, Min(1)] private int _inventoryHeight = 1;

        public int HashId => GetHashCode();
        public string Name => _itemName;
        public Sprite Sprite => itemSprite;
        public string Description => _itemDescription;
        public int InventoryWidth => _inventoryWidth;
        public int InventoryHeight => _inventoryHeight;
    }
}