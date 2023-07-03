using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem.Inventories
{
    public static class Utilities
    {
        public const float INVENTORY_SLOT_SIZE = 50f;
        
        
        public static Vector2Int GetInventoryGridPosition(Vector2 position) //BUG: RelativeToRect(rect)
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
        
        
        public static Vector2 GetAnchoredPositionRelativeToRect(Vector3 worldPoint, RectTransform relativeTo)
        {
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, worldPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(relativeTo, screenP, null, out Vector2 relativePoint);

            return relativePoint;
        }


        public static T GetFirstComponentBelow<T>(Vector2 screenSpacePosition) where T : MonoBehaviour
        {
            PointerEventData pointerData = new(EventSystem.current)
            {
                pointerId = -1,
                position = screenSpacePosition
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                T component = result.gameObject.GetComponent<T>();
                if (component != null)
                    return component;
            }
		
            return default;
        }
    }
}