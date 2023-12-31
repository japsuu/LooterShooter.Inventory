﻿using System;
using JetBrains.Annotations;
using LooterShooter.Framework.Clothing;
using LooterShooter.Framework.Inventories;
using Newtonsoft.Json;

namespace LooterShooter.Framework.Saving
{
    /// <summary>
    /// Save-data of a player.
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        [NotNull]
        [JsonRequired]
        [JsonProperty("identifier")]
        public string Identifier { get; private set; }
        
        [CanBeNull]
        [JsonProperty("baseInventory")]
        public SpatialInventory BaseInventory { get; private set; }
        
        [CanBeNull]
        [JsonProperty("equippedClothes")]
        public ClothingInventory[] EquippedClothes { get; private set; }
        
        [CanBeNull]
        [JsonProperty("equipmentSlots")]
        public EquipmentSlotSaveData[] SavedEquipmentSlots { get; private set; }


        public PlayerSaveData(string identifier, SpatialInventory baseInventory, ClothingInventory[] equippedClothes, EquipmentSlotSaveData[] savedEquipmentSlots)
        {
            Identifier = identifier;
            BaseInventory = baseInventory;
            EquippedClothes = equippedClothes;
            SavedEquipmentSlots = savedEquipmentSlots;
        }
    }
}