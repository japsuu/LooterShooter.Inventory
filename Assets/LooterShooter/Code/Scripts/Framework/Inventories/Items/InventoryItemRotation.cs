using System;

namespace LooterShooter.Framework.Inventories.Items
{
    public enum InventoryItemRotation
    {
        DEG_0,
        DEG_90,
    }

    public static class InventoryItemRotationExtensions
    {
        public static InventoryItemRotation NextRotation(this InventoryItemRotation current)
        {
            return current switch
            {
                InventoryItemRotation.DEG_0 => InventoryItemRotation.DEG_90,
                InventoryItemRotation.DEG_90 => InventoryItemRotation.DEG_0,
                _ => throw new ArgumentOutOfRangeException(nameof(current), current, null)
            };
        }
        
        public static float AsDegrees(this InventoryItemRotation current)
        {
            return current switch
            {
                InventoryItemRotation.DEG_0 => 0f,
                InventoryItemRotation.DEG_90 => -90f,
                _ => throw new ArgumentOutOfRangeException(nameof(current), current, null)
            };
        }
        
        public static bool ShouldFlipWidthAndHeight(this InventoryItemRotation current)
        {
            return current switch
            {
                InventoryItemRotation.DEG_0 => false,
                InventoryItemRotation.DEG_90 => true,
                _ => throw new ArgumentOutOfRangeException(nameof(current), current, null)
            };
        }
    }
}