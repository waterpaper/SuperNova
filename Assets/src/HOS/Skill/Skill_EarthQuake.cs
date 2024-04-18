using UnityEngine;
using Supernova.Unity;
using Supernova.Utils;

public class Skill_EarthQuake : Ability
{
    public string path = "Assets/res/prefab/game/Ability/Spikes attack.prefab";
    public float startDelay = 0.4f;
    public float hitTime = 0f;
    public float multiply = 2f;

    public Skill_EarthQuake() : base()
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
                prefab.position = character.transform.position + new Vector3(0, 0, 0.4f);

                Destroy(prefab, 1.8f);
                //AttackDelay(monster, multiply, hitTime);
                Attack(monster, multiply);
            },
            () => // instantiate에 실패하면 이 콜백이 호출됩니다.
            {
                Log.Warning("투사체 생성에 실패하였습니다.");
            }
        );
    }
}
