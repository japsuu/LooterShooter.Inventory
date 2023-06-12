// (c) Copyright Cleverous 2023. All rights reserved.

using Cleverous.VaultInventory.Scripts.Behaviors;
using UnityEngine;

namespace Cleverous.VaultInventory.Scripts.Interfaces
{
    /// <summary>
    /// An easy way to reference an Inventory and interact with it.
    /// </summary>
    public interface IUseInventory
    {
        Inventory Inventory { get; set; }
        Transform MyTransform { get; }
    }
}