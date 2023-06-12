﻿// (c) Copyright Cleverous 2023. All rights reserved.

using Cleverous.VaultInventory.Scripts.Behaviors;
using Cleverous.VaultInventory.Scripts.General;
using Cleverous.VaultInventory.Scripts.Interfaces;
using UnityEngine;

namespace Cleverous.VaultInventory.Inventory_Example.Scripts
{
    public class VaultExampleSoundManager : MonoBehaviour
    {
        [Header("For basic stuff")]
        public AudioClip ClipMoveBegin;
        public AudioClip ClipMoveEnd;
        public AudioClip ClipPickup;
        public AudioClip ClipSelected;

        [Header("For rare pickups")]
        public AudioClip ClipVictory;

        [Header("Special item")]
        [AssetDropdown(typeof(RootItem))]
        public RootItem CoinItem;
        public AudioClip ClipCoin;


        protected AudioSource SoundPlayer;
        protected IUseInventory Player;

        private void Start()
        {
            // hook into the UI's event for when a player is spawned
            VaultInventory.Scripts.General.VaultInventory.OnPlayerSpawn += SpawnHook;
        }

        protected virtual void SpawnHook(IUseInventory player)
        {
            Player = player;
            SoundPlayer = Player.MyTransform.GetComponent<AudioSource>();

            // Now let's subscribe to some events to get some actionable callbacks!
            VaultInventory.Scripts.General.VaultInventory.OnMoveItemBegin += OnLift;
            VaultInventory.Scripts.General.VaultInventory.OnMoveItemCancel += OnCancel;
            VaultInventory.Scripts.General.VaultInventory.OnMoveItemEnd += OnDrop;
            VaultInventory.Scripts.General.VaultInventory.OnSlotSelected += OnSelectedSlot;
            Player.Inventory.OnItemAdded += OnNewAdded;
        }

        protected virtual void OnLift(ItemUiPlug originSlot)
        {
            if (SoundPlayer == null) return;

            SoundPlayer.clip = ClipMoveBegin;
            SoundPlayer.Play();
        }

        protected virtual void OnCancel(ItemUiPlug slot)
        {
            if (SoundPlayer == null) return;

            SoundPlayer.clip = ClipMoveBegin;
            SoundPlayer.Play();
        }

        protected virtual void OnDrop(ItemUiPlug destinationSlot)
        {
            if (SoundPlayer == null) return;

            SoundPlayer.clip = ClipMoveEnd;
            SoundPlayer.Play();
        }

        protected virtual void OnNewAdded(RootItemStack data)
        {
            // We can easily check what kind of item it is and play unique sounds.
            // Or check item weight, and play heavier type sounds.
            // Or put specific sounds onto the Item itself and reference those directly instead of defining them here.
            // You've got lots of options with these callbacks!
            if (SoundPlayer == null || data == null || data.Source == null) return;

            if (data.Source == CoinItem) SoundPlayer.clip = ClipCoin;
            else if (data.Source.Rarity == ItemRarity.Inconcievable) SoundPlayer.clip = ClipVictory;
            else SoundPlayer.clip = ClipPickup;

            SoundPlayer.Play();
        }

        protected virtual void OnSelectedSlot(ItemUiPlug p)
        {
            if (SoundPlayer == null) return;

            SoundPlayer.clip = ClipSelected;
            SoundPlayer.Play();
        }
    }
}