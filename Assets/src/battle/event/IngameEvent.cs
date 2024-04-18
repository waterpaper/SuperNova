using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Supernova.Unity
{
    /// <summary>
    /// 게임 내부에서 emit되는 이벤트를 정의합니다.
    /// </summary>
    
    public class TargetedEvent : IEvent
    {
        public Enemy enemy;
    }

    public class DamageEvent : IEvent
    {
        public Enemy enemy;
        public long damage;
        public bool isCri;
        public long baseDamage;
        public long addDamage;
    }

    public class DeathEvent : IEvent
    {
        public Character player;
        public Enemy enemy;
    }
}