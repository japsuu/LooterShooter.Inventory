using System;
using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem.Inventories
{
    /// <summary>
    /// Controls multiple child <see cref="Inventory"/>s.TODO: Split to manager and an event-based renderer.
    /// </summary>
    [DefaultExecutionOrder(-101)]
    public class PlayerInventoryManager : MonoBehaviour
    {
        public static PlayerInventoryManager Singleton;
        
        [Header("References")]
        [SerializeField] private InventoryRenderer _inventoryRendererPrefab;
        [SerializeField] private RectTransform _inventoryRenderersRoot;
        
        [Header("Base Inventory")]
        [SerializeField] private string _baseInventoryName = "Pockets";
        [SerializeField, Min(0)] private int _baseInventoryWidth = 8;
        [SerializeField, Min(0)] private int _baseInventoryHeight = 4;
        
        private Dictionary<string, Inventory> _inventories;
        private Dictionary<string, InventoryRenderer> _renderers;


        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
            
            _inventories = new();
            _renderers = new();
            
            // Destroy all children >:).
            for (int i = _inventoryRenderersRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(_inventoryRenderersRoot.GetChild(i).gameObject);
            }
        }


        private void Start()
        {
            // Add base inventory.
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
            
            Inventory inventory = new(inventoryName, width, height);
            
            InventoryRenderer inventoryRenderer = Instantiate(_inventoryRendererPrefab, _inventoryRenderersRoot);
            inventoryRenderer.RenderInventory(inventory, inventoryName);
            
            inventory.AddedItem += OnAddedItem;
            inventory.MovedItem += OnMovedItem;
            inventory.RemovedItem += OnRemovedItem;

            _inventories.Add(inventoryName, inventory);
            _renderers.Add(inventoryName, inventoryRenderer);
            
            Logger.Log(LogLevel.DEBUG, $"{nameof(Inventory)}: {gameObject.name}", $"AddInventory '{inventory}'");
        }


        private void OnAddedItem(Inventory.AddItemEventArgs addItemEventArgs)
        {
            if (_renderers.TryGetValue(addItemEventArgs.AddedItem.ContainingInventory.Name, out InventoryRenderer inventoryRenderer))
            {
                inventoryRenderer.CreateNewDraggableItem(addItemEventArgs.AddedItem);
            }
        }


        private void OnRemovedItem(Inventory.RemoveItemEventArgs removeItemEventArgs)
        {
            if (_renderers.TryGetValue(removeItemEventArgs.RemovedItem.ContainingInventory.Name, out InventoryRenderer inventoryRenderer))
            {
                inventoryRenderer.RemoveEntityOfItem(removeItemEventArgs.RemovedItem);
            }
        }


        private void OnMovedItem(Inventory.MoveItemEventArgs moveItemEventArgs)
        {
            if (!_renderers.TryGetValue(moveItemEventArgs.OldItem.ContainingInventory.Name, out InventoryRenderer oldInventoryRenderer))
            {
                Logger.Log(LogLevel.WARN, $"{nameof(Inventory)}: {gameObject.name}", "Could not get the renderer of the old inventory of a moved item.");
                return;
            }
            
            if (!_renderers.TryGetValue(moveItemEventArgs.NewItem.ContainingInventory.Name, out InventoryRenderer newInventoryRenderer))
            {
                Logger.Log(LogLevel.WARN, $"{nameof(Inventory)}: {gameObject.name}", "Could not get the renderer of the new inventory of a moved item.");
                return;
            }
            
            oldInventoryRenderer.RemoveEntityOfItem(moveItemEventArgs.OldItem);
            newInventoryRenderer.CreateNewDraggableItem(moveItemEventArgs.NewItem);
        }


        public void RemoveInventory(string inventoryName)
        {
            if (!_inventories.Remove(inventoryName, out Inventory inventory))
            {
                Logger.Log(LogLevel.WARN, $"{nameof(Inventory)}: {gameObject.name}", $"Could not get the inventory with the given name ({inventoryName}).");
                return;
            }
            
            inventory.AddedItem -= OnAddedItem;
            inventory.MovedItem -= OnMovedItem;
            inventory.RemovedItem -= OnRemovedItem;
                
            if (_renderers.Remove(inventoryName, out InventoryRenderer inventoryRenderer))
            {
                Destroy(inventoryRenderer.gameObject);
            }
                
            //TODO: Drop items to ground or something? Add them to the (possible) new inventory that replaced this?
            Logger.Log(LogLevel.WARN, $"{nameof(Inventory)}: {gameObject.name}", "NotImplemented: I have no idea what to do with the items from the inventory you just removed!");
        }


        public List<InventoryItem> TryAddItems(ItemData itemData, int count)
        {
            List<InventoryItem> results = new();
            foreach (Inventory inventory in _inventories.Values)
            {
                if (results.Count == count)
                    return results;
                
                results.AddRange(inventory.TryAddItems(itemData, count));
            }

            return results;
        }


        public List<InventoryItem> TryRemoveItems(ItemData itemData, int count)
        {
            List<InventoryItem> results = new();
            foreach (Inventory inventory in _inventories.Values)
            {
                if (results.Count == count)
                    return results;
                
                results.AddRange(inventory.TryRemoveItems(itemData, count));
            }

            return results;
        }


        public List<InventoryItem> GetAllItemsOfType(ItemData itemData)
        {
            List<InventoryItem> results = new();
            foreach (Inventory inventory in _inventories.Values)
            {
                results.AddRange(inventory.GetAllItemsOfType(itemData));
            }

            return results;
        }
    }
}