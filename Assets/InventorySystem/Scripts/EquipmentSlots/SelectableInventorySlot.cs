using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem.EquipmentSlots
{
    public class SelectableInventorySlot : InventorySlot
    {
        [SerializeField] private KeyCode _selectKey = KeyCode.Alpha1;


        private void Update()
        {
            if (Input.GetKeyDown(_selectKey))
            {
                Selected();
            }
        }


        private void Selected()
        {
            Debug.LogWarning("NotImplemented: SelectedItem.");
        }
    }
}