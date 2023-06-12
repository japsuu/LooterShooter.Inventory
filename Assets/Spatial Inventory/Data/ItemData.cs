using System.Collections.Generic;
using UnityEngine;

namespace Spatial_Inventory.Data
{
    [CreateAssetMenu(fileName = "ItData_", menuName = "Prototyping/Item Data Asset", order = 0)]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string _itemName = "MissingYes";
        [SerializeField] private string _itemDescription = "Missing Description";
        [SerializeField] private Sprite _itemSprite;
        [SerializeField] private List<ItemTag> _tags;

        public int HashId => GetHashCode();
        public string Name => _itemName;
        public Sprite Sprite => _itemSprite;
        public string Description => _itemDescription;
        public List<ItemTag> Tags => _tags;
    }
}