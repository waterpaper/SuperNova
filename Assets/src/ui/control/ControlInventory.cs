using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Supernova.Utils;

namespace Supernova.Unity.UI
{
    public class ControlInventory : MonoBehaviour
    {
        [SerializeField]
        private RectTransform itemParent;

        private Dictionary<string, ControlItem> items = new Dictionary<string, ControlItem>();

        private void Awake()
        {
            Root.State.Inventory.Items.ObserveAdd().Subscribe(p => Root.World.StartCoroutine(this.Add(p.Value))).AddTo(this);
            Root.State.Inventory.Items.ObserveRemove().Subscribe(p => this.Remove(p.Value)).AddTo(this);
        }

        private void OnEnable()
        {
            StartCoroutine(this.RefreshAsCoroutine(Root.State.Inventory.Items));
        }

        private IEnumerator RefreshAsCoroutine(IEnumerable<ItemInstance> items)
        {
            itemParent.gameObject.SetActive(false);
            foreach (var item in itemParent.GetComponentsInChildren<ControlItem>())
            {
                item.gameObject.Release();
            }
            this.items.Clear();

            foreach (var item in items)
            {
                yield return this.Add(item);
            }

            itemParent.gameObject.SetActive(true);
            Log.Info("Inventory Refreshed.");
        }

        private IEnumerator Add(ItemInstance item)
        {
            yield return Res.InstantiateAssetAsCoroutineThen<ControlItem>(ControlItem.RES_PATH, this.itemParent, clone =>
            {
                clone.ItemInstance.Value = item;
                this.items.Add(item.Guid, clone);
            });
        }

        private void Remove(ItemInstance item)
        {
            if (this.items.TryGetValue(item.Guid, out var controlItem))
            {
                controlItem.gameObject.Release();
                this.items.Remove(item.Guid);
            }
        }
    }
}
