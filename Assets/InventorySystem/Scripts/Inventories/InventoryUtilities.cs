using UnityEngine;

namespace InventorySystem.Inventories
{
    public static class InventoryUtilities
    {
        public const float INVENTORY_SLOT_SIZE = 100f;
        
        
        public static Vector2Int GetInventoryGridPosition(Vector2 position)
        {
            int x = Mathf.RoundToInt(position.x / INVENTORY_SLOT_SIZE);
            int y = -Mathf.RoundToInt(position.y / INVENTORY_SLOT_SIZE);
            return new Vector2Int(x, y);
        }


        public static Vector2 SnapPositionToInventoryGrid(Vector2 position)
        {
            float x = Mathf.Round(position.x / INVENTORY_SLOT_SIZE) * INVENTORY_SLOT_SIZE;
            float y = Mathf.Round(position.y / INVENTORY_SLOT_SIZE) * INVENTORY_SLOT_SIZE;
            return new Vector2(x, y);
        }
    }
}