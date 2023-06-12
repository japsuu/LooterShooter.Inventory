﻿// (c) Copyright Cleverous 2023. All rights reserved.

using System;
using System.Collections.Generic;
using Cleverous.NetworkImposter;
using Cleverous.VaultInventory.Scripts.General;
using Cleverous.VaultInventory.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if MIRROR
using Mirror;
#elif FISHNET
using FishNet.Object;
#endif

namespace Cleverous.VaultInventory.Scripts.Behaviors
{
    /// <summary>
    /// This class manages the UI side of the <see cref="Inventory"/> by listening to a target for OnChanged events and refreshing the UI slot that changed.
    /// </summary>
    public class InventoryUi : UiBlockingPanel
    {
        public enum DragDropAction { Cancel, Move, Discard }
        public static Action OnAnyClickedItemChanged;
        public static Action OnAnyInventoryUiOpened;
        public static Action OnAnyInventoryUiClosed;
        public static ItemUiPlug DragOrigin;
        public static ItemUiPlug DragDestination;
        public static GameObject DragFloater;
        public static ItemUiPlug ClickedItem
        {
            get => m_clickedItem;
            set
            {
                m_clickedItem = value;
                OnAnyClickedItemChanged?.Invoke();
            }
        }
        private static ItemUiPlug m_clickedItem;
        

        [Header("References")]
        [Tooltip("The GameObject that contains the UI elements that control this Inventory. Should be either this gameObject, or one higher in the hierarchy - a parent 'wrapper' object.")]
        public GameObject Container;
        [Tooltip("The Grid Layout component that will host the Item Plugs in the UI.")]
        public GridLayoutGroup GridUi;
        [Tooltip("In order to put items in any of these slots the item must be one of these Restriction types.\n\nUsing None will always slot any item.")]
        [AssetDropdown(typeof(SlotRestriction))]
        public SlotRestriction[] Restrictions;
        [Tooltip("This will make the UI bind to the given interface when OnPlayerSpawn is triggered.\n\nCheck this only if this particular inventory ui is for the Player.")]
        public bool BindOnPlayerSpawn;

        [Header("Runtime Only")]
        public List<ItemUiPlug> Slots;
        public Inventory TargetInventory;
        /// <summary>
        /// A map for translating inventory indexes to grid slot indexes and vice versa. KEY is Inventory Array Index, VALUE is Grid UI Slot Index.
        /// </summary>
        public Dictionary<int, int> IndexMap;

        public Action OnClosed;
        public Action OnOpened;

        protected override void Awake()
        {
            base.Awake();
            if (Container == null) Container = gameObject;
            IndexMap = new Dictionary<int, int>();
            if (BindOnPlayerSpawn) General.VaultInventory.OnPlayerSpawn += LoadInventoryOfPlayer;
            General.VaultInventory.OnStartSceneChange += ClearUi;
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            OnClosed?.Invoke();
            OnAnyInventoryUiClosed?.Invoke();
            if (DragFloater != null) Destroy(DragFloater);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            OpenUi();
        }

        public static void HandleDragEvent()
        {
            DragDropAction action = DetectDragDropAction();

            int originIndex = 0;
            int goalIndex = 0;

            if (action == DragDropAction.Discard)
            {
                DragOrigin.Ui.TargetInventory.DoDiscard(DragOrigin.ReferenceInventoryIndex);
            }
            else if (action == DragDropAction.Move)
            {
                // Move event (hotbar is involved)
                if (DragDestination.GetType() == typeof(HotbarUiPlug))
                {
                    HotbarUiPlug destination = (HotbarUiPlug)DragDestination;
                    HotbarUiPlug origin = null;

                    if (DragOrigin.GetType() == typeof(HotbarUiPlug)) origin = (HotbarUiPlug)DragOrigin;

                    if (origin != null && !origin.IsOnCooldown && !destination.IsOnCooldown)
                    {
                        // hotbar > hotbar
                        destination.UpdateUiForHotbarPlug(origin.TargetData);
                        origin.UpdateUiForHotbarPlug(null);
                    }
                    else if (DragOrigin.GetReferenceVaultItemData() is IUseableDataEntity && !destination.IsOnCooldown)
                    {
                        // inventory > hotbar
                        destination.UpdateUiForHotbarPlug((IUseableDataEntity)DragOrigin.GetReferenceVaultItemData());
                    }
                }
                // Move event (no hotbar involved)
                else if (DragOrigin.Ui != null && DragOrigin.Ui.TargetInventory != null)
                {
                    NetworkBehaviour[] behaviours = DragOrigin.Ui.TargetInventory.GetNetworkBehaviours();
                    for (int i = 0; i < behaviours.Length; i++)
                    {
                        if (behaviours[i] == DragOrigin.Ui.TargetInventory) originIndex = i;
                    }

                    if (DragDestination.Ui == null || DragDestination.Ui.TargetInventory == null) return;
                    behaviours = DragDestination.Ui.TargetInventory.GetNetworkBehaviours();
                    for (int i = 0; i < behaviours.Length; i++)
                    {
                        if (behaviours[i] == DragDestination.Ui.TargetInventory) goalIndex = i;
                    }

                    // Debug.Log($"<color=red>CLIENT REQUEST INVENTORY {action} ACTION - Origin is index {originIndex}, goal is index {goalIndex}</color>");
                    DragOrigin.Ui.TargetInventory.CmdRequestMove(
                        DragOrigin.Ui.TargetInventory.NetId(),
                        DragDestination.Ui.TargetInventory.NetId(),
                        DragOrigin.ReferenceInventoryIndex,
                        DragDestination.ReferenceInventoryIndex,
                        originIndex,
                        goalIndex);
                }
            }

            if (ClickedItem != null) ClickedItem.SetAsNotEngaged();
            if (DragOrigin != null) DragOrigin.SetAsNotEngaged();
            if (DragDestination != null) DragDestination.SetAsNotEngaged();
            if (DragFloater != null) Destroy(DragFloater);

            ClickedItem = null;
            DragOrigin = null;
            DragDestination = null;
        }
        public static DragDropAction DetectDragDropAction()
        {
            if (DragOrigin != null && DragDestination != null) return DragDropAction.Move;
            if (DragOrigin != null && DragDestination == null) return DragDropAction.Discard;
            return DragDropAction.Cancel;
        }

