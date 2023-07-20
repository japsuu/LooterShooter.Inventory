using LooterShooter.Framework.Inventories.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace LooterShooter.Framework.Inventories.Items
{
    /// <summary>
    /// Represents an <see cref="ItemMetadata"/> instance contained in an <see cref="IInventory"/>.
    /// </summary>
    [JsonConverter(typeof(InventoryItemConverter))]
    public class InventoryItem
    {
        public IInventory ContainingInventory;
        public readonly ItemMetadata Metadata;
        public readonly InventoryBounds Bounds;
        public readonly InventoryItemRotation RotationInInventory;


        public InventoryItem(ItemMetadata itemMetadata, InventoryBounds bounds, InventoryItemRotation rotationInInventory, IInventory containingInventory)
        {
            Metadata = itemMetadata;
            Bounds = bounds;
            RotationInInventory = rotationInInventory;
            ContainingInventory = containingInventory;
        }


        public void OverwriteContainingInventory(IInventory containingInventory)
        {
            ContainingInventory = containingInventory;
        }


        public void RequestMove(IInventory newInventory, Vector2Int newPosition, InventoryItemRotation newRotation)
        {
            Logger.Write(LogLevel.DEBUG, $"{nameof(InventoryItem)}: {Metadata.ItemData.ItemName}", $"RequestMove: {Bounds.Position} -> {newPosition}, {RotationInInventory} -> {newRotation}");
            if (!IsMoveValid(newInventory, newPosition, newRotation))
                return;

            // Check that new inventory can create a new InventoryItem for the moved item.
            if (!newInventory.TryCreateNewInventoryItem(Metadata, newPosition, newRotation, Bounds, out InventoryItem newItem))
            {
                Logger.Write(LogLevel.DEBUG, $"{nameof(InventoryItem)}: {Metadata.ItemData.ItemName}", $"newInventory '{newInventory.Name}' can't create new {nameof(InventoryItem)} @ {newPosition}.");
                return;
            }

            ContainingInventory.RemoveItem(Bounds.Position);
            newInventory.AddItem(newItem);
        }


        private bool IsMoveValid(IInventory newInventory, Vector2Int newPosition, InventoryItemRotation newRotation)
        {
            // Check that new inventory is valid.
            if (newInventory == null)
            {
                Logger.Write(LogLevel.WARN, $"{nameof(InventoryItem)}: {Metadata.ItemData.ItemName}", "newInventory is null!");
                return false;
            }

            // Check that either movement or rotation or inventory change has happened.
            bool positionChanged = Bounds.Position != newPosition;
            bool rotationChanged = RotationInInventory != newRotation;
            bool inventoryChanged = ContainingInventory != newInventory;

            if (positionChanged || rotationChanged || inventoryChanged)
                return true;
            
            Logger.Write(
                LogLevel.DEBUG,
                $"{nameof(InventoryItem)}: {Metadata.ItemData.ItemName}",
                "movement is not valid because position, rotation or inventory did NOT change.");
            
            return false;
        }
    }
}