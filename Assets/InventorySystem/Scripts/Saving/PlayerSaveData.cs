using System;
using InventorySystem.Clothing;
using InventorySystem.Inventories;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace InventorySystem.Saving
{
    [Serializable]
    public class PlayerSaveData
    {
        [NotNull]
        [JsonRequired]
        public string Identifier { get; private set; }
        
        [CanBeNull]
        public SpatialInventory BaseInventory { get; private set; }
        
        [CanBeNull]
        public ClothingInventory[] EquippedClothes { get; private set; }


        public PlayerSaveData(string identifier, SpatialInventory baseInventory, ClothingInventory[] equippedClothes)
        {
            Identifier = identifier;
            BaseInventory = baseInventory;
            EquippedClothes = equippedClothes;
        }
    }
}