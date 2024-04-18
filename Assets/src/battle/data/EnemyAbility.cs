
namespace Supernova.Unity
{
    public struct EnemyAbility
    {
        public double hp;
        public long defense;
        public double damageReductionRate;
        public float avoidability;
    }
    /// <summary>
    /// ���������� ���� ������ ����ϴ� ���Դϴ�.
    /// </summary>
    public class BattleAbility
    {
        public static EnemyAbility StageEnemyAbility(long stage, EnemyType type, MonsterKind kind)
        {
            EnemyAbility abil = new EnemyAbility();

            abil.hp = Root.State.Logic.StageCalculator.HP(stage);
            abil.defense = Root.State.Logic.StageCalculator.Defense(stage);
            abil.damageReductionRate = 0;
            return abil;
        }

        public static EnemyAbility StageBossAbility(long stage, EnemyType type, BossKind kind)
        {
            EnemyAbility abil = new EnemyAbility();

            abil.hp = abil.hp = Root.State.Logic.StageCalculator.HP(stage);
            abil.defense = Root.State.Logic.StageCalculator.Defense(stage);
            abil.damageReductionRate = Root.State.Logic.StageCalculator.ReductionRate(stage);
            return abil;
        }
    }
}