using System.IO;
using LooterShooter.Ui.InventoryRenderering.Slot;
using UnityEngine;

namespace LooterShooter.Framework.Saving
{
    [DefaultExecutionOrder(-1001)]  //TODO: Convert to IInitializable.
    public class SaveSystem : SingletonBehaviour<SaveSystem>
    {
        private const string PLAYER_SAVE_FILE_NAME = "player_save_data.json";

        private PlayerDataSaver _playerDataSaver;


        public PlayerSaveData GetLocalPlayerSaveData() => _playerDataSaver.GetLocalPlayerData();


        private void Awake()
        {
            _playerDataSaver = new(Path.Combine(Application.persistentDataPath, PLAYER_SAVE_FILE_NAME));
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