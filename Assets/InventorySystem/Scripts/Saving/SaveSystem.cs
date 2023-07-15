using UnityEngine;

namespace InventorySystem.Saving
{
    [DefaultExecutionOrder(-1000)]
    public class SaveSystem : MonoBehaviour
    {
        private const string INVENTORY_SAVE_FILE_NAME = "inventory_snapshots.txt";

        //private InventorySaver _inventorySaver;
        
        public static SaveSystem Singleton;


        private void Awake()
        {
            if (Singleton != null)
            {
                Logger.Log(LogLevel.ERROR, $"Multiple {nameof(SaveSystem)} found in scene!");
                return;
            }
            
            Singleton = this;

            //_inventorySaver = new InventorySaver(INVENTORY_SAVE_FILE_NAME);
        }


        private void OnApplicationQuit()
        {
            SavePlayerData();
        }


        private void SavePlayerData()
        {
            
        }
    }
}