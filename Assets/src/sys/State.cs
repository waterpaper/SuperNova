using Newtonsoft.Json;
using System;
using UniRx;
using UnityEngine;
using System.Linq;
using Supernova.Api.Models;
using Supernova.Utils;
using Cysharp.Threading.Tasks;

namespace Supernova.Unity
{
    /// <summary>
    /// 플레이어 전체 데이터를 저장, 관리 합니다.
    /// </summary>
    public class State
    {
        /// <summary>
        /// 생태 로직
        /// </summary>
        public IStateLogic Logic { get; }
        public ReactiveProperty<Stat> Stat { get; } = new ReactiveProperty<Stat>();
        public ReactiveProperty<FinalStat> FinalStat { get; } = new ReactiveProperty<FinalStat>();
        public General General { get; } = new General();
        public Currency Currency { get; } = new Currency();
        public Inventory Inventory { get; } = new Inventory();
        public Upgrade Upgrade { get; } = new Upgrade();

        public State(IStateLogic logic, MonoBehaviour mono)
        {
            this.Logic = logic;

            Observable.CombineLatest(
                Upgrade.OnChangedAsObservable(),
                Inventory.Weapon, Inventory.Armor, Inventory.AccessoryLeft, Inventory.AccessoryRight,
                (upgrade, weapon, armor, accleft, accright) => (
                    upgrade,
                    weapon      : Inventory.Items.FirstOrDefault(p => p.Guid == weapon),
                    armor       : Inventory.Items.FirstOrDefault(p => p.Guid == armor),
                    accleft     : Inventory.Items.FirstOrDefault(p => p.Guid == accleft),
                    accright    : Inventory.Items.FirstOrDefault(p => p.Guid == accright)
                ))
                .Subscribe(data =>
                {
                    var upgradeStat = new Stat(
                        attack              : logic.StatUpgradeCalculator.Attack(data.upgrade.Attack.Value),
                        attackSpeed         : logic.StatUpgradeCalculator.AttackSpeed(data.upgrade.AttackSpeed.Value),
                        attackSpeedAmp      : logic.StatUpgradeCalculator.AttackSpeedAmp(data.upgrade.AttackSpeedAmp.Value),
                        cooltimeDecrease    : logic.StatUpgradeCalculator.CooltimeDecrease(data.upgrade.CooltimeDecrease.Value),
                        criticalProbabilty  : logic.StatUpgradeCalculator.CriticalProbability(data.upgrade.CriticalProbability.Value),
                        criticalDamage      : logic.StatUpgradeCalculator.CriticalDamage(data.upgrade.CriticalDamage.Value),
                        penetration         : logic.StatUpgradeCalculator.Penetration(data.upgrade.Penetration.Value),
                        damageAmpForBoss    : logic.StatUpgradeCalculator.DamageAmpForBoss(data.upgrade.DamageAmpForBoss.Value),
                        damageAmp           : logic.StatUpgradeCalculator.DamageAmp(data.upgrade.DamageAmp.Value),
                        additionalDamage    : logic.StatUpgradeCalculator.AdditionalDamage(data.upgrade.AdditionalDamage.Value),
                        goldGain            : logic.StatUpgradeCalculator.GoldGain(data.upgrade.GoldGain.Value),
                        goldGainAmp         : logic.StatUpgradeCalculator.GoldGainAmp(data.upgrade.GoldGainAmp.Value),
                        luckRate            : logic.StatUpgradeCalculator.LuckRate(data.upgrade.LuckRate.Value),
                        luckForce           : logic.StatUpgradeCalculator.LuckForce(data.upgrade.LuckForce.Value),
                        goldPerSec          : logic.StatUpgradeCalculator.GoldPerSec(data.upgrade.GoldPerSec.Value)
                    );

                    var weaponStat      = data.weapon == null   ? new Stat() : Root.GameInfo.ItemInfos[data.weapon.ItemID].Stat + Root.GameInfo.ItemInfos[data.weapon.ItemID].UpgradeStat * data.weapon.Enchant;
                    var armorStat       = data.armor == null    ? new Stat() : Root.GameInfo.ItemInfos[data.armor.ItemID].Stat + Root.GameInfo.ItemInfos[data.armor.ItemID].UpgradeStat * data.armor.Enchant;
                    var accleftStat     = data.accleft == null  ? new Stat() : Root.GameInfo.ItemInfos[data.accleft.ItemID].Stat + Root.GameInfo.ItemInfos[data.accleft.ItemID].UpgradeStat * data.accleft.Enchant;
                    var accrightStat    = data.accright == null ? new Stat() : Root.GameInfo.ItemInfos[data.accright.ItemID].Stat + Root.GameInfo.ItemInfos[data.accright.ItemID].UpgradeStat * data.accright.Enchant;

                    var stat = upgradeStat + weaponStat + armorStat + accleftStat + accrightStat;

                    this.Stat.SetValueAndForceNotify(stat);
                    this.FinalStat.SetValueAndForceNotify((stat).Finalize());
                })
                .AddTo(mono);

            this.Inventory.Items
                .ObserveRemove()
                .Subscribe(item =>
                {
                    if (this.Inventory.Weapon.Value == item.Value.Guid) { this.Inventory.Weapon.Value = null; }
                    if (this.Inventory.Armor.Value == item.Value.Guid) { this.Inventory.Armor.Value = null; }
                    if (this.Inventory.AccessoryLeft.Value == item.Value.Guid) { this.Inventory.AccessoryLeft.Value = null; }
                    if (this.Inventory.AccessoryRight.Value == item.Value.Guid) { this.Inventory.AccessoryRight.Value = null; }
                })
                .AddTo(mono);
        }

