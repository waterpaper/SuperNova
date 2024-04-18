using UnityEngine;

namespace Supernova.Unity
{
    public class Monster : Enemy
    {
        [SerializeField] public MonsterKind Kind { get; set; }

        public override void Setting(long stage)
        {
            base.Setting(stage);

            var abil = BattleAbility.StageEnemyAbility(Stage, Type, Kind);
            Hp = abil.hp;
            Defense = abil.defense;
            HpBar.SetHpBar(Hp);
        }
    }   
}


