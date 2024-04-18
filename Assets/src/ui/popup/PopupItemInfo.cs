using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Supernova.Utils;

namespace Supernova.Unity.UI
{
    public class PopupItemInfo : UIPopup<PopupItemInfoArgs>
    {
        [SerializeField]
        private RawImage imageIcon;
        [SerializeField]
        private Text textName, textEnchant;
        [SerializeField]
        private Text textDescription;
        [SerializeField]
        private Button[] buttonSkillIcons;
        [SerializeField]
        private Button buttonSell, buttonEnchant, buttonEquip, buttonClose;

        private readonly ReactiveProperty<ItemInstance> targetItem = new ReactiveProperty<ItemInstance>();

        public override void Init(PopupItemInfoArgs args)
        {
            this.targetItem.Value = args.ItemInstance;

            this.targetItem
                .Where(p => p != null)
                .Subscribe(itemInstance =>
                {
                    if (Root.State.Inventory.Items.Contains(itemInstance))
                    {
                        buttonSell.gameObject.SetActive(true);
                        buttonEquip.gameObject.SetActive(
                            Root.State.Inventory.Weapon.Value != itemInstance.Guid &&
                            Root.State.Inventory.Armor.Value != itemInstance.Guid &&
                            Root.State.Inventory.AccessoryLeft.Value != itemInstance.Guid &&
                            Root.State.Inventory.AccessoryRight.Value != itemInstance.Guid
                            );
                        buttonEnchant.gameObject.SetActive(true);

                    }
                    else
                    {
                        buttonSell.gameObject.SetActive(true);
                        buttonEquip.gameObject.SetActive(false);
                        buttonEnchant.gameObject.SetActive(false);
                    }

                    buttonClose.OnClickAsObservable().Subscribe(_ => this.Close()).AddTo(this);

                    StartCoroutine(this.InitAsync(itemInstance));
                })
                .AddTo(this);

            buttonSell
                .OnClickAsObservable()
                .Subscribe(p =>
                {
                    this.Sell();
                })
                .AddTo(this);

            buttonEnchant
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    this.Enchant();
                })
                .AddTo(this);

            buttonEquip
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    StartCoroutine(this.Equip());
                })
                .AddTo(this);
        }

        private IEnumerator InitAsync(ItemInstance itemInstance)
        {
            var itemInfo = Root.GameInfo.ItemInfos[itemInstance.ItemID];

            this.textName.text = itemInfo.Name;
            this.textDescription.text = (itemInfo.Stat + (itemInfo.UpgradeStat * itemInstance.Enchant)).ToString().Replace(";", "\n").Trim();
            switch (itemInfo.ItemType)
            {
            case ItemType.Weapon: this.textEnchant.text = itemInstance.Enchant == 0 ? "무기" : $"+{itemInstance.Enchant}강 무기"; break;
            case ItemType.Armor: this.textEnchant.text = itemInstance.Enchant == 0 ? "방어구" : $"+{itemInstance.Enchant}강 방어구"; break;
            case ItemType.Accessory: this.textEnchant.text = itemInstance.Enchant == 0 ? "액세서리" : $"+{itemInstance.Enchant}강 액세서리"; break;
            }

            yield return Res.LoadAssetAsCoroutineThen<Texture2D>(Constants.GetItemIconResPath(itemInstance.ItemID), texture =>
            {
                this.imageIcon.texture = texture;
            });
        }

        private void Sell()
        {
            var itemInstance = this.targetItem.Value;
            var price = Root.GameInfo.ItemInfos[itemInstance.ItemID].Price;

            Root.PopupManager.ShowPopup(new PopupYesNoArgs($"선택한 아이템을 판매하시겠습니까?", isYes =>
            {
                if (!isYes) { return; }

                Root.State.Inventory.Items.Remove(itemInstance);
                Root.State.Currency.Gold.Value += price;

                Root.PopupManager.ShowPopup(new PopupAlertArgs($"아이템을 판매하여 {price}골드를 얻었습니다."));
                this.Close();
            }));
        }

        private void Enchant()
        {
            var itemInstance = targetItem.Value;
            var (pw, pa, pal, par) = (Root.State.Inventory.Weapon.Value, Root.State.Inventory.Armor.Value, Root.State.Inventory.AccessoryLeft.Value, Root.State.Inventory.AccessoryRight.Value);
            var priceGold = Root.State.Logic.GeneralCalculator.EnchantPriceGold(itemInstance);
            var priceSoul = Root.State.Logic.GeneralCalculator.EnchantPriceSoul(itemInstance);
            if (Root.State.Currency.Gold.Value < priceGold || Root.State.Currency.Soul.Value < priceSoul)
            {
                Root.PopupManager.ShowPopup(new PopupAlertArgs("강화에 필요한 재화가 부족합니다."));
                return;
            }

            Root.State.Currency.Gold.Value -= priceGold;
            Root.State.Currency.Soul.Value -= priceSoul;

            var probabilty = Root.State.Logic.GeneralCalculator.EnchantProbability(itemInstance);
            var rand = UnityEngine.Random.value;
            if (rand > probabilty)
            {
                Root.PopupManager.ShowPopup(new PopupAlertArgs("강화에 실패했습니다."));
                return;
            }

            var newItem = ItemInstance.Upgrade(itemInstance);
            Root.State.Inventory.Items.Remove(itemInstance);
            Root.State.Inventory.Items.Add(newItem);

            Log.Info($"{Root.State.Inventory.Weapon.Value} : {itemInstance.Guid}");
            if (pw == itemInstance.Guid)
            {
                Root.State.Inventory.Weapon.Value = newItem.Guid;
            }
            else if (pa == itemInstance.Guid)
            {
                Root.State.Inventory.Armor.Value = newItem.Guid;
            }
            else if (pal == itemInstance.Guid)
            {
                Root.State.Inventory.AccessoryLeft.Value = newItem.Guid;
            }
            else if (par == itemInstance.Guid)
            {
                Root.State.Inventory.AccessoryRight.Value = newItem.Guid;
            }

            this.targetItem.Value = newItem;
        }

        private IEnumerator Equip()
        {
            var itemInstance = this.targetItem.Value;
            var info = Root.GameInfo.ItemInfos[itemInstance.ItemID];

            switch (info.ItemType)
            {
            case ItemType.Weapon:
                Root.State.Inventory.Weapon.Value = itemInstance.Guid;
                break;
            case ItemType.Armor:
                Root.State.Inventory.Armor.Value = itemInstance.Guid;
                break;
            case ItemType.Accessory:
                yield return Root.PopupManager.ShowPopupAsCoroutine(new PopupAccessorySlotArgs(result =>
                {
                    switch (result)
                    {
                    case PopupAccessorySlotResult.Left:
                        Root.State.Inventory.AccessoryLeft.Value = itemInstance.Guid;
                        break;
                    case PopupAccessorySlotResult.Right:
                        Root.State.Inventory.AccessoryRight.Value = itemInstance.Guid;
                        break;
                    }
                }));
                break;
            }

            Root.PopupManager.ShowPopup(new PopupAlertArgs("아이템을 장착했습니다."));
            this.Close();
        }
    }

    public class PopupItemInfoArgs : IPopupArgs
    {
        public ItemInstance ItemInstance { get; }

        public string PrefabName => "ItemInfo";

        public PopupItemInfoArgs(ItemInstance itemInstance)
        {
            this.ItemInstance = itemInstance;
        }
    }
}