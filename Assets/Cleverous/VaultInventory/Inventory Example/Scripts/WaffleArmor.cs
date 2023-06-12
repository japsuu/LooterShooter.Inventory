﻿// (c) Copyright Cleverous 2023. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory.Inventory_Example.Scripts
{
    public class WaffleArmor : WaffleBaseEquipment
    {
        [Header("[Armor]")]
        public int DefenseValue;

        protected override void Reset()
        {
            base.Reset();
            DefenseValue = 5;
        }
    }
}