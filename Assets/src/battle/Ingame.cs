using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Supernova.Unity
{
    public class Ingame : MonoBehaviour
    {
        public static BattleManager Battle { get; protected set; }

        private BossHpbar bossHpbar = null;
        private DropCoin dropCoin = null;
        private DropItemUIManager dropItem = null;


        public void Awake()
        {
            Battle = new();
            Bind();

            dropCoin = GetComponent<DropCoin>();
            if (GameObject.Find("BossBar"))
            {
                bossHpbar = GameObject.Find("BossBar").GetComponent<BossHpbar>();
                bossHpbar.gameObject.SetActive(false);
            }

            if (GameObject.Find("DropItemUIManager"))
            {
                dropItem = GameObject.Find("DropItemUIManager").GetComponent<DropItemUIManager>();
            }
        }
        private void Update()
        {
            Battle.StateJudgment();
        }

        private void OnDestroy()
        {
            UnBind();
        }

        private void Bind()
        {
            Root.EventListeners.On<TargetedEvent>(OnTargetedEvent);
            Root.EventListeners.On<DamageEvent>(OnDamageEvent);
            Root.EventListeners.On<DeathEvent>(OnDeathEvent);
        }

        private void UnBind()
        {
            Root.EventListeners.Off<TargetedEvent>(OnTargetedEvent);
            Root.EventListeners.Off<DamageEvent>(OnDamageEvent);
            Root.EventListeners.Off<DeathEvent>(OnDeathEvent);
        }

        private void OnTargetedEvent(TargetedEvent e)
        {
            if (e.enemy.Type == EnemyType.Boss)
                bossHpbar.SetBossHpBar(e.enemy.Hp);
        }
        private void OnDamageEvent(DamageEvent e)
        {
            DamageFontManager.Instance.ShowDamage(e.enemy.transform.position, e.baseDamage, e.isCri, e.addDamage);
            if (e.enemy.Type == EnemyType.Boss)
                bossHpbar.DamageBossHpBar(e.enemy.Hp);
        }
        private void OnDeathEvent(DeathEvent e)
        {
            DropItem(e.player, e.enemy);
            if (e.enemy.Type == EnemyType.Boss)
                bossHpbar.gameObject.SetActive(false);
        }

        private void DropItem(Character character, Enemy enemy)
        {
            dropCoin.Setting(character, enemy, enemy.Stage * 10, enemy.Stage * 10);

            var itemIndex = Root.State.Logic.DropItem();

            Debug.LogFormat("item: {0} ¿ª »πµÊ«’¥œ¥Ÿ.", itemIndex);
            if (itemIndex != -1)
            {
                Root.State.Inventory.Items.Add(ItemInstance.Create(itemIndex));
                dropItem.UICreate(3, itemIndex);
            }
        }
    }
}