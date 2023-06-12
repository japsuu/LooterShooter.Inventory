// (c) Copyright Cleverous 2023. All rights reserved.

using UnityEngine;

namespace Cleverous.VaultInventory.Scripts.Interfaces
{
    /// <summary>
    /// An interface to interact with a <see cref="DataEntity"/> that can be "used".
    /// </summary>
    public interface IUseableDataEntity
    {
        int GetDbKey();
        string Title { get; set; }
        string Description { get; set; }
        Sprite UiIcon { get; set; }
        float UseCooldownTime { get; set; }
        void UseBegin(IUseInventory user);
        void UseFinish(IUseInventory user);
        void UseCancel(IUseInventory user);
    }
}