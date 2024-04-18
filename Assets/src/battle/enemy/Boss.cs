using System.Collections.Generic;
using UnityEngine;

namespace Supernova.Unity
{
    public class Boss : Enemy
    {
        [SerializeField] public BossKind Kind { get; private set; }

        [SerializeField] public double DamageReductionRate { get; private set; }

        public override void Setting(long stage)
        {
            base.Setting(stage);

            var abil = BattleAbility.StageBossAbility(stage, Type, Kind);
            Hp = abil.hp;
            Defense = abil.defense;
            DamageReductionRate = abil.damageReductionRate;

            HpBar.SetHpBar(Hp);
        }

        public override double DefenseCheck(double damage, double penetration)
        {
            damage = base.DefenseCheck(damage, penetration);

            if(damage <= 0)
            {
                return 0;
            }

            damage -= damage * DamageReductionRate * 0.01f;

            return damage;
        }
    }
}
