using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem.Inventories
{
    /// <summary>
    /// Controls multiple child <see cref="SpatialInventory"/>s.TODO: Split to manager and an event-based renderer.
    /// </summary>
    [DefaultExecutionOrder(-101)]
    public class PlayerInventoryManager : MonoBehaviour
    {
        public static PlayerInventoryManager Singleton;
        
        [Header("References")]
        [SerializeField] private SpatialInventoryRenderer _spatialInventoryRendererPrefab;
        [SerializeField] private RectTransform _inventoryRenderersRoot;
        
        [Header("Base Inventory")]
        [SerializeField] private string _baseInventoryName = "Pockets";
        [SerializeField, Min(0)] private int _baseInventoryWidth = 4;
        [SerializeField, Min(0)] private int _baseInventoryHeight = 3;

        [SerializeField] private List<ItemData> _startingItems;

        private Dictionary<string, SpatialInventory> _inventories;
        private Dictionary<string, SpatialInventoryRenderer> _renderers;

        public bool HasSomethingInInventory => GetAllItems().Count > 0;


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

            foreach (ItemData startingItem in _startingItems)
            {
                TryAddItems(new ItemMetadata(startingItem), 1);
            }
        }


        public void AddInventory(string inventoryName, int width, int height)
        {
            if(width < 1 || height < 1)
                return;
            
            SpatialInventory spatialInventory = Persistence.Singleton.GetSpatialInventoryByName(inventoryName, width, height);
            
            SpatialInventoryRenderer spatialInventoryRenderer = Instantiate(_spatialInventoryRendererPrefab, _inventoryRenderersRoot);
            spatialInventoryRenderer.RenderInventory(spatialInventory, inventoryName);
            
            spatialInventory.AddedItem += OnAddedItem;
            spatialInventory.RemovedItem += OnRemovedItem;

            _inventories.Add(inventoryName, spatialInventory);
            _renderers.Add(inventoryName, spatialInventoryRenderer);
            
            Logger.Log(LogLevel.DEBUG, $"{nameof(SpatialInventory)}: {gameObject.name}", $"AddInventory '{spatialInventory}'");
        }


        private void OnAddedItem(AddItemEventArgs addItemEventArgs)
        {
            if (_renderers.TryGetValue(addItemEventArgs.AddedItem.ContainingInventory.Name, out SpatialInventoryRenderer inventoryRenderer))
            {
                inventoryRenderer.CreateNewDraggableItem(addItemEventArgs.AddedItem);
            }
        }


        private void OnRemovedItem(RemoveItemEventArgs removeItemEventArgs)
        {
            if (_renderers.TryGetValue(removeItemEventArgs.RemovedItem.ContainingInventory.Name, out SpatialInventoryRenderer inventoryRenderer))
            {
                inventoryRenderer.RemoveEntityOfItem(removeItemEventArgs.RemovedItem);
            }
        }


        public void RemoveInventory(string inventoryName)
        {
            if (!_inventories.Remove(inventoryName, out SpatialInventory inventory))
            {
                Logger.Log(LogLevel.WARN, $"{nameof(SpatialInventory)}: {gameObject.name}", $"Could not get the inventory with the given name ({inventoryName}).");
                return;
            }
            
            inventory.AddedItem -= OnAddedItem;
            inventory.RemovedItem -= OnRemovedItem;
            
            Persistence.Singleton.RemoveSpatialInventoryFromSaving(inventory);
                
            if (_renderers.Remove(inventoryName, out SpatialInventoryRenderer inventoryRenderer))
            {
                Destroy(inventoryRenderer.gameObject);
            }

            foreach (InventoryItem item in inventory.GetAllItems())
            {
                if (TryAddItems(item.Metadata, 1).Count < 1)
                {
                    //TODO: Drop items to ground or something? Add them to the (possible) new inventory that replaced this?
                    Logger.Log(
                        LogLevel.WARN, $"{nameof(SpatialInventory)}: {gameObject.name}",
                        "NotImplemented: I have no idea what to do with the items from the inventory you just removed!");
                }
            }
        }


        public List<InventoryItem> TryAddItems(ItemMetadata metadata, int count)
        {
            List<InventoryItem> results = new();
            foreach (SpatialInventory inventory in _inventories.Values)
            {
                if (results.Count == count)
                    return results;
                
                results.AddRange(inventory.RequestAddItems(metadata, count));
            }

            return results;
        }


        /// <summary>
        /// Tries to remove the given amount of defined itemData.
        /// We're using pure <see cref="ItemData"/> instead of <see cref="ItemMetadata"/>, since when removing items we probably do not care about the metadata of an item.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<InventoryItem> TryRemoveItems(ItemData data, int count)
        {
            List<InventoryItem> results = new();
            foreach (SpatialInventory inventory in _inventories.Values)
            {
                if (results.Count == count)
                    return results;
                
                results.AddRange(inventory.RequestRemoveItems(data, count));
            }

            return results;
        }


        public List<InventoryItem> GetAllItemsOfType(ItemData itemData)
        {
            List<InventoryItem> results = new();
            foreach (SpatialInventory inventory in _inventories.Values)
            {
                results.AddRange(inventory.GetAllItemsOfType(itemData));
            }

            return results;
        }


        public List<InventoryItem> GetAllItems()
        {
            List<InventoryItem> results = new();
            foreach (SpatialInventory inventory in _inventories.Values)
            {
                results.AddRange(inventory.GetAllItems());
            }

            return results;
        }
    }
}