using System;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem
{
    public class PrefabReferences : MonoBehaviour
    {
        public static PrefabReferences Singleton;
        
        [SerializeField] private DraggableItem _draggableItemPrefab;

        public DraggableItem DraggableItemPrefab => _draggableItemPrefab;


        private void Awake()
        {
            if (Singleton != null)
                throw new Exception($"Multiple {nameof(PrefabReferences)} in scene!");
            
            Singleton = this;
        }
    }
}