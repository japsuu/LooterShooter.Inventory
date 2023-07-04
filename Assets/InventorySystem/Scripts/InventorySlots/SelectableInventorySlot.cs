using UnityEngine;

namespace InventorySystem.InventorySlots
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