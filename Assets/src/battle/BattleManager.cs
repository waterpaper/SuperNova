using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Supernova.Unity
{
    /// <summary>
    /// 전투와 관련된 클래스를 생성하고 행동시키는 클래스입니다.
    /// 적은 미리 캐싱해둔 데이터를 복사해 사용합니다.(이유 - 새로 생성 호출시 지연 발생)
    /// </summary>
    public class BattleManager
    {
        //움직임 처리
        public ReactiveProperty<float> Move { get; } = new ReactiveProperty<float>();
        //소환 위치
        public ReactiveProperty<float> SpawnPostion { get; } = new ReactiveProperty<float>();

        private Character player;
        private List<Enemy> enemys = new();
        private Enemy targetEnemy = null;

        private Transform enemyRoot = null;
        private Dictionary<long, Enemy> enemyCache = new();

        private bool isStart = false;
        private float _attackTime = 0f;


        public PlayerState PlayerCurrentState
        {
            get
            {
                if (player != null)
                    return player.State;
                else
                    return 0;
            }
            private set { }
        }

        public bool IsAttackRange
        {
            get
            {
                if (player != null)
                    return player.isAttackRange;
                else
                    return false;
            }
            private set { }
        }


        public Enemy CurrentMonster { get { return targetEnemy; } }
        public Character Player { get { return player; } }


        public void Initialize()
        {
            Bind();
        }

        public void Destroy()
        {
            UnBind();
        }


        public async UniTask Load(Transform root)
        {
            await Res.InstantiateAssetAsCoroutineThen<Transform>(BattleConfig.playerPath, root, (prefab) =>
            {
                player = prefab.gameObject.GetComponent<Character>();
                player.transform.position = Vector3.zero;
                player.transform.tag = "Player";
            });

            enemyRoot = new GameObject("Enemy").transform;
            enemyRoot.parent = root.transform;

            GameObject enemyCacheRoot = new GameObject("EnemyCache");
            enemyCacheRoot.transform.parent = root.transform;

            foreach (var monsterInfo in Root.GameInfo.MonsterInfo.Values)
            {
                string str = $"{BattleConfig.enemyPath}{monsterInfo.Path}/{monsterInfo.Path}.prefab";

                await Res.InstantiateAssetAsCoroutineThen<Transform>(str, enemyCacheRoot.transform, (prefab) =>
                {
                    prefab.gameObject.SetActive(false);
                    enemyCache.Add(monsterInfo.MonsterID, prefab.GetComponent<Enemy>());
                });
            }
        }

        private void Bind()
        {
            SpawnPostion.Value = 5.0f;
            Root.State.Currency.Gold.Subscribe(gold =>
            {
                if (CallBacks.Instance.earnGold != null)
                    CallBacks.Instance.earnGold(Player, targetEnemy);
            });


            Move.Subscribe(value => MoveEnemys(-value));
            Move.Subscribe(value => SpawnPostion.Value -= value);
        }

        private void UnBind()
        {

        }

        public void StartSetting()
        {
            for (long i = 1; i <= 3; i++)
                ActiveSetting(Root.State.General.Kill.Value + i);
        }

        public void GameStart()
        {
            StartSetting();
            Move.Subscribe(value => MoveEnemys(-value));

            isStart = true;
            player.IsStart = true;
        }

        /// 각 player, enemy는 정보 및 ani 처리를 하며
        /// 공격 및 전체 로직은 이곳에서 판단하여 처리합니다.
        public void StateJudgment()
        {
            if (!isStart) return;
            _attackTime += Time.deltaTime;
            if (_attackTime > 100.0f)
                _attackTime = 100.0f;

            if (enemys.Count == 0)
            {
                player.Movement(Vector3.zero, 0.0f);
            }
            else
            {
                int index = -1;
                for (int i = 0; i < enemys.Count; i++)
                {
                    if (enemys[i].IsDead == false)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1) return;


                bool isAttackRange = (Vector3.Distance(player.transform.position, enemys[index].transform.position) <= player.AttackRangeOffset);

                if (player.State == PlayerState.Stay)
                {
                    if (isAttackRange == false)
                    {
                        Vector3 moveTarget = enemys[index].transform.position;

                        Vector3 framePos = Vector3.MoveTowards(Vector3.zero, moveTarget, player.moveMentSpeed * Time.deltaTime);
                        Vector3 moveDir = -(framePos);
                        Move.Value = moveDir.z;
                        player.isMoveState = true;
                        player.isAttackRange = false;
                        player.Movement(Vector3.zero, player.moveMentSpeed);
                        _attackTime = 100.0f;
                        targetEnemy = null;
                    }
                    else if (isAttackRange == true)
                    {
                        player.isAttackRange = true;
                        targetEnemy = enemys[index];
                        
                        Root.EventListeners.Emit(new TargetedEvent() { enemy = targetEnemy });
                        if (player.isSkillState != true)
                        {
                            AutoAttack(index);
                        }
                    }
                }
                else if (player.State == PlayerState.Move)
                {
                    if (isAttackRange == true && player.isMoveState)
                    {
                        player.Movement(Vector3.zero, 0.0f);
                    }
                    else
                    {
                        Vector3 moveTarget = enemys[index].transform.position;

                        Vector3 framePos = Vector3.MoveTowards(Vector3.zero, moveTarget, player.moveMentSpeed * Time.deltaTime);
                        Vector3 moveDir = -(framePos);
                        Move.Value = moveDir.z;
                    }

                }
            }
        }

        public void Attack(Enemy targetEnemy, float pMultiply = 1.0f, bool isCriAttck = true, bool isBossAddAttack = true, bool isAddDamage = true, bool isPlayerAutoAttack = false)
        {
            if (enemys.Count < 0) return;

            int index = -1;
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] == targetEnemy)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1 || enemys[index].IsDead) return;


            if (isPlayerAutoAttack == true)
            {
                CallBacks.Instance.afterAttack?.Invoke(player, enemys[index]);
            }

            var state = Root.State.FinalStat.Value;

            double attackDamage = state.Attack * pMultiply;
            double addAttack = 0;
            bool isCriPer = false;
            attackDamage = enemys[index].DefenseCheck(attackDamage, state.Penetration);

            //if (attackDamage <= 0)
            //{
            //  DamageFontManager.Instance.ShowMiss(enemys[index].transform.position);
            //  return;
            //}

            if (attackDamage >= 1)
            {
                if (isCriAttck)
                {
                    var criPer = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (criPer < state.CriticalProbability * 0.01f)
                    {
                        isCriPer = true;
                        attackDamage *= (state.CriticalDamage);
                    }
                }

                if (isBossAddAttack)
                {
                    if (enemys[index].Type == EnemyType.Boss)
                        attackDamage *= (state.DamageAmpForBoss);
                }

                if (isAddDamage)
                    addAttack += attackDamage * state.AdditionalDamage;
            }

            long finalAttack = (long)System.Math.Floor(attackDamage) + (long)addAttack;
            if (finalAttack <= 1) finalAttack = 1;

            enemys[index].Damage(finalAttack, (monster) => { enemys.Remove(monster); });
            Root.EventListeners.Emit(new DamageEvent() { enemy = enemys[index], damage = finalAttack, isCri = isCriPer, baseDamage = (long)System.Math.Floor(attackDamage),  addDamage = (long)addAttack });

            this.DeathEvent(index);
        }

        private long CalcAdditionalDamage(long damage, FinalStat finalStat)
        {
            double ratio = finalStat.AdditionalDamage;

            return (long)(damage * ratio);
        }

        public void Attack_ActiveCrirical(Enemy targetEnemy, float pMultiply = 1.0f)
        {
            if (enemys.Count < 0) return;

            int index = -1;
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] == targetEnemy)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1 || enemys[index].IsDead) return;

            var state = Root.State.FinalStat.Value;

            double attackDamage = state.Attack * pMultiply;
            double addAttack = 0;

            attackDamage = enemys[index].DefenseCheck(attackDamage, state.Penetration);

            if (attackDamage >= 1)
            {
                attackDamage *= (state.CriticalDamage);

                if (enemys[index].Type == EnemyType.Boss)
                {
                    attackDamage *= (state.DamageAmpForBoss);
                }

                addAttack += attackDamage * state.AdditionalDamage;
            }

            long finalAttack = (long)System.Math.Floor(attackDamage + addAttack);
            if (finalAttack <= 1) finalAttack = 1;

            enemys[index].Damage(finalAttack, (monster) => { enemys.Remove(monster); });
            Root.EventListeners.Emit(new DamageEvent() { enemy = enemys[index], damage = finalAttack, isCri = true, baseDamage = (long)System.Math.Floor(attackDamage), addDamage = (long)addAttack });

            this.DeathEvent(index);
        }

        public void Attack_DamageValue(Enemy targetEnemy, long damage, bool isTrueDamage = false)
        {
            if (enemys.Count < 0) return;

            int index = -1;
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] == targetEnemy)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1 || enemys[index].IsDead) return;

            var state = Root.State.FinalStat.Value;
            double attackDamage = damage;

            if (isTrueDamage == false)
                attackDamage = enemys[index].DefenseCheck(damage, state.Penetration);

            long finalAttack = (long)attackDamage;

            enemys[index].Damage(finalAttack, (monster) => { enemys.Remove(monster); });
            DamageFontManager.Instance.ShowDamage(enemys[index].transform.position, finalAttack, false);
            Root.EventListeners.Emit(new DamageEvent() { enemy = enemys[index], damage = finalAttack, isCri = false, baseDamage = (long)System.Math.Floor(attackDamage), addDamage = 0 });
            this.DeathEvent(index);
        }

        public void Attack_DeathAttack(Enemy targetEnemy)
        {
            if (enemys.Count < 0) return;

            int index = -1;
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] == targetEnemy)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1 || enemys[index].IsDead) return;

            long damage = (long)enemys[index].Hp;

            enemys[index].Damage(damage, (monster) => { enemys.Remove(monster); });
            Root.EventListeners.Emit(new DamageEvent() { enemy = enemys[index], damage = damage, isCri = false, baseDamage = damage, addDamage = 0 });

            this.DeathEvent(index);
        }

        public void AutoAttack(int enemyIndex)
        {
            if (player.State == PlayerState.Stay && player.isAttackRange == true)
            {
                double attackSpeed = 1.0f / Root.State.FinalStat.Value.AttackSpeed;
                float aniAttackSpeed = (float)Root.State.FinalStat.Value.AttackSpeed / (0.792f) * 10f;

                aniAttackSpeed = Mathf.Ceil(aniAttackSpeed) / 10;

                //Log.Info(_attackTime);
                //Log.Info(aniAttackSpeed);

                if (aniAttackSpeed < 1.0f)
                    aniAttackSpeed = 1.0f;

                player.AttackSpeed = aniAttackSpeed;

                if (_attackTime >= attackSpeed)
                {
                    //Log.Info("attackSpeed : " + attackSpeed);
                    CallBacks.Instance.beforeAttack?.Invoke(player, enemys[enemyIndex]);
                    if (enemys.Count > 0)
                        player.Behavior(1, enemys[enemyIndex]);
                    _attackTime = 0;

                    SoundManager.Instance.PlayEffect("Attack");

                    //Log.Info(enemyIndex);
                    //CallBacks.Instance.afterAttack?.Invoke(player, enemys[enemyIndex]);
                }
            }
        }

        public void DeathEvent(int enmeyIndex)
        {
            if (enemys[enmeyIndex].IsDead)
            {
                player.isAttackRange = false;

                CallBacks.Instance.monsterDie?.Invoke(player, enemys[enmeyIndex]);
                Root.EventListeners.Emit(new DeathEvent() { player = player, enemy = enemys[enmeyIndex] });

                int activeCount = 1;
                for (int i = 0; i < enemys.Count; i++)
                {
                    if (enemys[i].IsDead == false)
                        activeCount++;
                }

                ActiveSetting(Root.State.General.Kill.Value + activeCount);
            }
        }

        public void RemoveEvent(Enemy e)
        {
            enemys.Remove(e);
        }
        
        public bool Skill(long skillKind)
        {
            if (enemys.Count < 0) return false;

            int index = -1;
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i].IsDead == false)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1) return false;

            if (player.isAttackRange)
            {
                if (player.isAttackState == false)
                    player.AttackStop();

                player.Behavior(2, enemys[index], skillKind);
            }

            return true;
        }

        private void MoveEnemys(float speed)
        {
            foreach (var enemy in enemys)
            {
                enemy.transform.Translate(new Vector3(0, 0, -speed));
            }

        }

        public void ActiveSetting(long stage)
        {
            int remainder = (int)Math.Abs(stage % (BattleConfig.mapCount * BattleConfig.maxTileThemeEnemyCount)) / 10;
            TileMapEnum nowMapIndex = (TileMapEnum)remainder + 1;
            int mapId = BattleConfig.mapIDStart + remainder;
            SpawnPostion.Value += 3; 

            var mapInfo = Root.GameInfo.MapInfos.Values.FirstOrDefault(data => data.MapID == mapId);
            if (stage % BattleConfig.maxTileThemeEnemyCount != 0)
                ActiveEnemySetting(mapInfo.Monsters, stage, new Vector3(0, 0, SpawnPostion.Value));
            
            else
                ActiveBossSetting(mapInfo.Boss, stage, new Vector3(0, 0, SpawnPostion.Value));
        }

        void ActiveEnemySetting(List<long> monsterIds, long stage, Vector3 pos)
        {
            int index;
            int remainder = (int)(stage % BattleConfig.maxTileThemeEnemyCount);
            if ((remainder >= 1 && remainder <= 2) || (remainder >= 6 && remainder <= 7))
                index = 0;
            else
                index = 1;

            if (enemyCache.ContainsKey(monsterIds[index]) == false) return;

            var enemyObject = GameObject.Instantiate(enemyCache[monsterIds[index]].gameObject, enemyRoot);
            var enemy = enemyObject.GetComponent<Monster>();
            enemy.Awake();
            enemy.Setting(stage);
            enemy.transform.position = pos;
            enemy.gameObject.SetActive(true);

            enemys.Add(enemy);
        }

        void ActiveBossSetting(long bossId, long stage, Vector3 pos)
        {
            if (enemyCache.ContainsKey(bossId) == false) return;

            var enemyObject = GameObject.Instantiate(enemyCache[bossId].gameObject, enemyRoot);
            var boss = enemyObject.GetComponent<Boss>();
            boss.Awake();
            boss.Setting(stage);
            boss.transform.position = pos;
            boss.gameObject.SetActive(true);

            enemys.Add(boss);
        }
    }
}
