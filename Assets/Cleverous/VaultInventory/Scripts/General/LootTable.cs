﻿// (c) Copyright Cleverous 2023. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory.Scripts.General
{
    public class LootTable : DataEntity
    {
        [Header("[Loot Table]")]
        [AssetDropdown(typeof(RootItem))]
        public RootItem[] Items;
        public int[] Amounts;
        public float[] Weights;

        protected virtual bool ErrorCheck()
        {
            if (Items.Length == 0 || Amounts.Length != Items.Length || Weights.Length != Items.Length)
            {
                Debug.LogError("Loot Table error! Either no items or there are mismatched array lengths.", this);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Grab one item from the Loot Table.
        /// </summary>
        public RootItemStack GetLoot()
        {
            if (ErrorCheck()) return null;

            int index = Random.Range(0, Items.Length);
            return new RootItemStack(Items[index], Amounts[index]);
        }
    }
}