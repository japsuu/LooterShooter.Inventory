using System;

namespace InventorySystem.Inventories.Items
{
    [Serializable]
    public enum ItemType
    {
        MATERIAL = 10,
        CONSUMABLE = 20,
        WEAPON = 30,
        WEAPON_ATTACHMENT = 31,
        CLOTHING = 40
    }
}