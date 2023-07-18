using System.Collections.Generic;
using System.Linq;
using InventorySystem.Clothing;
using InventorySystem.Inventories;
using InventorySystem.Inventories.Serialization;
using InventorySystem.InventorySlots;

namespace InventorySystem.Saving
{
    /// <summary>
    /// NOTE: Ideally, we would save a list of PlayerSaveData to memory, when the server stops.
    /// Then we would have a method in PlayerDataSaver to asynchronously query for the SaveData of a specific player,
    /// and apply it before the said player is allowed to join the game.
    ///
    /// We should also have methods for saving all data for every player online, and saving data for an individual player (when they leave).
    /// And ofc the method to load individual data described above.
    /// </summary>
    public class PlayerDataSaver : JsonSaver<PlayerSaveData>
    {
        private readonly PlayerSaveData _playerSaveData;
        private readonly HashSet<EquipmentSlot> _savedEquipmentSlots;


        public PlayerSaveData GetLocalPlayerData() => _playerSaveData;
        
        
        public PlayerDataSaver(string saveFileName) : base(saveFileName)
        {
            _savedEquipmentSlots = new();
            _playerSaveData = LoadData();
        }


        public void SaveLocalPlayerData()
        {
            PlayerInventoryManager inventoryManager = PlayerInventoryManager.Singleton;
            
            const string playerId = "PLAYER_ID_CHANGE_ME";
            SpatialInventory playerBaseInventory = inventoryManager.BaseInventory;
            ClothingInventory[] playerClothes = inventoryManager.ClothingInventories.Values.ToArray();
            EquipmentSlotSaveData[] equipmentSlotSaveData = (from slot in _savedEquipmentSlots.ToArray() where slot.AssignedItemMetadata != null select new EquipmentSlotSaveData(slot.Name, slot.AssignedItemMetadata)).ToArray();
            
            PlayerSaveData saveData = new(playerId, playerBaseInventory, playerClothes, equipmentSlotSaveData);
            
            SaveData(saveData, new SpatialInventoryConverter());
        }


        public void RegisterEquipmentSlotForSaving(EquipmentSlot slot)
        {
            if(_savedEquipmentSlots.Contains(slot))
            {
                if (slot.AssignedItemMetadata == null)
                    _savedEquipmentSlots.Remove(slot);
                
                return;
            }
            
            _savedEquipmentSlots.Add(slot);
        }
    }
}