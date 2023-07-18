using System;
using InventorySystem.InventorySlots;
using UnityEngine;

namespace InventorySystem.Saving
{
    /// <summary>
    /// WARN: Should be initializable!
    /// </summary>
    [DefaultExecutionOrder(-1001)]
    public class SaveSystem : MonoBehaviour
    {
        private const string INVENTORY_SAVE_FILE_NAME = "player_save_data.txt";

        private PlayerDataSaver _playerDataSaver;
        
        public static SaveSystem Singleton;


        public PlayerSaveData GetLocalPlayerSaveData() => _playerDataSaver.GetLocalPlayerData();


        private void Awake()
        {
            if (Singleton != null)
            {
                Logger.Out(LogLevel.ERROR, $"Multiple {nameof(SaveSystem)} found in scene!");
                return;
            }
            
            Singleton = this;

            _playerDataSaver = new(INVENTORY_SAVE_FILE_NAME);
        }


        private void OnEnable()
        {
            EquipmentSlot.ContentsChanged += _playerDataSaver.RegisterEquipmentSlotForSaving;
        }


        private void OnDisable()
        {
            EquipmentSlot.ContentsChanged -= _playerDataSaver.RegisterEquipmentSlotForSaving;
        }


        private void OnApplicationQuit()
        {
            _playerDataSaver.SaveLocalPlayerData();
        }
    }
}