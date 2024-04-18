namespace Supernova.Unity
{
    public struct FinalStat
    {
        public double Attack { get; private set; }
        public double AttackSpeed { get; private set; }
        public double CooltimeDecrease { get; private set; }
        public double CriticalProbability { get; private set; }
        public double CriticalDamage { get; private set; }
        public double Penetration { get; private set; }
        public double DamageAmpForBoss { get; private set; }
        public double DamageAmp { get; private set; }
        public double AdditionalDamage { get; private set; }
        public double GoldGain { get; private set; }
        public double LuckRate { get; private set; }
        public double LuckForce { get; private set; }
        public double GoldPerSec { get; private set; }

        public FinalStat(
            double attack,
            double attackSpeed,
            double cooltimeDecrease,
            double criticalProbability,
            double criticalDamage,
            double penetration,
            double damageAmpForBoss,
            double damageAmp,
            double additionalDamage,
            double goldGain,
            double luckRate,
            double luckForce,
            double goldPerSec)
        {
            Attack = attack;
            AttackSpeed = attackSpeed;
            CooltimeDecrease = cooltimeDecrease;
            CriticalProbability = criticalProbability;
            CriticalDamage = criticalDamage;
            CooltimeDecrease = cooltimeDecrease;
            Penetration = penetration;
            DamageAmpForBoss = damageAmpForBoss;
            DamageAmp = damageAmp;
            AdditionalDamage = additionalDamage;
            GoldGain = goldGain;
            LuckRate = luckRate;
            LuckForce = luckForce;
            GoldPerSec = goldPerSec;
        }
    }
}