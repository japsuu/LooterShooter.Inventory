using System;

namespace InventorySystem.Inventories
{
    public enum ItemRotation
    {
        DEG_0,
        DEG_90,
        DEG_180,
        DEG_270
    }

    public static class InventoryEntityRotationExtensions
    {
        public static ItemRotation NextRotation(this ItemRotation current)
        {
            return current switch
            {
                ItemRotation.DEG_0 => ItemRotation.DEG_90,
                ItemRotation.DEG_90 => ItemRotation.DEG_180,
                ItemRotation.DEG_180 => ItemRotation.DEG_270,
                ItemRotation.DEG_270 => ItemRotation.DEG_0,
                _ => throw new ArgumentOutOfRangeException(nameof(current), current, null)
            };
        }
        
        public static float AsDegrees(this ItemRotation current)
        {
            return current switch
            {
                ItemRotation.DEG_0 => 0f,
                ItemRotation.DEG_90 => -90f,
                ItemRotation.DEG_180 => -180f,
                ItemRotation.DEG_270 => -270f,
                _ => throw new ArgumentOutOfRangeException(nameof(current), current, null)
            };
        }
    }
}