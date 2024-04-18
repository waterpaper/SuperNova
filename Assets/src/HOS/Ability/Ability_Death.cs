using UnityEngine;
using Supernova.Unity;
using DG.Tweening;

public class Ability_Death : Ability
{
    public string particlePath = "Assets/res/prefab/game/Ability/Death.prefab";
    public float hitTime = 1f;
    public float destroyTime = 2;

    private float percent = 0.01f;
    private float coolDown = 5;

    public Ability_Death() : base()
    {
    }

    public override void Equip()
    {
        CallBacks.Instance.afterAttack += Use;
    }
    public override void UnEquip()
    {
        CallBacks.Instance.afterAttack -= Use;
    }

    public override void Use(Character character, Enemy monster)
    {
        //throw new System.NotImplementedException();
        if (isUsed || monster == null || monster.IsDead)
        {
            Debug.Log("Monster id dead.."); return;
        }

        float percentage = GetPercentage();

        if(percentage <= percent)
        {
            Instantiate(
            particlePath,
            Root.World.transform,
            (prefab) =>
            {
                prefab.transform.position = monster.transform.position;

                Root.World.StartCoroutine(Cor_DeathAttack(monster, hitTime));

                CoolDown(coolDown);

                Destroy(prefab, destroyTime);
            },
            () =>
            {

            }
            );
        }
    }

    private float GetPercentage()
    {
        return Random.Range(0.0f, 1.0f);
    }

    System.Collections.IEnumerator Cor_DeathAttack(Enemy target, float time)
    {
        yield return new WaitForSeconds(time);

        Ingame.Battle.Attack_DeathAttack(target);
    }
}
