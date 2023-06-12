// (c) Copyright Cleverous 2023. All rights reserved.

using Cleverous.VaultInventory.Scripts.General;
using UnityEngine;

namespace Cleverous.VaultInventory.Inventory_Example.Scripts
{
    public abstract class WaffleBaseEquipment : RootItem
    {
        [Header("[Equipment]")]
        public int Weight;

        protected override void Reset()
        {
            base.Reset();
            Weight = 42;
        }
    }
}