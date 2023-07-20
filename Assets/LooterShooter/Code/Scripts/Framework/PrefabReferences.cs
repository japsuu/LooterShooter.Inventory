using LooterShooter.Ui.InventoryRenderering;
using UnityEngine;

namespace LooterShooter.Framework
{
    public class PrefabReferences : SingletonBehaviour<PrefabReferences>
    {
        [SerializeField] private DraggableItem _draggableItemPrefab;

        public DraggableItem DraggableItemPrefab => _draggableItemPrefab;
    }
}