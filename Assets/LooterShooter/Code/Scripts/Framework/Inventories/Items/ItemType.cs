using System;

namespace LooterShooter.Framework.Inventories.Items
{
    [Serializable]
    public enum ItemType
    {
        /// <summary>
        /// Item can be crafted with.
        /// </summary>
        MATERIAL = 10,
        
        /// <summary>
        /// Item can be consumed by the player.
        /// </summary>
        CONSUMABLE = 20,
        
        /// <summary>
        /// Item can be used as weapon.
        /// </summary>
        WEAPON = 30,
        
        /// <summary>
        /// Item can be attached to a weapon.
        /// </summary>
        WEAPON_ATTACHMENT = 31,
        
        /// <summary>
        /// Item can be worn as clothing.
        /// </summary>
        CLOTHING = 40
    }
}