using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem.Inventories
{
    /// <summary>
    /// Controls multiple child <see cref="Inventory"/>s.TODO: Split to event based renderer.
    /// </summary>
    public class PlayerInventoryManager : MonoBehaviour
    {
        public static PlayerInventoryManager Singleton;
        
        [Header("References")]
        [SerializeField] private InventoryRenderer _inventoryRendererPrefab;
        [SerializeField] private RectTransform _inventoryRenderersRoot;
        
        [Header("Base Inventory")]
        [SerializeField] private string _baseInventoryName = "Pockets";
        [SerializeField, Min(1)] private int _baseInventoryWidth = 8;
        [SerializeField, Min(1)] private int _baseInventoryHeight = 4;
        
        private Dictionary<string, Inventory> _inventories;
        private Dictionary<Inventory, InventoryRenderer> _renderers;


        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
            
            _inventories = new();
            _renderers = new();
        }


        private void Start()
        {
            // Destroy all children >:).
            for (int i = _inventoryRenderersRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(_inventoryRenderersRoot.GetChild(i).gameObject);
            }
            
            AddInventory(_baseInventoryName, _baseInventoryWidth, _baseInventoryHeight);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                //TODO: Notify UI to draw inventories.
            }
        }


        public void AddInventory(string inventoryName, int width, int height)
        {
            if(width < 1 || height < 1)
                return;
            
            Inventory inventory = new(width, height);
            
            InventoryRenderer inventoryRenderer = Instantiate(_inventoryRendererPrefab, _inventoryRenderersRoot);
            inventoryRenderer.RenderInventory(inventory, inventoryName);
            
            inventory.AddedItem += OnAddedItem;
            inventory.MovedItem += OnMovedItem;
            inventory.RemovedItem += OnRemovedItem;
            
            _inventories.Add(inventoryName, inventory);
            _renderers.Add(inventory, inventoryRenderer);
        }


        private void OnAddedItem(Inventory inventory, InventoryItem item)
        {
            if (_renderers.TryGetValue(inventory, out InventoryRenderer inventoryRenderer))
            {
                inventoryRenderer.CreateNewEntityForItem(item);
            }
        }


        private void OnRemovedItem(Inventory inventory, InventoryItem item)
        {
            if (_renderers.TryGetValue(inventory, out InventoryRenderer inventoryRenderer))
            {
                inventoryRenderer.RemoveEntityOfItem(item);
            }
        }


        private void OnMovedItem(Inventory oldInventory, Inventory newInventory, InventoryItem item, Vector2Int oldPos, Vector2Int newPos)
        {
            if (_renderers.TryGetValue(oldInventory, out InventoryRenderer oldInventoryRenderer) &&
                _renderers.TryGetValue(newInventory, out InventoryRenderer newInventoryRenderer))
            {
                oldInventoryRenderer.RemoveEntityOfItem(item);
                newInventoryRenderer.CreateNewEntityForItem(item);
            }
        }


        public void RemoveInventory(string inventoryName)
        {
            if (_inventories.Remove(inventoryName, out Inventory inventory))
            {
                inventory.AddedItem -= OnAddedItem;
                inventory.MovedItem -= OnMovedItem;
                inventory.RemovedItem -= OnRemovedItem;
                
                if (_renderers.Remove(inventory, out InventoryRenderer inventoryRenderer))
                {
                    Destroy(inventoryRenderer.gameObject);
                }
                
                //TODO: Drop items to ground or something? Add them to the (possible) new inventory that replaced this?
                Debug.LogError("NotImplemented: I have no idea what to do with the items from the inventory you just removed!");
            }
        }


        public bool TryAddItem(ItemData itemData)
        {
            foreach (Inventory inventory in _inventories.Values)
            {
                if (inventory.TryAddItem(itemData))
                    return true;
            }

            return false;
        }


        public int TryRemoveItems(ItemData item, int count)
        {
            int removedInTotal = 0;
            foreach (Inventory inventory in _inventories.Values)
            {
                removedInTotal += inventory.TryRemoveItems(item, count);

                if (removedInTotal == count)
                    return removedInTotal;
            }

            return removedInTotal;
        }


        public int ContainsItem(ItemData itemData)
        {
            return _inventories.Values.Sum(inventory => inventory.ContainsItem(itemData));
        }
    }
}