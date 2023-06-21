using System;

namespace InventorySystem.EquipmentSlots
{
    [Flags]
    [Serializable]
    public enum ItemType
    {
        Nothing,
        Clothing,
        Weapon,
        Attachment
    }
}