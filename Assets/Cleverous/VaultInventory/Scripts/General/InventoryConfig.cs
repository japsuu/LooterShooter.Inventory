// (c) Copyright Cleverous 2023. All rights reserved.

using System;

namespace Cleverous.VaultInventory.Scripts.General
{
    [Serializable]
    public class InventoryConfig
    {
        public SlotRestriction[] SlotRestrictions;

        public InventoryConfig()
        {
            SlotRestrictions = new SlotRestriction[10];
        }
    }
}