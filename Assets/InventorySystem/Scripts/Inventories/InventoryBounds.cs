using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Inventories
{
    /// <summary>
    /// Axis-Aligned-Bounding-Box (AABB).
    /// Uses screen space coordinate dimensions (Y+ = down, X+ = right).
    /// </summary>
    public struct InventoryBounds
    {
        public Vector2Int RootPosition;
        public Vector2Int ExtentPosition;
        public readonly int Width;
        public readonly int Height;
        public InventoryItemRotation CurrentRotation;

        public static readonly InventoryItemRotation[] PossibleRotations = new[]
        {
            InventoryItemRotation.DEG_0,
            InventoryItemRotation.DEG_90,
            InventoryItemRotation.DEG_180,
            InventoryItemRotation.DEG_270,
        };
        

        public InventoryBounds(Vector2Int position, InventoryItemRotation rotation, int width, int height)
        {
            CurrentRotation = rotation;

            switch (rotation)
            {
                case InventoryItemRotation.DEG_0:
                    Width = width;
                    Height = height;
                    RootPosition = position;
                    ExtentPosition = new Vector2Int(RootPosition.x + Width, RootPosition.y + Height);
                    break;
                case InventoryItemRotation.DEG_90:
                    Width = height;
                    Height = width;
                    RootPosition = new Vector2Int(position.x - height, position.y);
                    ExtentPosition = new Vector2Int(position.x, position.y + width);
                    break;
                case InventoryItemRotation.DEG_180:
                    Width = width;
                    Height = height;
                    RootPosition = new Vector2Int(position.x + width, position.y + height);
                    ExtentPosition = position;
                    break;
                case InventoryItemRotation.DEG_270:
                    Width = height;
                    Height = width;
                    RootPosition = new Vector2Int(position.x, position.y - width);
                    ExtentPosition = new Vector2Int(position.x + height, position.y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
            }
        }


        public bool IntersectsWith(InventoryBounds other)
        {
            return RootPosition.x < other.RootPosition.x + other.Width &&
                   RootPosition.x + Width > other.RootPosition.x &&
                   RootPosition.y < other.RootPosition.y + other.Height &&
                   Height + RootPosition.y > other.RootPosition.y;
        }


        public bool IsContainedIn(InventoryBounds other)
        {
            // If top left point outside.
            if (RootPosition.x < other.RootPosition.x || RootPosition.y < other.RootPosition.y)
                return false;

            // If bottom right point outside.
            if (ExtentPosition.x > other.ExtentPosition.x || ExtentPosition.y > other.ExtentPosition.y)
                return false;

            return true;
        }


        public bool Contains(InventoryBounds other)
        {
            return Contains(other.RootPosition) && Contains(other.ExtentPosition);
        }


        public bool Contains(Vector2Int position)
        {
            return
                position.x >= RootPosition.x &&
                position.y >= RootPosition.y &&
                position.x <= ExtentPosition.x &&
                position.y <= ExtentPosition.y;
        }


        public IEnumerable<Vector2Int> AllPositionsWithin()
        {
            for (int y = RootPosition.y; y < RootPosition.y + Height; y++)
            {
                for (int x = RootPosition.x; x < RootPosition.x + Width; x++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }


        public override string ToString()
        {
            return $"InventoryBounds RP:{RootPosition}, EP:{ExtentPosition}, W:{Width}, H:{Height}";
        }
    }
}