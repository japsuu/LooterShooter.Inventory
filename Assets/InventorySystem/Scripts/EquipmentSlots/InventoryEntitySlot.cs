using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using InventorySystem.Inventories.Spatial.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.EquipmentSlots
{
    [RequireComponent(typeof(Image))]
    public abstract class InventoryEntitySlot : MonoBehaviour, IInventoryEntityDropTarget
    {
        [SerializeField] private RectTransform _contentRectTransform;
        
        [Tooltip("Items  this slot is restricted to. Leave empty to accept any kind of item.")]
        [SerializeField] private ItemType[] _itemTypeRestrictions;
        
        public Inventory Inventory => _inventory;
        public RectTransform RectTransform => _contentRectTransform;
        
        private Image _slotImage;
        private SimpleInventory _inventory;


        private void Awake()
        {
            _slotImage = GetComponent<Image>();
        }


        public abstract bool AcceptsEntity(InventoryEntity entity);


        public void OnEntityDropped(InventoryEntity entity)
        {
            throw new System.NotImplementedException();
        }


        public void OnEntityLifted(InventoryEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}