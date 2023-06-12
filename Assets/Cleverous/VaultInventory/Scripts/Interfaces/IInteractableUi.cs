﻿// (c) Copyright Cleverous 2023. All rights reserved.

using Cleverous.VaultInventory.Scripts.Interactions;
using UnityEngine;

namespace Cleverous.VaultInventory.Scripts.Interfaces
{
    /// <summary>
    /// <para>An Interactable UI Item. Could also be used for other interactables, like structures in the world, but mainly for UI plugs.</para>
    /// </summary>
    public interface IInteractableUi
    {
        Transform MyTransform { get; }
        Interaction[] Interactions { get; }
        void Interact(IUseInventory interactor = null);
        void InteractionSelect();
        void InteractionDeselect();
    }
}