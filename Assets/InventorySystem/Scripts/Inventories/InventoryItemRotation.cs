using System;

namespace InventorySystem.Inventories
{
    public enum InventoryItemRotation
    {
        DEG_0,
        DEG_90,
        DEG_180,
        DEG_270
    }

    public static class InventoryEntityRotationExtensions
    {
        public static InventoryItemRotation NextRotation(this InventoryItemRotation current)
        {
            return current switch
            {
                InventoryItemRotation.DEG_0 => InventoryItemRotation.DEG_90,
                InventoryItemRotation.DEG_90 => InventoryItemRotation.DEG_180,
                InventoryItemRotation.DEG_180 => InventoryItemRotation.DEG_270,
                InventoryItemRotation.DEG_270 => InventoryItemRotation.DEG_0,
                _ => throw new ArgumentOutOfRangeException(nameof(current), current, null)
            };
        }
    }
}