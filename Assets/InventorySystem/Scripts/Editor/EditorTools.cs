using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Scripts.Editor
{
    public class EditorTools
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Looter Shooter/Open Save Folder")]
        private static void OpenSaveFolder()
        {
            Process.Start(Application.persistentDataPath);
        }
#endif
    }
}