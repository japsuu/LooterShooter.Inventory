using System;
using System.Collections.Generic;
using LooterShooter.Framework.Inventories.Items;
using UnityEngine;

namespace LooterShooter.Framework.Inventories
{
    /// <summary>
    /// Axis-Aligned-Bounding-Box (AABB) for an item inside an inventory.
    /// Uses screen space coordinate dimensions (Y+ = down, X+ = right).
    /// </summary>
    public readonly struct InventoryBounds
    {
        public readonly Vector2Int Position;
        public readonly int Width;
        public readonly int Height;
        
        private readonly Vector2Int _extent;
        

        public InventoryBounds(int width, int height)
        {
            Width = width;
            Height = height;
            Position = Vector2Int.zero;
            _extent = new Vector2Int(Position.x + Width, Position.y + Height);
        }
        

        public InventoryBounds(ItemData itemData, Vector2Int position, InventoryItemRotation rotation)
        {
            int width = rotation.ShouldFlipWidthAndHeight() ? itemData.InventorySizeY : itemData.InventorySizeX;
            int height = rotation.ShouldFlipWidthAndHeight() ? itemData.InventorySizeX : itemData.InventorySizeY;
            
            Width = width;
            Height = height;
            Position = position;
            _extent = new Vector2Int(Position.x + Width, Position.y + Height);
        }


        public bool OverlapsWith(InventoryBounds other)
        {
            return Position.x < other.Position.x + other.Width &&
                   Position.x + Width > other.Position.x &&
                   Position.y < other.Position.y + other.Height &&
                   Height + Position.y > other.Position.y;
        }


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