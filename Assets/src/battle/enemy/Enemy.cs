using System;
using System.Collections;
using UnityEngine;
using Supernova.Utils;

namespace Supernova.Unity
{
    /// <summary>
    /// 적 전체 정보 및 ani을 처리합니다.
    /// </summary>
    public abstract class Enemy : MonoBehaviour
    {
        protected readonly int hashHit = Animator.StringToHash("Hit");
        protected readonly int hashDead = Animator.StringToHash("IsDead");

        [SerializeField] public double Hp { get; protected set; }
        [SerializeField] public long Defense { get; protected set; }
        [SerializeField] public EnemyType Type { get; private set; }
        [SerializeField] public EnemyState State { get; private set; }
        public bool IsDead { get; private set; }
        public long Stage { get; private set; }
        public GameObject HeartPoint { get; private set; }
        public GameObject HeadPoint { get; private set; }
        public Animator Ani { get; private set; }
        public Hpbar HpBar { get; private set; }


        public void Awake()
        {
            Ani = gameObject.GetComponentInChildren<Animator>();
            HpBar = gameObject.transform.GetChild(1).GetComponent<Hpbar>();

            HeartPoint = gameObject.transform.GetChild(2).gameObject;
            HeadPoint = gameObject.transform.GetChild(3).gameObject;

            if (Ani == null)
                Log.Info("anim not found");
        }

        public void Update()
        {
            if (HpBar != null)
                HpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.5f, 0));
        }

        private void OnEnable()
        {
            if (Ani != null)
                Ani.SetBool(hashDead, false);

            if (HpBar != null)
            {
                HpBar.gameObject.SetActive(true);
                HpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.5f, 0));
            }

            IsDead = false;
        }

        private void OnDisable()
        {
            if (HpBar != null)
                HpBar.gameObject.SetActive(false);
        }

        public void Damage(long damage, Action<Enemy> deadAfterEvent)
        {
            this.Hp -= damage;

            if (this.Hp <= 0)
            {
                this.Hp = 0;
                Root.State.General.Kill.Value += 1;
                StartCoroutine(Dead(deadAfterEvent));
            }
            else {
                if(State == EnemyState.Stay)
                    StartCoroutine(Hit());
            }
            
            HpBar.DamageHpBar(this.Hp);
        }

        protected IEnumerator Hit()
        {
            State = EnemyState.Hit;
            Ani.SetTrigger(hashHit);

            yield return new WaitForSeconds(0.5f);
            State = EnemyState.Stay;
        }

        protected IEnumerator Dead(Action<Enemy> deadAfterEvent)
        {
            State = EnemyState.Dead;
            Ani.SetBool(hashDead, true);
            IsDead = true;
            yield return new WaitForSeconds(1.0f);
            HpBar.gameObject.SetActive(false);

            yield return new WaitForSeconds(3.0f);

            State = EnemyState.Stay;
            gameObject.SetActive(false);
            deadAfterEvent(this);
            Destroy(gameObject);
            Ingame.Battle.RemoveEvent(this);
        }

        public virtual void Setting(long stage)
        {
            Stage = stage;
        }

        public virtual double DefenseCheck(double damage, double penetration)
        {
            double defenseRemainder = Defense - penetration;

            if (defenseRemainder < 0)
                defenseRemainder = 0;

            return damage * (100 / (100 + defenseRemainder));
        }
    }
}
