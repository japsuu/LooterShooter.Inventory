﻿// (c) Copyright Cleverous 2023. All rights reserved.

using Cleverous.VaultInventory.Scripts.General;
using UnityEngine;

namespace Cleverous.VaultInventory.Inventory_Example.Scripts
{
    public class WaffleResource : RootItem
    {
        [Header("[Resource]")]
        public int SomeCraftingValue;

        protected override void Reset()
        {
            base.Reset();
            MaxStackSize = 500;
            SomeCraftingValue = 123;
        }
    }
}