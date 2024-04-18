using System;
using System.Linq;
using Supernova.Utils;

namespace Supernova.Unity
{
    /// <summary>
    /// 여기에서 일반적인 데이터 로직을 처리합니다.
    /// </summary>
    public class MainStateLogic : IStateLogic
    {
        private static readonly decimal
            IDLEGOLD_BASE = 1.0m,
            GOLDGAIN_BASE = 10.0m,
            GOLDGAIN_PEPUPGRADE = 1.5m,
            GOLDGAINAMP_PERUPGRADE = 1.1m;

        public IStatUpgradeCalculator StatUpgradeCalculator { get; } = new MainStatUpgradeCalculator();

        public IGeneralCalculator GeneralCalculator { get; } = new MainGeneralCalculator();

        public IStageCalculator StageCalculator { get; } = new StageCalculator();

        public void AddHitGold(bool isJackpot)
        {
            var final = Root.State.FinalStat.Value;
            var add = final.GoldGain;
            double mul = 1;
            if (isJackpot)
            {
                mul = final.LuckForce;
            }

            Root.State.Currency.Gold.Value += (decimal)(add * mul);
        }

        public void AddIdleGold()
        {
            var add = Root.State.FinalStat.Value.GoldPerSec;

            Root.State.Currency.Gold.Value += (decimal)(add);
        }

        public void AddItem(long itemID)
        {
            Root.State.Inventory.Items.Add(ItemInstance.Create(itemID));
        }

        public void EnchantItem(string guid)
        {
            if (Root.State.Currency.Gold.Value < 1000)
            {
                Log.Warning($"not enough gold to enchant the item. (guid:{guid})");
                return;
            }
            Root.State.Currency.Gold.Value -= 1000;

            var items = Root.State.Inventory.Items;
            var item = items.FirstOrDefault(p => p.Guid == guid);
            if (item == null)
            {
                Log.Warning($"failed to enchant an item. item not found. (guid:{guid})");
                return;
            }
            // TODO: check item type

            items.Remove(item);
            items.Add(ItemInstance.Upgrade(item));
        }

        public void ComposeItem(string guid1, string guid2)
        {
            throw new System.NotImplementedException();
        }

        public long DropItem()
        {
            var type = UnityEngine.Random.Range(0, 2);
            var percent = UnityEngine.Random.Range(0.0f, 1.0f);

            var keys = Root.GameInfo.ItemInfos.Where(x => x.Value.ItemType == (ItemType)type).Select(x => x.Key);
            UnityEngine.Debug.LogFormat("type: {0} 중에 뽑습니다. keys: {1}", (ItemType)type, keys.Count());
            for(int i = keys.Count() - 1; i >= 0; --i)
            {
                UnityEngine.Debug.LogFormat("i: {0}", i);
                var item = Root.GameInfo.ItemInfos[keys.ElementAt(i)];
                UnityEngine.Debug.LogFormat("itemCode: {0}, drop: {1}, percent: {2}", item.Name, item.DropRate, percent);
                if(percent <= item.DropRate)
                {
                    return item.ItemID;
                }
            }

            return -1;
        }
    }

    public class MainGeneralCalculator : IGeneralCalculator
    {
        public long EnchantPriceGold(ItemInstance itemInstance)
        {
            return 100 * itemInstance.Enchant;
        }

        public long EnchantPriceSoul(ItemInstance itemInstance)
        {
            return 0;
        }

        public double EnchantProbability(ItemInstance itemInstance)
        {
            return 1.0 / Math.Max(1, itemInstance.Enchant);
        }
    }

    public class StageCalculator : IStageCalculator
    {
        private double SqrtCalc(double x)
        {
            return Math.Sqrt(x);
        }

        private long GetStage(long killCount)
        {
            return killCount / 10;
        }
        public double HP(long killCount)
        {
            //long stage = GetStage(killCount);

            return 50 + killCount * 5;
        }

        public long Defense(long killCount)
        {
            //long stage = GetStage(killCount);

            return 5 + killCount * 1;
        }
        public double ReductionRate(long killCount)
        {
            long stage = GetStage(killCount);

            double max = 1000000;
            double ymax = 99.999999;

            return Math.Min(ymax, Math.Max(0, SqrtCalc(stage / max)));
        }
    }

    public class MainStatUpgradeCalculator : IStatUpgradeCalculator
    {
        private double LogCalc(double x, double b)
        {
            return Math.Log(x, b);
        }
        private double SqrtCalc(double x)
        {
            return Math.Sqrt(x);
        }

        public double Attack(long upgrade)
        {
            return 10 + upgrade * 1;
        }

        public double Penetration(long upgrade)
        {
            return 0 + upgrade * 1;
        }

        public double AttackSpeed(long upgrade)
        {
            double max = 10000;
            double initial = 0.2;
            double ymax = 3.0;
            return Math.Min(ymax, initial + Math.Max(0, SqrtCalc(upgrade / max)) * (ymax - initial));
        }

        public double AttackSpeedAmp(long upgrade)
        {
            double max = 10000;
            return Math.Min(1, 0 + Math.Max(0, SqrtCalc(upgrade / max)));
        }

        public double CriticalProbability(long upgrade)
        {
            double max = 10000;
            return Math.Min(1, 0 + Math.Max(0, SqrtCalc(upgrade / max)));
        }

        public double CriticalDamage(long upgrade)
        {
            double max = 10000;
            double initial = 0.5;
            return initial + Math.Max(0, SqrtCalc(upgrade / max)) * (1 - initial);
        }

        public double DamageAmp(long upgrade)
        {
            double max = 10000;
            return 0 + Math.Max(0, SqrtCalc(upgrade / max));
        }

        public double DamageAmpForBoss(long upgrade)
        {
            double max = 10000;
            return 0 + Math.Max(0, SqrtCalc(upgrade / max));
        }

        public double AdditionalDamage(long upgrade)
        {
            double max = 10000;
            return 0 + Math.Max(0, SqrtCalc(upgrade / max));
        }

        public double CooltimeDecrease(long upgrade)
        {
            double max = 10000;
            double ymax = 0.6;
            return Math.Min(ymax, 0 + Math.Max(0, SqrtCalc(upgrade / max)) * ymax);
        }

        public double GoldPerSec(long upgrade)
        {
            return 1 + upgrade * 1;
        }

        public double GoldGain(long upgrade)
        {
            return 10 + upgrade * 15;
        }

        public double GoldGainAmp(long upgrade)
        {
            double max = 10000;
            return 0 + Math.Max(0, SqrtCalc(upgrade / max));
        }

        public double LuckRate(long upgrade)
        {
            double max = 10000;
            return Math.Min(1, 0 + Math.Max(0, SqrtCalc(upgrade / max)));
        }

        public double LuckForce(long upgrade)
        {
            double max = 10000;
            return 0 + Math.Max(0, SqrtCalc(upgrade / max));
        }

        ///////////////////////////////////////////////////////////////////////

        public long PriceAdditionalDamage(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceAttack(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceAttackSpeed(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceAttackSpeedAmp(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceCooltimeDecrease(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceCriticalDamage(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceCriticalDamageAmp(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceCriticalProbability(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceDamageAmp(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceDamageAmpForBoss(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceGoldGain(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceGoldGainAmp(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceGoldPerSec(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceLuckForce(long upgrade)
        {
            return upgrade * 100;
        }

        public long PriceLuckRate(long upgrade)
        {
            return upgrade * 100;
        }

        public long PricePenetration(long upgrade)
        {
            return upgrade * 100;
        }
    }
}
