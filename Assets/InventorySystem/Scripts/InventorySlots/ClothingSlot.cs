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

        public ClothingType AcceptedClothingType => _acceptedClothingType;

        
        protected override void Awake()
        {
            base.Awake();

            _removeButton.gameObject.SetActive(false);
        }
        
        
        private void OnEnable()
        {
            PlayerClothingManager.EquippedClothesChanged += OnEquippedClothesChanged;
        }


        private void OnDisable()
        {
            PlayerClothingManager.EquippedClothesChanged -= OnEquippedClothesChanged;
        }


        private void OnEquippedClothesChanged(ClothingType type, ItemMetadata itemMetadata)
        {
            if(type != _acceptedClothingType)
                return;
            
            if (itemMetadata == null)
            {
                RemoveItem(Vector2Int.zero);
            }
            else
            {
                InventoryBounds bounds = new(itemMetadata.ItemData, Vector2Int.zero, ItemRotation.DEG_0);
                InventoryItem newItem = new(itemMetadata, bounds, ItemRotation.DEG_0, this);
                
                AddItem(newItem);
            }
        }


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            if (!base.CanDropDraggableItem(draggableItem))
                return false;

            if (draggableItem.InventoryItem.Metadata.ItemData is not ClothingItemData clothing)
                return false;
            
            return clothing.ClothingType == _acceptedClothingType;
        }


        protected override void OnItemAdded()
        {
            base.OnItemAdded();

            if (AssignedItem.Metadata.ItemData is not ClothingItemData)
                return;
            
            _removeButton.gameObject.SetActive(true);
            _removeButton.onClick.AddListener(RequestRemoveClothing);
        }


        protected override void OnItemRemoved(ItemMetadata itemMetadata)
        {
            base.OnItemRemoved(itemMetadata);

            if (itemMetadata.ItemData is not ClothingItemData clothing)
                return;
            
            PlayerClothingManager.Singleton.RequestRemoveClothes(clothing.ClothingType);
            PlayerInventoryManager.Singleton.TryAddItems(itemMetadata, 1);
            
            _removeButton.onClick.RemoveListener(RequestRemoveClothing);
            _removeButton.gameObject.SetActive(false);
        }


        protected override void HandleDroppedDraggableItem(DraggableItem draggableItem)
        {
            base.HandleDroppedDraggableItem(draggableItem);
            
            PlayerClothingManager.Singleton.RequestEquipClothes(AssignedItem.Metadata);
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