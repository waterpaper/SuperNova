using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Supernova.Unity
{
    /// <summary>
    /// ���� ���ο��� emit�Ǵ� �̺�Ʈ�� �����մϴ�.
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