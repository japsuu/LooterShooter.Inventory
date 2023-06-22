using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using InventorySystem.Inventories.Spatial;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.EquipmentSlots
{
    [RequireComponent(typeof(Image))]
    public abstract class ItemSlot : MonoBehaviour, IItemDropTarget
    {
        [SerializeField] private RectTransform _contentRectTransform;
        
        public Inventory Inventory => _inventory;
        public RectTransform RectTransform => _contentRectTransform;
        
        private Image _slotImage;
        private SingleStackInventory _inventory;


        private void Awake()
        {
            _slotImage = GetComponent<Image>();
        }


        public bool AcceptsItem(InventoryItem item, InventoryBounds itemBounds)
        {
            
        }


        public bool TryReceiveItem(IItemDropTarget fromTarget, Vector2Int fromPosition, Vector2Int toPosition, ItemRotation toRotation)
        {
            
        }
    }
}