        public virtual void LoadInventoryOfPlayer(IUseInventory avatar)
        {
            SetTargetInventory(avatar.Inventory);
        }
        public virtual void SetTargetInventory(Inventory inv)
        {
            if (inv == null)
            {
                //Debug.LogError("Inventory UI tried to hook into target inventory but the Inventory is invalid!", this);
                return;
            }

            // Debug.Log($"<color=yellow>||| Inventory UI set target to '{inv.gameObject.name}'.</color>", this);

            ClearUi();
            inv.OnInitialized += ReloadCurrentInventory; // If it's not ready or is ever is initialized again, we have to reload. Basicaly 'wait' for the right time via event.
            TargetInventory = inv;

            if (!TargetInventory.IsInitialized)
            {
                //Debug.Log($"<color=red>    FAILED. Inventory was not Initialized for '{inv.gameObject.name}' but we are watching it for initialization.</color>", this);
                return;
            }

            // Create new by looping through the target inventory slots
            for (int indexInv = 0; indexInv < inv.MaxSlots; indexInv++)
            {
                // compare each restriction for this UI grid to the target inventory's slot restriction on this index
                foreach (SlotRestriction sr in Restrictions)
                {
                    // if there's a match, then we can add a slot plug on this UI grid since its a restriction type we want to see here.
                    if (inv.Configuration.SlotRestrictions[indexInv] == sr)
                    {
                        AddNewSlot(indexInv);
                    }
                }
            }

            // Subscribe new
            inv.OnChanged += UpdateUi;
            inv.OnDestroyed += DestroyedCleanup;
            if (!NetworkPipeline.StaticIsServer()) inv.CmdRefreshAllFromServer();

            // OpenUi();

            //Debug.Log($"<color=lime>    SUCCESS. Inventory linked {Slots.Count} slots to ui '{gameObject.name}'.</color>", this);
        }
        private void ReloadCurrentInventory()
        {
            //Debug.Log($"<color=yellow> >>> Inventory UI RELOAD called --------------------.</color>", this);
            SetTargetInventory(TargetInventory);
        }
        protected void AddNewSlot(int inventoryIndex)
        {
            GameObject x = Instantiate(General.VaultInventory.ItemSlotTemplate, GridUi.transform);
            ItemUiPlug comp = x.GetComponentInChildren<ItemUiPlug>();
            comp.ReferenceInventoryIndex = inventoryIndex;
            comp.Ui = this;
            Slots.Add(comp);
            int slotIndex = Slots.Count - 1;
            IndexMap.Add(inventoryIndex, slotIndex);
            // Debug.Log($"<color=cyan>||| Index {inventoryIndex} mapped to slot index {slotIndex}</color>");
            UpdateUi(inventoryIndex);
        }

        public virtual void UpdateUi(int inventoryIndex)
        {
            // check for responsibilities. does this script handle this slot?
            if (!IndexMap.ContainsKey(inventoryIndex)) return;

            int slotIndex = IndexMap[inventoryIndex];
            //Debug.Log($"<color=cyan>    UPDATING SLOT {slotIndex} for inventory index {inventoryIndex} on '{gameObject.name}'.</color>", this);

            Slots[slotIndex].UpdateUi(TargetInventory.Get(inventoryIndex), TargetInventory.Configuration.SlotRestrictions[inventoryIndex]);
        }
        public virtual void ClearUi()
        {
            //Debug.Log($"<color=cyan>    ||| Clearing Inventory UI for '{gameObject.name}'.</color>", gameObject);
            if (TargetInventory != null) TargetInventory.OnChanged -= UpdateUi;

            foreach (ItemUiPlug x in Slots) Destroy(x.SlotOwnerObject);
            Slots = new List<ItemUiPlug>();
            IndexMap = new Dictionary<int, int>();
        }
        public override void CloseUi()
        {
            base.CloseUi();

            Container.SetActive(false);
            OnAnyInventoryUiClosed?.Invoke();
            OnClosed?.Invoke();

            EventSystem.current.SetSelectedGameObject(null);
        }
        public override void OpenUi()
        { 
            base.OpenUi();

            Container.SetActive(true);
            OnAnyInventoryUiOpened?.Invoke();
            OnOpened?.Invoke();

            if (Slots == null || Slots.Count == 0) return;
            EventSystem.current.SetSelectedGameObject(Slots[0].gameObject);
        }

        /// <summary>
        /// In cases where the target inventory is destroyed but no replacement is made, we have to wipe the UI to prevent null refs everywhere.
        /// Normally this is not an issue because of respawn immediately assigning a new target, but possibly in rare cases there may not be an inventory to assign.
        /// </summary>
        /// <param name="inv">The inventory that is being destroyed.</param>
        protected virtual void DestroyedCleanup(Inventory inv)
        {
            // if we already have a new target that is different, then its safe to say we've already 
            // taken care of the UI slots and populated it with valid content. If not, we must clear the UI.
            if (inv != TargetInventory) return;
            ClearUi();
        }
    }
}