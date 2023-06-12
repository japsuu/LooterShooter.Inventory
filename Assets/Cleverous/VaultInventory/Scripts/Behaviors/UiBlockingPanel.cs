// (c) Copyright Cleverous 2023. All rights reserved.

using Cleverous.VaultInventory.Scripts.General;
using UnityEngine;

namespace Cleverous.VaultInventory.Scripts.Behaviors
{
    /// <summary>
    /// A class for flagging that a panel is considered "blocking" of something - for instance Player Movement. Can be easily checked via a bool in the <see cref="VaultInventory"/> class.
    /// </summary>
    public abstract class UiBlockingPanel : MonoBehaviour
    {
        protected bool IsBlocking;

        protected virtual void Awake()
        {
            SetIsBlocking(gameObject.activeSelf);
            General.VaultInventory.RegisterBlockingPanel(this);
        }
        protected virtual void OnEnable()
        {
            SetIsBlocking(true);
        }
        protected virtual void OnDestroy()
        {
            SetIsBlocking(false);
            General.VaultInventory.DeregisterBlockingPanel(this);
        }
        protected virtual void OnDisable()
        {
            SetIsBlocking(false);
        }

        public virtual void OpenUi()
        {
            SetIsBlocking(true);
        }
        public virtual void CloseUi()
        {
            SetIsBlocking(false);
        }

        public virtual void SetIsBlocking(bool state)
        {
            IsBlocking = state;
        }
        public virtual bool GetIsBlocking()
        {
            return IsBlocking;
        }
    }
}