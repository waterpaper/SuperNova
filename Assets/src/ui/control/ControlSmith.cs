using UniRx;
using UnityEngine;
using System.Linq;

namespace Supernova.Unity.UI
{
    public class ControlSmith : MonoBehaviour
    {
        [SerializeField]
        private ControlSmithItem weapon, armor;

        private void Awake()
        {
            Root.State.Inventory.Weapon
                .Select(p => Root.State.Inventory.Items.FirstOrDefault(q => q.Guid == p))
                .Subscribe(item =>
                {
                    weapon.ItemInstance.Value = item;
                    weapon.gameObject.SetActive(item != null);
                })
                .AddTo(this);

            Root.State.Inventory.Armor
                .Select(p => Root.State.Inventory.Items.FirstOrDefault(q => q.Guid == p))
                .Subscribe(item =>
                {
                    armor.ItemInstance.Value = item;
                    armor.gameObject.SetActive(item != null);
                })
                .AddTo(this);

            weapon.OnClickAsObservable()
                .Subscribe(item =>
                {
                    if (item == null) { return; }

                    var priceGold = Root.State.Logic.GeneralCalculator.EnchantPriceGold(item);
                    var priceSoul = Root.State.Logic.GeneralCalculator.EnchantPriceSoul(item);
                    if (Root.State.Currency.Gold.Value < priceGold || Root.State.Currency.Soul.Value < priceSoul)
                    {
                        Root.PopupManager.ShowPopup(new PopupAlertArgs("강화에 필요한 재화가 부족합니다."));
                        return;
                    }

                    Root.State.Currency.Gold.Value -= priceGold;
                    Root.State.Currency.Soul.Value -= priceSoul;

                    var probabilty = Root.State.Logic.GeneralCalculator.EnchantProbability(item);
                    var rand = Random.value;
                    if (rand > probabilty)
                    {
                        Root.PopupManager.ShowPopup(new PopupAlertArgs("강화에 실패했습니다."));
                        return;
                    }

                    var newItem = ItemInstance.Upgrade(item);
                    Root.State.Inventory.Items.Remove(item);
                    Root.State.Inventory.Items.Add(newItem);
                    Root.State.Inventory.Weapon.Value = newItem.Guid;
                })
                .AddTo(this);

            armor.OnClickAsObservable()
                .Subscribe(item =>
                {
                    if (item == null) { return; }

                    var priceGold = Root.State.Logic.GeneralCalculator.EnchantPriceGold(item);
                    var priceSoul = Root.State.Logic.GeneralCalculator.EnchantPriceSoul(item);
                    if (Root.State.Currency.Gold.Value < priceGold || Root.State.Currency.Soul.Value < priceSoul)
                    {
                        Root.PopupManager.ShowPopup(new PopupAlertArgs("강화에 필요한 재화가 부족합니다."));
                        return;
                    }

                    Root.State.Currency.Gold.Value -= priceGold;
                    Root.State.Currency.Soul.Value -= priceSoul;

                    var probabilty = Root.State.Logic.GeneralCalculator.EnchantProbability(item);
                    var rand = Random.value;
                    if (rand > probabilty)
                    {
                        Root.PopupManager.ShowPopup(new PopupAlertArgs("강화에 실패했습니다."));
                        return;
                    }

                    var newItem = ItemInstance.Upgrade(item);
                    Root.State.Inventory.Items.Remove(item);
                    Root.State.Inventory.Items.Add(newItem);
                    Root.State.Inventory.Weapon.Value = newItem.Guid;
                })
                .AddTo(this);
        }
    }
}
