using System;
using UnityEngine;

namespace SourceData.Scripts
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
        [SerializeField] private InventorySize _inventorySize = new(1, 1);

        public int HashId => GetHashCode();
        public string Name => _itemName;
        public Sprite Sprite => itemSprite;
        public string Description => _itemDescription;
        public InventorySize InventorySize => _inventorySize;
    }


    [Serializable]
    public struct InventorySize
    {
        [Min(1)]
        public int X;
        
        [Min(1)]
        public int Y;


        public InventorySize(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}