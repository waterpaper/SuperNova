using UnityEngine;
using Supernova.Unity;
using Supernova.Utils;

public class Ability_Backroll : Ability
{
    public string particlePath = "Assets/res/prefab/game/Ability/Front spikes attack.prefab";

    private float coolDown = 10;

    public Ability_Backroll() : base()
    {
    }

    public override void Equip()
    {
        CallBacks.Instance.earnGold += Use;
    }
    public override void UnEquip()
    {
        CallBacks.Instance.earnGold -= Use;
    }

    public override void Use(Character character, Enemy monster)
    {
        //throw new System.NotImplementedException();
        if (isUsed || monster == null || monster.IsDead)
        {
            Debug.Log("Monster id dead.."); return;
        }

        Instantiate(
            particlePath,
            Root.World.transform,
            (prefab) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
            {
                prefab.transform.position = character.transform.position;// + new Vector3(0, 0.5f, 0);

                Ingame.Battle.Attack_DamageValue(monster, (long)(Root.State.Currency.Gold.Value / 100));

                Destroy(prefab, 1);

                CoolDown(coolDown);
            },
            () => // instantiate에 실패하면 이 콜백이 호출됩니다.
            {
                Log.Warning("투사체 생성에 실패하였습니다.");
            }
        );
    }
}
