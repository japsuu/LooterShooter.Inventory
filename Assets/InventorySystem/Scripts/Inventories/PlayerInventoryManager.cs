using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

namespace InventorySystem.Inventories
{
    /// <summary>
    /// Controls multiple child <see cref="InventoryX"/>s.TODO: Split to event based renderer.
    /// </summary>
    public class PlayerInventoryManager : MonoBehaviour
    {
        public static PlayerInventoryManager Singleton;
        
        [FormerlySerializedAs("_inventoryRendererPrefab")]
        [Header("References")]
        [SerializeField] private SpatialInventoryRenderer _spatialInventoryRendererPrefab;
        [SerializeField] private RectTransform _inventoryRenderersRoot;
        
        [Header("Base Inventory")]
        [SerializeField] private string _baseInventoryName = "Pockets";
        [SerializeField, Min(0)] private int _baseInventoryWidth = 8;
        [SerializeField, Min(0)] private int _baseInventoryHeight = 4;
        
        private Dictionary<string, InventoryX> _inventories;
        private Dictionary<InventoryX, SpatialInventoryRenderer> _renderers;


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
            
            // Add base inventory
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
            
            InventoryX inventoryX = new(width, height);
            
            SpatialInventoryRenderer spatialInventoryRenderer = Instantiate(_spatialInventoryRendererPrefab, _inventoryRenderersRoot);
            spatialInventoryRenderer.RenderInventory(inventoryX, inventoryName);
            
            inventoryX.ItemMetadataAdded += OnItemMetadataAdded;
            inventoryX.ItemMetadataChanged += OnItemMetadataChanged;
            inventoryX.ItemMetadataRemoved += OnItemMetadataRemoved;
            
            _inventories.Add(inventoryName, inventoryX);
            _renderers.Add(inventoryX, spatialInventoryRenderer);
            
            print($"added {inventoryName} ({width}x{height})");
        }


        private void OnItemMetadataAdded(InventoryX inventoryX, InventoryItem<> data)
        {
            if (_renderers.TryGetValue(inventoryX, out SpatialInventoryRenderer inventoryRenderer))
            {
                inventoryRenderer.CreateNewEntityForItem(data);
            }
        }


        private void OnItemMetadataRemoved(InventoryX inventoryX, InventoryItem<> data)
        {
            if (_renderers.TryGetValue(inventoryX, out SpatialInventoryRenderer inventoryRenderer))
            {
                inventoryRenderer.RemoveEntityOfItem(data);
            }
        }


        private void OnItemMetadataChanged(InventoryX oldInventoryX, InventoryX newInventoryX, InventoryItem<> data, Vector2Int oldPos, Vector2Int newPos)
        {
            if (_renderers.TryGetValue(oldInventoryX, out SpatialInventoryRenderer oldInventoryRenderer) &&
                _renderers.TryGetValue(newInventoryX, out SpatialInventoryRenderer newInventoryRenderer))
            {
                oldInventoryRenderer.RemoveEntityOfItem(data);
                newInventoryRenderer.CreateNewEntityForItem(data);
            }
        }


        public void RemoveInventory(string inventoryName)
        {
            if (_inventories.Remove(inventoryName, out InventoryX inventory))
            {
                inventory.ItemMetadataAdded -= OnItemMetadataAdded;
                inventory.ItemMetadataChanged -= OnItemMetadataChanged;
                inventory.ItemMetadataRemoved -= OnItemMetadataRemoved;
                
                if (_renderers.Remove(inventory, out SpatialInventoryRenderer inventoryRenderer))
                {
                    Destroy(inventoryRenderer.gameObject);
                }
                
                //TODO: Drop items to ground or something? Add them to the (possible) new inventory that replaced this?
                Debug.LogError("NotImplemented: I have no idea what to do with the items from the inventory you just removed!");
            }
        }


        public bool TryAddItem(ItemData itemData)
        {
            foreach (InventoryX inventory in _inventories.Values)
            {
                if (inventory.TryAddItems(itemData))
                    return true;
            }

            return false;
        }


        public int TryRemoveItems(ItemData item, int count)
        {
            int removedInTotal = 0;
            foreach (InventoryX inventory in _inventories.Values)
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