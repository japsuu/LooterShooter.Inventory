using InventorySystem.Clothing;
using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;
using UnityEngine.UI;
using ItemType = InventorySystem.Inventories.Items.ItemType;

namespace InventorySystem.InventorySlots
{
    public class ClothingSlot : ItemSlot
    {
        protected override ItemType[] ItemTypeRestrictions => new[]
        {
            ItemType.CLOTHING
        };


        [SerializeField] private ClothingType _acceptedClothingType;
        [SerializeField] private Button _removeButton;


        protected override void Awake()
        {
            base.Awake();

            _removeButton.gameObject.SetActive(false);
        }


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            if (!base.CanDropDraggableItem(draggableItem))
                return false;

            if (draggableItem.InventoryItem.Metadata.ItemData is not ClothingItem clothing)
                return false;
            
            return clothing.Type == _acceptedClothingType;
        }


        protected override void OnItemAdded()
        {
            base.OnItemAdded();

            if (AssignedItem.Metadata.ItemData is not ClothingItem clothing)
                return;

            PlayerClothingManager.Singleton.TryEquipClothes(clothing);
            
            _removeButton.gameObject.SetActive(true);
            _removeButton.onClick.AddListener(RequestRemoveClothing);
        }


        protected override void OnItemRemoved(ItemMetadata itemMetadata)
        {
            base.OnItemRemoved(itemMetadata);

            if (itemMetadata.ItemData is not ClothingItem clothing)
                return;
            
            PlayerInventoryManager.Singleton.TryAddItems(itemMetadata, 1);
            PlayerClothingManager.Singleton.TryRemoveClothes(clothing.Type);
            
            _removeButton.onClick.RemoveListener(RequestRemoveClothing);
            _removeButton.gameObject.SetActive(false);
        }


        private void RequestRemoveClothing() => RemoveItem(Vector2Int.zero);


#if UNITY_EDITOR
        private void OnValidate()
        {
            if(UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;
            
            gameObject.name = $"Clothing Slot ({_acceptedClothingType.ToString()})";
        }
#endif
    }
}