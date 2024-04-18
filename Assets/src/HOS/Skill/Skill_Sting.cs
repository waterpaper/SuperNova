using UnityEngine;
using Supernova.Unity;
using Supernova.Utils;

public class Skill_Sting : Ability
{
    public string path = "Assets/res/prefab/game/Ability/Sword Slash Combo 8.prefab";
    public float startDelay = 0f;
    public float hitTime = 0.4f;
    public float multiply = 1.2f;

    public Skill_Sting() : base()
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
            startDelay,
            path,
            Root.World.transform,
            (prefab) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
            {
                prefab.position = character.transform.position + new Vector3(0, -0.5f, 0);
                prefab.Rotate(-15, 0, 0);

                Destroy(prefab, 1);
                AttackDelay(monster, multiply, hitTime);
            },
            () => // instantiate에 실패하면 이 콜백이 호출됩니다.
            {
                Log.Warning("투사체 생성에 실패하였습니다.");
            }
        );
    }
}