        public void Reset()
        {
            if (!System.IO.File.Exists($"{Application.persistentDataPath}/.dat"))
            {
                Utils.Log.Warning("FAILED TO RESET DATA. DATA NOT FOUND.");
                return;
            }
            System.IO.File.Delete($"{Application.persistentDataPath}/.dat");
        }

        /// <summary>
        /// 불러온 데이터를 저장합니다.
        /// </summary>
        /// <returns></returns>
        public async UniTask Load()
        {
            var res = await Root.NetworkManager.Request(new Api.Rest.Info.UserGameInfoReq());
            if(res == null)
            {
                Log.Warning("데이터 로드에 실패했습니다. 게임을 새로 시작합니다.");
                throw new Exception();
            }

            try
            {
                var data = res;
                this.Currency.Gold.Value = data.Gold;
                this.Currency.Soul.Value = data.Soul;
                this.Currency.Ruby.Value = data.Ruby;

                this.General.Kill.Value = data.Kill;

                foreach (var item in data.Items) { this.Inventory.Items.Add(item); }
                this.Inventory.Weapon.Value = data.Weapon;
                this.Inventory.Armor.Value = data.Armor;
                this.Inventory.AccessoryLeft.Value = data.AccessoryL;
                this.Inventory.AccessoryRight.Value = data.AccessoryR;

                this.Upgrade.Attack.Value = data.Attack;
                this.Upgrade.AttackSpeed.Value = data.AttackSpeed;
                this.Upgrade.AttackSpeedAmp.Value = data.AttackSpeedAmp;
                this.Upgrade.CooltimeDecrease.Value = data.CooltimeDecrease;
                this.Upgrade.CriticalProbability.Value = data.CriticalProbability;
                this.Upgrade.CriticalDamage.Value = data.CriticalDamage;
                this.Upgrade.Penetration.Value = data.Penetration;
                this.Upgrade.DamageAmpForBoss.Value = data.DamageAmpForBoss;
                this.Upgrade.DamageAmp.Value = data.DamageAmp;
                this.Upgrade.AdditionalDamage.Value = data.AdditionalDamage;
                this.Upgrade.GoldGain.Value = data.GoldGain;
                this.Upgrade.GoldGainAmp.Value = data.GoldGainAmp;
                this.Upgrade.LuckRate.Value = data.LuckRate;
                this.Upgrade.LuckForce.Value = data.LuckForce;
                this.Upgrade.GoldPerSec.Value = data.GoldPerSec;

                // validate
                if (this.Inventory.Items.Any(p => this.Inventory.Items.Where(q => q.Guid == p.Guid).Count() > 1))
                {
                    Log.Warning("같은 고유키의 아이템이 여러 개 발견되었습니다. 인벤토리를 초기화 합니다.");
                    this.Inventory.Items.Clear();
                    this.Inventory.Weapon.Value = null;
                    this.Inventory.Armor.Value = null;
                    this.Inventory.AccessoryLeft.Value = null;
                    this.Inventory.AccessoryRight.Value = null;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Warning("데이터 로드에 실패했습니다.");
                throw e;
            }
        }

        /// <summary>
        /// 데이터를 저장합니다.
        /// </summary>
        /// <returns></returns>
        public async void Save()
        {
            Api.Rest.Info.UserGameInfoSaveReq data = new()
            {
                Gold = this.Currency.Gold.Value,
                Soul = this.Currency.Soul.Value,
                Ruby = this.Currency.Ruby.Value,

                Kill = this.General.Kill.Value,

                Items = this.Inventory.Items.ToList(),
                Weapon = this.Inventory.Weapon.Value,
                Armor = this.Inventory.Armor.Value,
                AccessoryL = this.Inventory.AccessoryLeft.Value,
                AccessoryR = this.Inventory.AccessoryRight.Value,

                Attack = this.Upgrade.Attack.Value,
                AttackSpeed = this.Upgrade.AttackSpeed.Value,
                AttackSpeedAmp = this.Upgrade.AttackSpeedAmp.Value,
                CooltimeDecrease = this.Upgrade.CooltimeDecrease.Value,
                CriticalProbability = this.Upgrade.CriticalProbability.Value,
                CriticalDamage = this.Upgrade.CriticalDamage.Value,
                Penetration = this.Upgrade.Penetration.Value,
                DamageAmpForBoss = this.Upgrade.DamageAmpForBoss.Value,
                DamageAmp = this.Upgrade.DamageAmp.Value,
                AdditionalDamage = this.Upgrade.AdditionalDamage.Value,
                GoldGain = this.Upgrade.GoldGain.Value,
                GoldGainAmp = this.Upgrade.GoldGainAmp.Value,
                LuckRate = this.Upgrade.LuckRate.Value,
                LuckForce = this.Upgrade.LuckForce.Value,
                GoldPerSec = this.Upgrade.GoldPerSec.Value
            };

            var res = await Root.NetworkManager.Request(data);
            Log.Warning("세이브 통신 완료.");
        }
    }

