// (c) Copyright Cleverous 2023. All rights reserved.

using System.Collections;
using Cleverous.VaultInventory.Scripts.General;
using Cleverous.VaultInventory.Scripts.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cleverous.VaultInventory.Scripts.Behaviors
{
    /// <summary>
    /// Used for showing 'popup' alerts in the game UI when the inventory changes state - such as when adding or removing items.
    /// </summary>
    public class UiPickupAlert : MonoBehaviour
    {
        protected IUseInventory TargetAgent;
        public AnimationCurve Curve;

        public void Start()
        {
            General.VaultInventory.OnPlayerSpawn += PlayerSpawn;
        }

        protected virtual void PlayerSpawn(IUseInventory agent)
        {
            TargetAgent = agent;
            TargetAgent.Inventory.OnItemAdded += OnInventoryAdd;
            TargetAgent.Inventory.OnItemRemoved += OnInventoryRemove;
        }

        protected virtual void OnInventoryAdd(RootItemStack data)
        {
            if (data == null) return;
            StartCoroutine(DoAlert(data, false));
        }
        protected virtual void OnInventoryRemove(RootItemStack data)
        {
            if (data == null) return;
            StartCoroutine(DoAlert(data, true));
        }

        protected virtual IEnumerator DoAlert(RootItemStack data, bool isRemoval)
        {
            GameObject go = Instantiate(General.VaultInventory.ItemFloaterTemplate, Vector3.zero, Quaternion.identity, transform);
            Image img = go.GetComponent<Image>();
            TMP_Text txt = go.GetComponentInChildren<TMP_Text>();

            if (txt != null) txt.text = data.StackSize.ToString();
            if (img == null) Debug.LogError("There is no Image on the prefab for pickup alerts.");
            else img.sprite = data.Source.UiIcon;

            float totalTime = 1;
            float timer = 0;
            while (timer < totalTime)
            {
                if (img != null)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Clamp(totalTime - timer, 0, 1));
                }

                Vector3 dir = isRemoval ? Vector3.up : Vector3.down;
                go.transform.position = transform.position + dir * Curve.Evaluate(timer) * 50;

                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(go);
        }

        protected virtual void OnDestroy()
        {
            if (TargetAgent != null && TargetAgent.Inventory != null)
            {
                TargetAgent.Inventory.OnItemAdded -= OnInventoryAdd;
                TargetAgent.Inventory.OnItemRemoved -= OnInventoryRemove;
            }
        }
    }
}