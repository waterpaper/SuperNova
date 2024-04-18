using UnityEngine;
using Supernova.Unity;
using Supernova.Utils;

public class Ability_Theif : Ability
{
    public string particlePath = "Assets/res/prefab/game/Ability/Gold dot.prefab";
    public float particleTime = 0.5f;

    public Ability_Theif() : base()
    {
    }

    public override void Equip()
    {
        CallBacks.Instance.monsterDie += Use;
    }
    public override void UnEquip()
    {
        CallBacks.Instance.monsterDie -= Use;
    }

    public override void Use(Character character, Enemy monster)
    {
        //throw new System.NotImplementedException();
        //if (monster.IsDead)
        //{
        //    Debug.Log("Monster id dead.."); return;
        //}

        Instantiate(
            particlePath,
            Root.World.transform,
            (prefab) =>
            {
                prefab.position = character.transform.position;

                Destroy(prefab, particleTime);
            },
            () =>
            {
                Log.Warning("투사체 생성에 실패하였습니다.");
            });

        var stage = monster.Stage;

        Root.State.Currency.Gold.Value += stage * 10;
    }
}
