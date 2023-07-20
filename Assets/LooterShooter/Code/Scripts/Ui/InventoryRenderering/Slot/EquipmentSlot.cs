using System;
using System.Linq;
using LooterShooter.Framework;
using LooterShooter.Framework.Inventories;
using LooterShooter.Framework.Inventories.Items;
using LooterShooter.Framework.Saving;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Logger = LooterShooter.Framework.Logger;

namespace LooterShooter.Ui.InventoryRenderering.Slot
{
    public class EquipmentSlot : InventorySlot, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static event Action<EquipmentSlot> ContentsChanged;
        
        [SerializeField] private KeyCode _selectKey = KeyCode.Alpha1;

        [SerializeField] private TMP_Text _selectKeyText;
        [SerializeField] private TMP_Text _itemNameText;
        
        [Tooltip("Unique id of this slot. Used for saving it's contents.")]
        [SerializeField] private string _uniqueIdentifier = "weapon_1";
        
        [Tooltip("What types of items can be dropped to this slot. Leave empty to not allow any items.")]
        [SerializeField] private ItemType[] _itemTypeRestrictions;
        
        private DraggableItem _draggedItem;

        protected override string Identifier => $"equipment_{_uniqueIdentifier}";
        protected override ItemType[] ItemTypeRestrictions => _itemTypeRestrictions;


        protected override void Awake()
        {
            base.Awake();

            _selectKeyText.text = ExtractNumbersOrLetters(_selectKey);
            _itemNameText.text = "";

            PlayerSaveData saveData = SaveSystem.Singleton.GetLocalPlayerSaveData();
            
            if (saveData == null)
                return;
            
            if (saveData.SavedEquipmentSlots == null)
                return;
                
            foreach (EquipmentSlotSaveData slot in saveData.SavedEquipmentSlots)
            {
                if (slot.Identifier != Name)
                    continue;

                if (slot.ContainedItem != null)
                {
                    AddItem(
                        new InventoryItem(
                            slot.ContainedItem,
                            new InventoryBounds(
                                slot.ContainedItem.ItemData, Vector2Int.zero, InventoryItemRotation.DEG_0),
                            InventoryItemRotation.DEG_0, this));
                }

                break;
            }
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
            string item = AssignedItem == null ? "None" : AssignedItem.Metadata.ItemData.ItemName;
            Logger.Write(LogLevel.INFO, Name, $"SelectedItem: {item}.");
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
            
            ContentsChanged?.Invoke(this);
        }


        protected override void OnItemAdded()
        {
            base.OnItemAdded();
            
            _itemNameText.text = AssignedItem.Metadata.ItemData.ItemName;
            
            ContentsChanged?.Invoke(this);
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