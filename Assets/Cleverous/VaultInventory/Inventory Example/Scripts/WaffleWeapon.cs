// (c) Copyright Cleverous 2023. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory.Inventory_Example.Scripts
{
    public class WaffleWeapon : WaffleBaseEquipment
    {
        [Header("[Weapon]")]
        public int AttackValue;

        protected override void Reset()
        {
            base.Reset();
            AttackValue = 5;
        }
    }
}