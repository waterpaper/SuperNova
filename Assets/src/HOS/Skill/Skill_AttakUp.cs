using UnityEngine;
using Supernova.Unity;
using Supernova.Utils;

public class Skill_AttakUp : Ability
{
    public string path = "Assets/res/prefab/game/Ability/PowerUp.prefab";
    public float buffTime = 5;
    public float interval = 0.5f;
    public float multiply = 0.2f;

    public Skill_AttakUp() : base()
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
        Instantiate(
            path,
            Root.World.transform,
            (prefab) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
            {
                prefab.position = character.transform.position + new Vector3(0, 0.1f, 0);// + new Vector3(0, 0.3f, 0.2f);

                Root.World.StartCoroutine(DotAttack(monster));

                Destroy(prefab, buffTime);
            },
            () => // instantiate에 실패하면 이 콜백이 호출됩니다.
            {
                Log.Warning("투사체 생성에 실패하였습니다.");
            }
        );
    }

    private System.Collections.IEnumerator DotAttack(Enemy target)
    {
        float passTime = 0;
        do
        {
            passTime += interval;

            Ingame.Battle.Attack(target, multiply);

            yield return new WaitForSeconds(interval);
        } while (passTime <= buffTime);
    }
}
