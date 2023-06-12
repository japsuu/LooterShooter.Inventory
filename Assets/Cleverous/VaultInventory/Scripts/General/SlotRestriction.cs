// (c) Copyright Cleverous 2023. All rights reserved.

using System;
using System.Collections.Generic;
using Cleverous.VaultInventory.Scripts.Behaviors;
using Spatial_Inventory.Data;

namespace Cleverous.VaultInventory.Scripts.General
{
    /// <summary>
    /// Used for reference comparisons to permit <see cref="RootItemStack"/>s to be placed in particular <see cref="ItemUiPlug"/> slots in an <see cref="Inventory"/>.
    /// </summary>
    [Serializable]
    public class SlotRestriction
    {
        public List<ItemTag> AllowedItemTags = new();
    }
}