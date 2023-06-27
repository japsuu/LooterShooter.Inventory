using UnityEngine;

namespace InventorySystem.EquipmentSlots
{
    public class SelectableSlot : InventoryEntitySlot
    {
        [Tooltip("Key used to select this slot. Leave at 'none' to disable selection.")]
        [SerializeField] private KeyCode _selectKey = KeyCode.None;

        private bool _isSelectionEnabled;
        
        
        protected override void Awake()
        {
            _isSelectionEnabled = _selectKey != KeyCode.None;
        }


        protected override void Update()
        {
            if(_isSelectionEnabled)
                return;
            
            if (Input.GetKeyDown(_selectKey))
                Selected();
        }


        private void Selected()
        {
            
        }
    }
}