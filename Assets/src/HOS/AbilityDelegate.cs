using System.Collections.Generic;
using Supernova.Unity;
using UniRx;
using UnityEngine;
using System.Linq;
using Supernova.Utils;

public class AbilityDelegate : MonoBehaviour
{
    static AbilityDelegate instance;
    public static AbilityDelegate Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType(typeof(AbilityDelegate)) as AbilityDelegate;

                // Create new instance if one doesn't already exist.
                if (instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<AbilityDelegate>();
                    singletonObject.name = typeof(AbilityDelegate).ToString() + " (Singleton)";

                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    //public Dictionary<long, Ability> abilitys = new Dictionary<long, Ability>();
    // 0
    private Dictionary<long, Ability> leftAbilities = new Dictionary<long, Ability>();
    // 1
    private Dictionary<long, Ability> rightAbilities = new Dictionary<long, Ability>();
    //private Dictionary<long, Ability >rightAbility;

    //public SkillSlot leftSkill;
    //public SkillSlot rightSkill;
    [SerializeField]
    private SkillSlot[] skillSlots;

    // 1
    private Ability leftSkill;
    // 0
    private Ability rightSkill;

    void Awake()
    {
        #region Weapon
        // 스킬 적용
        Root.State.Inventory.Weapon
            .Select(p => Root.State.Inventory.Items.FirstOrDefault(q => q.Guid == p))
            .Where(p => p != null)
            .Select(p => Root.GameInfo.ItemInfos[p.ItemID])
            .Subscribe(weapon =>
            {
                var skills = weapon.Skills.ToArray();
                for (var i = 0; i < skillSlots.Length; i++)
                {
                    if (i < skills.Length)
                    {
                        skillSlots[i].SkillID.Value = skills[i];
                        skillSlots[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        skillSlots[i].gameObject.SetActive(false);
                    }
                }

                Log.Info("무기 스킬 적용: {0}", weapon.ItemID);
            })
            .AddTo(this);

        // 스킬 제거
        Root.State.Inventory.Weapon
            .Where(p => p == null)
            .Subscribe(_ =>
            {
                foreach (var skillSlot in skillSlots)
                {
                    skillSlot.SkillID.Value = -1;
                    skillSlot.gameObject.SetActive(false);
                }

                Log.Info("무기 스킬 해제");
            })
            .AddTo(this);

        // 스킬 사용
        foreach (var skillSlot in skillSlots)
        {
            skillSlot.OnInvokeAsObservable()
                .Select(p => Root.GameInfo.SkilInfos[p])
                .Subscribe(skill =>
                {
                    // 캐릭터 상태 체크
                    // if true ? 스킬 발동 : 넘어감
                    // 1: 대기, 2: 움직임, 3: 공격 ...
                    PlayerState characterState = Ingame.Battle.PlayerCurrentState;
                    //Debug.Log("playerState: " + characterState.ToString());
                    if (Ingame.Battle.IsAttackRange == false) return;
                    if (characterState == PlayerState.Stay || characterState == PlayerState.Attacking)
                    {
                        // 사용 가능
                        Log.Info($"스킬 사용: {skill.SkillID}");

                        bool access = Ingame.Battle.Skill(skill.SkillID);

                        if (access)
                        {
                            Ability ability = null;
                            switch (skill.SkillID)
                            {
                                case 101:
                                    ability = new Skill_Cutdown();
                                break;
                                case 102:
                                    ability = new Skill_Sting();
                                break;
                                case 103:
                                    ability = new Skill_EarthQuake();
                                break;
                                case 104:
                                    ability = new Skill_BloodRake();
                                break;
                                case 301:
                                    ability = new Skill_Meteor();
                                break;
                                case 501:
                                    ability = new Skill_AttakUp();
                                break;
                                default:
                                    // ability = new Skill_EarthQuake();
                                    Debug.Log("Skill Bug");
                                break;
                            }
                            ability.Use(Ingame.Battle.Player, Ingame.Battle.CurrentMonster);

                            // 쿨타임 적용
                            skillSlot.SetCoolDown();
                        }
                    }
                    else
                    {
                        // 사용 불가능
                        Log.Info($"스킬 사용 불가: {skill.SkillID}");
                    }
                })
                .AddTo(this);
        }
        #endregion
        #region Armor
        #endregion
        #region Accessory
        // 특수능력 적용
        Root.State.Inventory.AccessoryLeft
            .Select(p => Root.State.Inventory.Items.FirstOrDefault(q => q.Guid == p))
            .Where(p => p != null)
            .Select(p => Root.GameInfo.ItemInfos[p.ItemID])
            .Subscribe(accessory =>
            {
                var abilities = accessory.Skills;

                AttachAbilities(abilities, 0);

                Log.Info("장신구 특수능력 적용: {0}", accessory.ItemID);
            })
            .AddTo(this);

        // 특수능력 제거
        Root.State.Inventory.AccessoryLeft
            .Where(p => p == null)
            .Subscribe(_ =>
            {
                DetachAbilitys(0);

                Log.Info("장신구 특수 능력 해제");
            })
            .AddTo(this);

        // 특수능력 적용
        Root.State.Inventory.AccessoryRight
            .Select(p => Root.State.Inventory.Items.FirstOrDefault(q => q.Guid == p))
            .Where(p => p != null)
            .Select(p => Root.GameInfo.ItemInfos[p.ItemID])
            .Subscribe(accessory =>
            {
                var abilities = accessory.Skills;

                AttachAbilities(abilities, 1);

                Log.Info("장신구 특수능력 적용: {0}", accessory.ItemID);
            })
            .AddTo(this);

        // 특수능력 제거
        Root.State.Inventory.AccessoryRight
            .Where(p => p == null)
            .Subscribe(_ =>
            {
                DetachAbilitys(1);

                Log.Info("장신구 특수 능력 해제");
            })
            .AddTo(this);
        #endregion
    }

    private void AttachAbilities(System.Collections.Generic.IEnumerable<long> skills, int direction)
    {
        var abilities = direction == 0 ? leftAbilities : rightAbilities;
        DetachAbilitys(direction);

        for (int i = 0; i < skills.Count(); ++i)
        {
            AttachAbility(skills.ElementAt(i), direction);
        }
    }
    private void AttachAbility(long skill, int direction)
    {
        var abilities = direction == 0 ? leftAbilities : rightAbilities;

        if (abilities.ContainsKey(skill))
        {
#if UNITY_EDITOR
            Log.Info("이미 장착된 능력입니다.");
#endif
            return;
        }

        Ability ability = null;
        switch (skill)
        {
            case 1001:
                ability = new Ability_AutoArrow();
            break;
            case 1002:
                ability = new Ability_Thunder();
            break;
            case 1003:
                ability = new Ability_Theif();
            break;
            case 1004:
                ability = new Ability_Backroll();
            break;
            case 1005:
                ability = new Ability_Death();
            break;
            default:
#if UNITY_EDITOR
            Log.Info("잘못된 스킬  Index 입니다.");
#endif
            break;
        }

        if(ability != null)
        {
            ability?.Equip();
            abilities.Add(skill, ability);
        }
        else
        {
            Log.Info("특수능력 장착에 실패 했습니다.");
        }
    }

    private void DetachAbilitys(int direction)
    {
        var abilities = direction == 0 ? leftAbilities : rightAbilities;

        foreach (var item in abilities)
        {
            item.Value.UnEquip();
        }

        abilities.Clear();
    }
}
