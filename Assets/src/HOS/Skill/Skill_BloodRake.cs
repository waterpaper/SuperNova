using UnityEngine;
using Supernova.Unity;
using Supernova.Utils;

public class Skill_BloodRake : Ability
{
    public string path = "Assets/res/prefab/game/Ability/Sword Slash 17.prefab";
    public float mainMultiply = 1.2f;
    public float submultiply = 0.5f;
    public int attackCount = 10;

    public Skill_BloodRake() : base()
    {
    }

    public override void Equip()
    {
    }
    public override void UnEquip()
    {
    }

    public override void Use(Character character, Enemy monster)
    {
        //throw new System.NotImplementedException();
        if (monster != null && monster.IsDead)
        {
            Debug.Log("Monster id dead.."); return;
        }

        InstantiateDelay(
            0.1f, 
            path,
            Root.World.transform,
            (prefab) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
            {
                prefab.position = character.transform.position + new Vector3(0, 0.5f, 0.0f);

                Destroy(prefab, 1f);
                //AttackDelay(monster, multiply, hitTime);

                AttackDelay(monster, mainMultiply, 0.2f);

                for(int i = 0; i < attackCount; ++i)
                {
                    AttackDelay(monster, submultiply, 0.3f + i * 0.05f);
                }
            },
            () => // instantiate에 실패하면 이 콜백이 호출됩니다.
            {
                Log.Warning("투사체 생성에 실패하였습니다.");
            }
        );
    }
}
