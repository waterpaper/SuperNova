using System.Collections;
using UniRx;
using UnityEngine;
using Supernova.Utils;

namespace Supernova.Unity
{
    /// <summary>
    /// 플레이어의 정보를 가지고 있는 클래스입니다.
    /// 내부 ani 및 transform을 조정합니다.
    /// </summary>
    public class Character : MonoBehaviour
    {
        private readonly int hashAttack = Animator.StringToHash("isAttack");
        private readonly int hashAttackSpeed = Animator.StringToHash("attackSpeed");
        private readonly int hashAttackKind = Animator.StringToHash("attackKind");
        private readonly int hashSpeed = Animator.StringToHash("speed");
        private readonly int hashStay = Animator.StringToHash("isStay");
        private readonly int hashStart = Animator.StringToHash("isStart");

        [SerializeField]
        public float moveMentSpeed = 3.0f;
        [SerializeField]
        private float turnSpeed = 180f;
        [SerializeField]
        private float attackRangeOffset = 1.3f;
        [SerializeField]
        private float attackSpeed = 1.0f;
        [SerializeField]
        private PlayerState state;
        [SerializeField]
        private Enemy target;

        private Animator ani;
        private int attackKind;
        private IEnumerator skillCo;

        public bool isAttackRange = false;
        public bool isMoveState = false;
        public bool isAttackState = false;
        public bool isSkillState = false;

        private bool isSkillAnimStart = false;

        public float AttackRangeOffset { get { return attackRangeOffset; } private set { } }
        public float AttackSpeed { get { return attackSpeed; } set { 
                attackSpeed = value;
                ani.SetFloat(hashAttackSpeed, attackSpeed);
        }}

        public PlayerState State { get { return state; } private set { } }
        public bool IsStart { set { ani.SetBool(hashStart, value); } }


        void Start()
        {
            ani = GetComponent<Animator>();

            if (ani == null)
                Log.Warning("Not found ChartacterController");

            state = PlayerState.Stay;
            ani.SetBool(hashStay, true);
            ani.SetFloat(hashAttackSpeed, attackSpeed);
        }

        private void Update()
        {
            if (ani.GetBool(hashStay) == true && state != PlayerState.Stay && isSkillAnimStart == false)
            {
                state = PlayerState.Stay;

                isMoveState = false;
                isAttackState = false;

                ani.SetBool(hashAttack, false);

                if(isSkillAnimStart == false)
                {
                    isSkillState = false;
                    ani.SetInteger("skillKind", 0);
                    ani.SetBool("isSkill", false);
                    skillCo = null;
                }
            }
        }

        void Rotate(Vector3 target)
        {
            Vector3 dir = target - transform.position;
            Vector3 dirXZ = new Vector3(dir.x, 0f, dir.z);

            //Rotation
            {
                if (dirXZ != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dirXZ);
                    Quaternion frameRot = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

                    transform.rotation = frameRot;
                }
            }
        }

        public void Movement(Vector3 target, float speed)
        {   
            if (target != Vector3.zero)
                Rotate(target);

            state = PlayerState.Move;
            ani.SetFloat(hashSpeed, speed);
        }


        public void Behavior(int beHavior, Enemy enemy, long skillKind = 0)
        {
            target = enemy;

            if (beHavior == 1)
            {
                attackKind = UnityEngine.Random.Range(1, 3);
                StartCoroutine(AttackBehavior(attackKind));
            }
            else if (beHavior == 2)
            {
                if (!isAttackRange) return;
                if (isAttackState)
                    AttackStop();

                if (skillCo != null)
                {
                    /* if (_state == PlayerState.SkillBefore)
                         StopCoroutine(_skillCo);
                     else
                         return;*/
                    return;
                }

                skillCo = SkillBehavior(skillKind);
                StartCoroutine(skillCo);
            }
        }

        public IEnumerator AttackBehavior(int _attackKind)
        {
            state = PlayerState.Attacking;
            ani.SetBool(hashAttack, true);
            ani.SetInteger(hashAttackKind, _attackKind);
            ani.SetBool(hashStay, false);

            isAttackState = true;
            yield return null;
        }

        void AttackEvent(int type)
        {
            if (isAttackState == false) return;
            if (type == 1)
            {
                //_state = PlayerState.AttackBefore;
            }
            else if (type == 2)
            {
                //_state = PlayerState.Attacking;

                if (target.IsDead == false)
                {
                    SoundManager.Instance.PlayEffect("Hit");
                    Ingame.Battle.Attack(target, 1, true, true, true, true);
                }
                //BattleManager.Instance.Attack_DeathAttack(_target);

                if (target.IsDead == true)
                    target = null;

                //Debug.Log(_target);
            }
            else if (type == 3)
            {
                //_state = PlayerState.AttackEnd;
            }
            else if (type == 4)
            {
                AttackStop();
            }
        }

        public void AttackStop()
        {
            ani.SetBool(hashAttack, false);
            isAttackState = false;
        }

        public IEnumerator SkillBehavior(long skillKind)
        {
            isSkillState = true;
            isSkillAnimStart = true;
            //Log.Info("skill start");
            ani.SetBool("isSkill", true);
            ani.SetInteger("skillKind", (int)skillKind);
            ani.SetBool(hashStay, false);
            state = PlayerState.Skilling;
            
            yield return new WaitForSeconds(0.2f);
            isSkillAnimStart = false;

            yield return null;
        }


    }

    public enum PlayerState
    {
        Null,
        Stay,
        Move,
        Attacking,
        Skilling,
        Last,
    }
}
