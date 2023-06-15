using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Inventories
{
    /// <summary>
    /// Axis-Aligned-Bounding-Box (AABB).
    /// Uses screen space coordinate dimensions (Y+ = down, X+ = right).
    /// </summary>
    public readonly struct InventoryBounds
    {
        public readonly Vector2Int Position;
        public readonly int Width;
        public readonly int Height;
        
        private readonly Vector2Int _extent;
        

        public InventoryBounds(Vector2Int position, int width, int height)
        {
            Width = width;
            Height = height;
            Position = position;
            _extent = new Vector2Int(Position.x + Width, Position.y + Height);

            /*switch (rotation)
            {
                case ItemRotation.DEG_0:
                    Width = width;
                    Height = height;
                    Position = position;
                    _extent = new Vector2Int(Position.x + Width, Position.y + Height);
                    break;
                case ItemRotation.DEG_90:
                    Width = height;
                    Height = width;
                    Position = new Vector2Int(position.x - height, position.y);
                    _extent = new Vector2Int(position.x, position.y + width);
                    break;
                case ItemRotation.DEG_180:
                    Width = width;
                    Height = height;
                    Position = new Vector2Int(position.x + width, position.y + height);
                    _extent = position;
                    break;
                case ItemRotation.DEG_270:
                    Width = height;
                    Height = width;
                    Position = new Vector2Int(position.x, position.y - width);
                    _extent = new Vector2Int(position.x + height, position.y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
            }*/
        }


        public bool OverlapsWith(InventoryBounds other)
        {
            return Position.x < other.Position.x + other.Width &&
                   Position.x + Width > other.Position.x &&
                   Position.y < other.Position.y + other.Height &&
                   Height + Position.y > other.Position.y;
        }


        /*public bool IsContainedIn(InventoryBounds other)
        {
            // If top left point outside.
            if (RootPosition.x < other.RootPosition.x || RootPosition.y < other.RootPosition.y)
                return false;

            // If bottom right point outside.
            if (ExtentPosition.x > other.ExtentPosition.x || ExtentPosition.y > other.ExtentPosition.y) 
                return false;

            return true;
        }*/


        public bool Contains(InventoryBounds other)
        {
            return Contains(other.Position) && Contains(other._extent - Vector2Int.one);
        }


        public bool Contains(Vector2Int position)
        {
            return
                position.x >= Position.x &&
                position.y >= Position.y &&
                position.x < _extent.x &&
                position.y < _extent.y;
        }


        public IEnumerable<Vector2Int> AllPositionsWithin()
        {
            for (int y = Position.y; y < Position.y + Height; y++)
            {
                for (int x = Position.x; x < Position.x + Width; x++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }


        public override string ToString()
        {
            return $"Pos:{Position}, W:{Width}, H:{Height}, Ext:{_extent}";
        }


        public bool Equals(InventoryBounds other)
        {
            return Position.Equals(other.Position) && Width == other.Width && Height == other.Height;
        }


        public override bool Equals(object obj)
        {
            return obj is InventoryBounds other && Equals(other);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Width, Height);
        }


        public static bool operator ==(InventoryBounds left, InventoryBounds right)
        {
            return left.Equals(right);
        }


        public static bool operator !=(InventoryBounds left, InventoryBounds right)
        {
            return !left.Equals(right);
        }
    }
}