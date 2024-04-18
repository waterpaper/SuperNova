using UnityEngine;
using Supernova.Unity;
using Supernova.Utils;

public class Ability_Thunder : Ability
{
    public string projectilePath = "Assets/res/prefab/game/Ability/Lightning strike.prefab";
    public string hitPath = "Assets/res/prefab/game/Ability/Lightning strike Hit.prefab";
    public float hitTime = 2;
    public float multiply = 1.2f;

    public Ability_Thunder() : base()
    {
    }

    public override void Equip()
    {
        //throw new System.NotImplementedException();
        CallBacks.Instance.afterAttack += Use;
    }
    public override void UnEquip()
    {
        CallBacks.Instance.afterAttack -= Use;
    }

    public override void Use(Character character, Enemy monster)
    {
        //throw new System.NotImplementedException();
        if (monster.IsDead)
        {
            Debug.Log("Monster id dead.."); return;
        }

        Instantiate(
            projectilePath,
            Root.World.transform,
            (prefab) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
            {
                prefab.position = monster.HeartPoint.transform.position;

            InstantiateDelay(
                0.2f,
                hitPath,
                Root.World.transform,
                (prefab2) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
                {
                    prefab2.transform.position = monster.HeartPoint.transform.position;

                    Attack(monster, multiply);
                    Destroy(prefab, 2);
                    Destroy(prefab2, 2);
                },
                () =>
                {

                });
            },
            () => // instantiate에 실패하면 이 콜백이 호출됩니다.
            {
                Log.Warning("투사체 생성에 실패하였습니다.");
            }
         );
    }
}
