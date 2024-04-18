using System.Text;

namespace Supernova.Unity
{
    /// <summary>
    /// 스텟을 관리하는 구조체입니다.
    /// </summary>
    public struct Stat
    {
        [Newtonsoft.Json.JsonProperty]
        public double Attack { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double AttackSpeed { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double AttackSpeedAmp { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double CooltimeDecrease { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double CriticalProbability { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double CriticalDamage { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double Penetration { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double DamageAmpForBoss { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double DamageAmp { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double AdditionalDamage { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double GoldGain { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double GoldGainAmp { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double LuckRate { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double LuckForce { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public double GoldPerSec { get; private set; }

        public static Stat operator +(Stat a, Stat b)
        {
            return new Stat()
            {
                Attack = a.Attack + b.Attack,
                AttackSpeed = a.AttackSpeed + b.AttackSpeed,
                AttackSpeedAmp = a.AttackSpeedAmp + b.AttackSpeedAmp,
                CooltimeDecrease = a.CooltimeDecrease + b.CooltimeDecrease,
                CriticalProbability = a.CriticalProbability + b.CriticalProbability,
                CriticalDamage = a.CriticalDamage + b.CriticalDamage,
                Penetration = a.Penetration + b.Penetration,
                DamageAmpForBoss = a.DamageAmpForBoss + b.DamageAmpForBoss,
                DamageAmp = a.DamageAmp + b.DamageAmp,
                AdditionalDamage = a.AdditionalDamage + b.AdditionalDamage,
                GoldGain = a.GoldGain + b.GoldGain,
                GoldGainAmp = a.GoldGainAmp + b.GoldGainAmp,
                LuckRate = a.LuckRate + b.LuckRate,
                LuckForce = a.LuckForce + b.LuckForce,
                GoldPerSec = a.GoldPerSec + b.GoldPerSec,
            };
        }

        public static Stat operator *(Stat a, long b)
        {
            return new Stat()
            {
                Attack = a.Attack * b,
                AttackSpeed = a.AttackSpeed * b,
                AttackSpeedAmp = a.AttackSpeedAmp * b,
                CooltimeDecrease = a.CooltimeDecrease * b,
                CriticalProbability = a.CriticalProbability * b,
                CriticalDamage = a.CriticalDamage * b,
                Penetration = a.Penetration * b,
                DamageAmpForBoss = a.DamageAmpForBoss * b,
                DamageAmp = a.DamageAmp * b,
                AdditionalDamage = a.AdditionalDamage * b,
                GoldGain = a.GoldGain * b,
                GoldGainAmp = a.GoldGainAmp * b,
                LuckRate = a.LuckRate * b,
                LuckForce = a.LuckForce * b,
                GoldPerSec = a.GoldPerSec * b,
            };
        }

        public Stat(
            double attack,
            double attackSpeed,
            double attackSpeedAmp,
            double cooltimeDecrease,
            double criticalProbabilty,
            double criticalDamage,
            double penetration,
            double damageAmpForBoss,
            double damageAmp,
            double additionalDamage,
            double goldGain,
            double goldGainAmp,
            double luckRate,
            double luckForce,
            double goldPerSec)
        {
            Attack = attack;
            AttackSpeed = attackSpeed;
            AttackSpeedAmp = attackSpeedAmp;
            CooltimeDecrease = cooltimeDecrease;
            CriticalProbability = criticalProbabilty;
            CriticalDamage = criticalDamage;
            Penetration = penetration;
            DamageAmpForBoss = damageAmpForBoss;
            DamageAmp = damageAmp;
            AdditionalDamage = additionalDamage;
            GoldGain = goldGain;
            GoldGainAmp = goldGainAmp;
            LuckRate = luckRate;
            LuckForce = luckForce;
            GoldPerSec = goldPerSec;
        }

        public FinalStat Finalize()
        {
            return new FinalStat(
                attack: Attack * (1.0 + AttackSpeedAmp),
                attackSpeed: AttackSpeed * (1.0 + AttackSpeedAmp),
                cooltimeDecrease: CooltimeDecrease,
                criticalProbability: CriticalProbability,
                criticalDamage: 1.0 + CriticalDamage,
                penetration: Penetration,
                damageAmpForBoss: 1.0 + DamageAmpForBoss,
                damageAmp: 1.0 + DamageAmp,
                additionalDamage: AdditionalDamage, 
                goldGain: GoldGain * (1.0 + GoldGainAmp),
                luckRate: LuckRate,
                luckForce: 1.0 + LuckForce, 
                goldPerSec: GoldPerSec
                );
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.Attack > 0) { sb.Append($"공격력 {string.Format("{0:0.000}", this.Attack)};"); }
            if (this.Penetration > 0) { sb.Append($"관통력 {string.Format("{0:0.000}", this.Penetration)};"); }
            if (this.AttackSpeed > 0) { sb.Append($"공격속도 {string.Format("{0:0.000}", this.AttackSpeed)};"); }
            if (this.AttackSpeedAmp> 0) { sb.Append($"공격속도 증폭 {string.Format("{0:0.000}", this.AttackSpeedAmp)};"); }
            if (this.CriticalProbability > 0) { sb.Append($"크리티컬 확률 {string.Format("{0:0.000}", this.CriticalProbability)};"); }
            if (this.CriticalDamage > 0) { sb.Append($"크리티컬 피해량 {string.Format("{0:0.000}", this.CriticalDamage)};"); }
            if (this.DamageAmp > 0) { sb.Append($"피해량 증폭 {string.Format("{0:0.000}", this.DamageAmp)};"); }
            if (this.DamageAmpForBoss > 0) { sb.Append($"보스 피해량 증폭 {string.Format("{0:0.000}", this.DamageAmpForBoss)};"); }
            if (this.GoldPerSec > 0) { sb.Append($"추가 피해량 {string.Format("{0:0.000}", this.AdditionalDamage)};"); }
            if (this.CooltimeDecrease > 0) { sb.Append($"쿨타임 감소 {string.Format("{0:0.000}", this.CooltimeDecrease)};"); }
            if (this.GoldGain > 0) { sb.Append($"골드 획득량 {string.Format("{0:0.000}", this.GoldGain)};"); }
            if (this.LuckRate > 0) { sb.Append($"행운 확률 {string.Format("{0:0.000}", this.LuckRate)};"); }
            if (this.LuckForce > 0) { sb.Append($"행운 골드 배율량 {string.Format("{0:0.000}", this.LuckForce)};"); }
            if (this.GoldGainAmp > 0) { sb.Append($"골드 획득량 증폭 {string.Format("{0:0.000}", this.GoldGainAmp)};"); }
            if (this.GoldPerSec > 0) { sb.Append($"초당 골드 획득량 {string.Format("{0:0.000}", this.GoldPerSec)};"); }
            return sb.ToString();
        }
    }
}
