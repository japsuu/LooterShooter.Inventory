using System.Collections.Generic;
using JetBrains.Annotations;
using UnInventory.Core.Extensions;
using UnInventory.Core.MVC.Model.Data;
using UnInventory.Standard;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryTesting.Scripts
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private GameObject _prefabInventory;
        [SerializeField] private Button _buttonOpenClose;


        private void Start()
        {
            IEnumerable<DataEntity> entities = ResourcesExt.LoadDataEntities("InventoryFolder");
            InventoryOpenCloseObject inventory = new(_prefabInventory, entities, "FirstInventory");
            _buttonOpenClose.onClick.AddListener(() => inventory.OpenClose());
        }
        //[SerializeField] private RectTransform _inventoryRootTransform;
        
        
        // [UsedImplicitly]
        // public void ToggleInventoryPanelVisibility()
        // {
        //     bool isInventoryVisible = _inventoryRootTransform.gameObject.activeInHierarchy;
        //
        //     _inventoryRootTransform.gameObject.SetActive(!isInventoryVisible);
        // }
    }
}
