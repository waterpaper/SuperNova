using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    [RequireComponent(typeof(Button))]
    public class ControlSmithItem : MonoBehaviour
    {
        public const string RES_PATH = "Assets/res/prefab/UI/ControlItem.prefab";

        [SerializeField]
        private RawImage itemIcon;
        [SerializeField]
        private Text textName, textDescription;
        [SerializeField]
        private Button buttonEnchant;

        public ReactiveProperty<ItemInstance> ItemInstance { get; private set; } = new ReactiveProperty<ItemInstance>(null);
        public IObservable<ItemInstance> OnClickAsObservable() => buttonEnchant.OnClickAsObservable().Select(_ => this.ItemInstance.Value);

        private void Awake()
        {
            ItemInstance
                .Where(p => p != null)
                .Subscribe(item =>
                {
                    var itemInfo = Root.GameInfo.ItemInfos[item.ItemID];

                    itemIcon.gameObject.SetActive(true);
                    textName.text = $"{itemInfo.Name}{(item.Enchant == 0 ? "" : $" (+{item.Enchant})")}";
                    textDescription.text = $"{Root.State.Logic.GeneralCalculator.EnchantPriceGold(item)} 골드 · {Root.State.Logic.GeneralCalculator.EnchantPriceSoul(item)} 소울 · {string.Format("{0:0.00%}", Root.State.Logic.GeneralCalculator.EnchantProbability(item))}";

                    Root.World.StartCoroutine(Res.LoadAssetAsCoroutineThen<Texture2D>(Constants.GetItemIconResPath(item.ItemID), texture =>
                    {
                        itemIcon.texture = texture;
                    }));
                })
                .AddTo(this);

            ItemInstance
                .Where(p => p == null)
                .Subscribe(_ =>
                {
                    itemIcon.gameObject.SetActive(false);
                    textName.text = string.Empty;
                    textDescription.text = string.Empty;
                })
                .AddTo(this);

            this.GetComponent<Button>()
                .OnClickAsObservable()
                .Select(_ => ItemInstance.Value)
                .Where(item => item != null)
                .Subscribe(item =>
                {
                    Root.PopupManager.ShowPopup(new PopupItemInfoArgs(item));
                })
                .AddTo(this);
        }
    }
}