    public class General
    {
        public ReactiveProperty<long> Kill { get; } = new ReactiveProperty<long>();
    }

    public class Currency
    {
        public ReactiveProperty<decimal> Gold { get; } = new ReactiveProperty<decimal>(decimal.Zero);
        public ReactiveProperty<long> Soul { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> Ruby { get; } = new ReactiveProperty<long>();
    }

    public class Inventory
    {
        public ReactiveProperty<string> Weapon { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Armor { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> AccessoryLeft { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> AccessoryRight { get; } = new ReactiveProperty<string>();
        public ReactiveCollection<ItemInstance> Items { get; } = new ReactiveCollection<ItemInstance>();

    }

    public class Upgrade
    {
        public ReactiveProperty<long> Attack { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> AttackSpeed { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> AttackSpeedAmp { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> CooltimeDecrease { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> CriticalProbability { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> CriticalDamage { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> CriticalDamageAmp { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> Penetration { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> DamageAmpForBoss { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> DamageAmp { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> AdditionalDamage { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> GoldGain { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> GoldGainAmp { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> LuckRate { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> LuckForce { get; } = new ReactiveProperty<long>();
        public ReactiveProperty<long> GoldPerSec { get; } = new ReactiveProperty<long>();

        public IObservable<Upgrade> OnChangedAsObservable() =>
            Observable.CombineLatest(
                Attack,
                AttackSpeed,
                AttackSpeedAmp,
                CooltimeDecrease,
                CriticalProbability,
                CriticalDamage,
                CriticalDamageAmp,
                Penetration,
                DamageAmpForBoss,
                DamageAmp,
                AdditionalDamage,
                GoldGain,
                GoldGainAmp,
                LuckRate,
                LuckForce,
                GoldPerSec
            )
            .Select(p => this);
    }

    public interface IChangable<T>
    {
        IObservable<T> OnChangedAsObservable();
    }

    public interface IStateLogic
    {
        IGeneralCalculator GeneralCalculator { get; }
        IStatUpgradeCalculator StatUpgradeCalculator { get; }
        IStageCalculator StageCalculator { get; }

        /// <summary>
        /// 대기 중 지급되는 골드를 처리합니다. 일정 시간 마다 주기적으로 호출하세요.
        /// </summary>
        void AddIdleGold();
        /// <summary>
        /// 골드 버튼을 탭할 떄 마다 호출하세요.
        /// </summary>
        void AddHitGold(bool isJackpot);
        /// <summary>
        /// 아이템을 추가합니다.
        /// </summary>
        void AddItem(long itemID);
        /// <summary>
        /// 보유한 아이템을 강화합니다. 아이템이 무기 또는 방어구여야 합니다.
        /// </summary>
        void EnchantItem(string guid);
        /// <summary>
        /// 보유한 두 아이템을 합성합니다. 두 아이템이 악세서리여야 합니다.
        /// </summary>
        void ComposeItem(string guid1, string guid2);

        long DropItem();
    }

    public interface IStatUpgradeCalculator
    {
        double Attack(long upgrade);
        double AttackSpeed(long upgrade);
        double AttackSpeedAmp(long upgrade);
        double CooltimeDecrease(long upgrade);
        double CriticalProbability(long upgrade);
        double CriticalDamage(long upgrade);
        double Penetration(long upgrade);
        double DamageAmpForBoss(long upgrade);
        double DamageAmp(long upgrade);
        double AdditionalDamage(long upgrade);
        double GoldGain(long upgrade);
        double GoldGainAmp(long upgrade);
        double LuckRate(long upgrade);
        double LuckForce(long upgrade);
        double GoldPerSec(long upgrade);

        long PriceAttack(long upgrade);
        long PriceAttackSpeed(long upgrade);
        long PriceAttackSpeedAmp(long upgrade);
        long PriceCooltimeDecrease(long upgrade);
        long PriceCriticalProbability(long upgrade);
        long PriceCriticalDamage(long upgrade);
        long PriceCriticalDamageAmp(long upgrade);
        long PriceAdditionalDamage(long upgrade);
        long PricePenetration(long upgrade);
        long PriceDamageAmpForBoss(long upgrade);
        long PriceDamageAmp(long upgrade);
        long PriceGoldGain(long upgrade);
        long PriceGoldGainAmp(long upgrade);
        long PriceLuckRate(long upgrade);
        long PriceLuckForce(long upgrade);
        long PriceGoldPerSec(long upgrade);
    }

    public interface IGeneralCalculator
    {
        long EnchantPriceGold(ItemInstance itemInstance);
        long EnchantPriceSoul(ItemInstance itemInstance);
        double EnchantProbability(ItemInstance itemInstance);
    }

    public interface IStageCalculator
    {
        double HP(long killCount);
        long Defense(long killCount);
        double ReductionRate(long killCount);
    }
}
