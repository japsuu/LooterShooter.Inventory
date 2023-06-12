// (c) Copyright Cleverous 2023. All rights reserved.

using System;
using System.Collections.Generic;
using Cleverous.VaultInventory.Scripts.Behaviors;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cleverous.VaultInventory.Scripts.General
{
    /// <summary>
    /// For serializing and deserializing an <see cref="Inventory"/> and it's content from or to a particular state.
    /// </summary>
    [Serializable]
    public partial class InventoryState
    {
        public InventoryConfig Config;
        
        

        public InventoryState(Inventory source, List<RootItemStack> content)
        {
            //TODO: Implement.
            throw new NotImplementedException();
        }

        /// <summary>
        /// A simple way to JSON an <see cref="InventoryState"/> from an <see cref="Inventory"/>.
        /// </summary>
        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
        /// <summary>
        /// A simple way to get an <see cref="InventoryState"/> from saved JSON which can be used in <see cref="Inventory"/>.Initialize().
        /// </summary>
        public static InventoryState FromJson(string json)
        {
            return JsonUtility.FromJson<InventoryState>(json);
        }
    }
}