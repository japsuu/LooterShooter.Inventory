using System.Linq;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem.InventorySlots
{
    public class EquipmentSlot : ItemSlot, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private KeyCode _selectKey = KeyCode.Alpha1;

        [SerializeField] private TMP_Text _selectKeyText;
        [SerializeField] private TMP_Text _itemNameText;
        
        [Tooltip("What types of items can be dropped to this slot. Leave empty to not allow any items.")]
        [SerializeField] private ItemType[] _itemTypeRestrictions;
        
        private DraggableItem _draggedItem;

        protected override ItemType[] ItemTypeRestrictions => _itemTypeRestrictions;


        protected override void Awake()
        {
            base.Awake();

            _selectKeyText.text = ExtractNumbersOrLetters(_selectKey);
            _itemNameText.text = "";
        }


        private void Update()
        {
            if (Input.GetKeyDown(_selectKey))
            {
                Selected();
            }
        }
        
        
        private void Selected()
        {
            string item = AssignedItem == null ? "None" : AssignedItem.Metadata.ItemData.Name;
            Logger.Log(LogLevel.INFO, Name, $"SelectedItem: {item}.");
        }

        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(AssignedItem == null)
                return;
            
            _draggedItem = Instantiate(PrefabReferences.Singleton.DraggableItemPrefab, RectTransform);

            _draggedItem.Initialize(AssignedItem, true);
            
            _draggedItem.OnBeginDrag(eventData);
        }


        public void OnDrag(PointerEventData eventData)
        {
            if(_draggedItem == null)
                return;
            
            _draggedItem.OnDrag(eventData);
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if(_draggedItem == null)
                return;
            
            _draggedItem.OnEndDrag(eventData);
        }


        protected override void OnItemRemoved(ItemMetadata itemMetadata)
        {
            base.OnItemRemoved(itemMetadata);
            
            _itemNameText.text = "";
            
            if(_draggedItem != null)
                Destroy(_draggedItem.gameObject);
        }


        protected override void OnItemAdded()
        {
            base.OnItemAdded();
            
            _itemNameText.text = AssignedItem.Metadata.ItemData.Name;
        }


        private static string ExtractNumbersOrLetters(KeyCode keycode)
        {
            string input = keycode.ToString();
            string numbers = new(input.Where(char.IsDigit).ToArray());
            string letters = new(input.Where(char.IsLetter).ToArray());

            if (!string.IsNullOrEmpty(numbers))
            {
                return numbers;
            }

            return string.IsNullOrEmpty(letters) ? "?" : letters;
        }
    }
}