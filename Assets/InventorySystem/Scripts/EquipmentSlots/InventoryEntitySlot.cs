using System.Linq;
using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.EquipmentSlots
{
    /// <summary>
    /// An UI slot, that an InventoryEntity can be dropped on to.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public abstract class InventoryEntitySlot : MonoBehaviour, InventoryItemReceiver
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


        public virtual bool AcceptsFloater(Floater floater)
        {
            // Return true if we have a matching restriction or there is no restrictions.
            return _itemTypeRestrictions.Length < 1 ||
                   _itemTypeRestrictions.Any(restriction => restriction == floater.FloaterData.Metadata.Type);
        }


        public void OnEntityDropped(Floater floater)
        {
            throw new System.NotImplementedException();
        }


        public void OnEntityLifted(Floater floater)
        {
            throw new System.NotImplementedException();
        }
    }
}