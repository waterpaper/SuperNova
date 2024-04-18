using UnityEngine;
using Supernova.Unity;
using Supernova.Utils;

public class Skill_Meteor : Ability
{
    public string path = "Assets/res/prefab/game/Ability/Meteor 2.prefab";
    public float multiply = 3f;
    public int attackCount = 10;
    public float hitTime = 0.6f;

    public Skill_Meteor() : base()
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

        Instantiate(
            path,
            Root.World.transform,
            (prefab) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
            {
                prefab.position = monster.transform.position + new Vector3(0, 0.1f, 0);

                Destroy(prefab, 3f);
                //AttackDelay(monster, multiply, hitTime);

                AttackDelay(monster, multiply, hitTime);
            },
            () => // instantiate에 실패하면 이 콜백이 호출됩니다.
            {
                Log.Warning("투사체 생성에 실패하였습니다.");
            }
        );
    }
}
