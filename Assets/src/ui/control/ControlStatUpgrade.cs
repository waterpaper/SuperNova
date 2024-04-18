using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class ControlStatUpgrade : MonoBehaviour
    {
        [SerializeField]
        private StatUpgradeType statType;
        [SerializeField]
        private Text textLabel, textDescription, textPrice;

        private void Awake()
        {
            this.GetComponent<Button>()
                .OnClickAsObservable()
                .Select(_ =>
                {
                    IReactiveProperty<long> stat = null;
                    long price = -1;

                    switch (this.statType)
                    {
                    case StatUpgradeType.Attack:
                        stat = Root.State.Upgrade.Attack;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceAttack(stat.Value + 1);
                        break;
                    case StatUpgradeType.AttackSpeed:
                        stat = Root.State.Upgrade.AttackSpeed;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceAttackSpeed(stat.Value + 1);
                        break;
                    case StatUpgradeType.AttackSpeedAmp:
                        stat = Root.State.Upgrade.AttackSpeedAmp;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceAttackSpeedAmp(stat.Value + 1);
                        break;
                    case StatUpgradeType.CooltimeDecrease:
                        stat = Root.State.Upgrade.CooltimeDecrease;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceCooltimeDecrease(stat.Value + 1);
                        break;
                    case StatUpgradeType.CriticalDamage:
                        stat = Root.State.Upgrade.CriticalDamageAmp;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceCriticalDamage(stat.Value + 1);
                        break;
                    case StatUpgradeType.CriticalDamageAmp:
                        stat = Root.State.Upgrade.CriticalDamageAmp;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceCriticalDamageAmp(stat.Value + 1);
                        break;
                    case StatUpgradeType.CriticalProbability:
                        stat = Root.State.Upgrade.CriticalProbability;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceCriticalProbability(stat.Value + 1);
                        break;
                    case StatUpgradeType.DamageAmp:
                        stat = Root.State.Upgrade.DamageAmp;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceDamageAmp(stat.Value + 1);
                        break;
                    case StatUpgradeType.DamageAmpForBoss:
                        stat = Root.State.Upgrade.DamageAmpForBoss;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceDamageAmpForBoss(stat.Value + 1);
                        break;
                    case StatUpgradeType.AdditionalDamage:
                        stat = Root.State.Upgrade.AdditionalDamage;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceAdditionalDamage(stat.Value + 1);
                        break;
                    case StatUpgradeType.GoldGain:
                        stat = Root.State.Upgrade.GoldGain;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceGoldGain(stat.Value + 1);
                        break;
                    case StatUpgradeType.GoldGainAmp:
                        stat = Root.State.Upgrade.GoldGainAmp;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceGoldGainAmp(stat.Value + 1);
                        break;
                    case StatUpgradeType.LuckForce:
                        stat = Root.State.Upgrade.LuckForce;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceLuckForce(stat.Value + 1);
                        break;
                    case StatUpgradeType.LuckRate:
                        stat = Root.State.Upgrade.LuckRate;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceLuckRate(stat.Value + 1);
                        break;
                    case StatUpgradeType.Penetration:
                        stat = Root.State.Upgrade.Penetration;
                        price = Root.State.Logic.StatUpgradeCalculator.PricePenetration(stat.Value + 1);
                        break;
                    case StatUpgradeType.GoldPerSec:
                        stat = Root.State.Upgrade.GoldPerSec;
                        price = Root.State.Logic.StatUpgradeCalculator.PriceGoldPerSec(stat.Value + 1);
                        break;
                    }
                    return (stat, price);
                })
                .Where(p => p.stat != null && p.price != -1)
                .Subscribe(p =>
                {
                    if (Root.State.Currency.Gold.Value < p.price)
                    {
                        Root.PopupManager.ShowPopup(new PopupAlertArgs("골드가 부족합니다."));
                        return;
                    }

                    Root.State.Currency.Gold.Value -= p.price;
                    p.stat.Value += 1;
                })
                .AddTo(this);

            var suc = Root.State.Logic.StatUpgradeCalculator;

            Root.State.Upgrade.Attack
                .Where(_ => statType == StatUpgradeType.Attack)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"공격력 Lv.{upgrade.next}";
                    textDescription.text = $"공격력 {suc.Attack(upgrade.cur)} -> {suc.Attack(upgrade.next)}";
                    textPrice.text = $"{suc.PriceAttack(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.AttackSpeed
                .Where(_ => statType == StatUpgradeType.AttackSpeed)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"공격속도 Lv.{upgrade.next}";
                    textDescription.text = $"공격속도 {suc.AttackSpeed(upgrade.cur)} -> {suc.AttackSpeed(upgrade.next)}";
                    textPrice.text = $"{suc.PriceAttackSpeed(upgrade.next)} 골드";

                    gameObject.SetActive(upgrade.cur < 1000);
                })
                .AddTo(this);

            Root.State.Upgrade.AttackSpeedAmp
                .Where(_ => statType == StatUpgradeType.AttackSpeedAmp)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"공격속도 증폭 Lv.{upgrade.next}";
                    textDescription.text = $"공격속도 증폭 {suc.AttackSpeedAmp(upgrade.cur)} -> {suc.AttackSpeedAmp(upgrade.next)}";
                    textPrice.text = $"{suc.PriceAttackSpeedAmp(upgrade.next)} 골드";

                    gameObject.SetActive(upgrade.cur < 1000);
                })
                .AddTo(this);

            Root.State.Upgrade.CooltimeDecrease
                .Where(_ => statType == StatUpgradeType.CooltimeDecrease)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"쿨타임 감소 Lv.{upgrade.next}";
                    textDescription.text = $"쿨타임 감소 {suc.CooltimeDecrease(upgrade.cur)} -> {suc.CooltimeDecrease(upgrade.next)}";
                    textPrice.text = $"{suc.PriceCooltimeDecrease(upgrade.next)} 골드";

                    gameObject.SetActive(upgrade.cur < 1000);
                })
                .AddTo(this);

            Root.State.Upgrade.CriticalDamage
                .Where(_ => statType == StatUpgradeType.CriticalDamage)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"크리티컬 피해량 Lv.{upgrade.next}";
                    textDescription.text = $"크리티컬 피해량 {suc.CriticalDamage(upgrade.cur)} -> {suc.CriticalDamage(upgrade.next)}";
                    textPrice.text = $"{suc.PriceCriticalDamage(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.CriticalDamageAmp
                .Where(_ => statType == StatUpgradeType.CriticalDamageAmp)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"크리티컬 피해량 증폭 Lv.{upgrade.next}";
                    //textDescription.text = $"크리티컬 피해량 증폭 {suc.CriticalDamageAmp(upgrade.cur)} -> {suc.CriticalDamageAmp(upgrade.next)}";
                    textPrice.text = $"{suc.PriceCriticalDamageAmp(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.CriticalProbability
                .Where(_ => statType == StatUpgradeType.CriticalProbability)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"크리티컬 확률 Lv.{upgrade.next}";
                    textDescription.text = $"크리티컬 확률 {suc.CriticalProbability(upgrade.cur)} -> {suc.CriticalProbability(upgrade.next)}";
                    textPrice.text = $"{suc.PriceCriticalProbability(upgrade.next)} 골드";

                    gameObject.SetActive(upgrade.cur < 1000);
                })
                .AddTo(this);

            Root.State.Upgrade.DamageAmp
                .Where(_ => statType == StatUpgradeType.DamageAmp)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"피해량 증폭 Lv.{upgrade.next}";
                    textDescription.text = $"피해량 증폭 {suc.DamageAmp(upgrade.cur)} -> {suc.DamageAmp(upgrade.next)}";
                    textPrice.text = $"{suc.PriceDamageAmp(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.DamageAmpForBoss
                .Where(_ => statType == StatUpgradeType.DamageAmpForBoss)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"보스 대상 피해량 증폭 Lv.{upgrade.next}";
                    textDescription.text = $"보스 대상 피해량 증폭 {suc.DamageAmpForBoss(upgrade.cur)} -> {suc.DamageAmpForBoss(upgrade.next)}";
                    textPrice.text = $"{suc.PriceDamageAmpForBoss(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.AdditionalDamage
                .Where(_ => statType == StatUpgradeType.AdditionalDamage)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"추가 피해량 Lv.{upgrade.next}";
                    textDescription.text = $"추가 피해량 {suc.AdditionalDamage(upgrade.cur)} -> {suc.AdditionalDamage(upgrade.next)}";
                    textPrice.text = $"{suc.PriceAdditionalDamage(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.GoldGain
                .Where(_ => statType == StatUpgradeType.GoldGain)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"골드 획득량 Lv.{upgrade.next}";
                    textDescription.text = $"골드 획득량 {suc.GoldGain(upgrade.cur)} -> {suc.GoldGain(upgrade.next)}";
                    textPrice.text = $"{suc.PriceGoldGain(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.GoldGainAmp
                .Where(_ => statType == StatUpgradeType.GoldGainAmp)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"골드 획득량 증폭 Lv.{upgrade.next}";
                    textDescription.text = $"골드 획득량 증폭 {suc.GoldGainAmp(upgrade.cur)} -> {suc.GoldGainAmp(upgrade.next)}";
                    textPrice.text = $"{suc.PriceGoldGainAmp(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.LuckForce
                .Where(_ => statType == StatUpgradeType.LuckForce)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"행운 골드 획득량 Lv.{upgrade.next}";
                    textDescription.text = $"행운 골드 획득량 {suc.LuckForce(upgrade.cur)} -> {suc.LuckForce(upgrade.next)}";
                    textPrice.text = $"{suc.PriceLuckForce(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.LuckRate
                .Where(_ => statType == StatUpgradeType.LuckRate)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"행운 확률 Lv.{upgrade.next}";
                    textDescription.text = $"행운 확률 {suc.LuckRate(upgrade.cur)} -> {suc.LuckRate(upgrade.next)}";
                    textPrice.text = $"{suc.PriceLuckRate(upgrade.next)} 골드";

                    gameObject.SetActive(upgrade.cur < 1000);
                })
                .AddTo(this);

            Root.State.Upgrade.Penetration
                .Where(_ => statType == StatUpgradeType.Penetration)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"관통력 Lv.{upgrade.next}";
                    textDescription.text = $"관통력 {suc.Penetration(upgrade.cur)} -> {suc.Penetration(upgrade.next)}";
                    textPrice.text = $"{suc.PricePenetration(upgrade.next)} 골드";
                })
                .AddTo(this);

            Root.State.Upgrade.GoldPerSec
                .Where(_ => statType == StatUpgradeType.GoldPerSec)
                .Select(cur => (cur: cur, next: cur + 1))
                .Subscribe(upgrade =>
                {
                    textLabel.text = $"초당 골드 획득량 Lv.{upgrade.next}";
                    textDescription.text = $"초당 골드 획득량 {suc.GoldPerSec(upgrade.cur)} -> {suc.GoldPerSec(upgrade.next)}";
                    textPrice.text = $"{suc.PriceGoldPerSec(upgrade.next)} 골드";
                })
                .AddTo(this);
        }
    }

    public enum StatUpgradeType
    {
        Attack,
        AttackSpeed,
        AttackSpeedAmp,
        CooltimeDecrease,
        CriticalDamage,
        CriticalDamageAmp,
        CriticalProbability,
        DamageAmp,
        DamageAmpForBoss,
        AdditionalDamage,
        GoldGain,
        GoldGainAmp,
        LuckForce,
        LuckRate,
        Penetration,
        GoldPerSec,
    }
}
