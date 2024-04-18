using Supernova.Unity;
using System.Collections;
using UnityEngine;

public abstract class Ability
{
    protected bool isUsed = false;

    public Ability()
    {

    }

    public abstract void Equip();
    public abstract void UnEquip();
    public abstract void Use(Character character, Enemy monster);

    protected void Instantiate(string path, Transform parent, System.Action<Transform> action1, System.Action action2)
    {
        Root.World.StartCoroutine(Res.InstantiateAssetAsCoroutineThen<Transform>(
            path,
            parent,
            action1,
            action2
            ));
    }

    protected void InstantiateDelay(float time, string path, Transform parent, System.Action<Transform> action1, System.Action action2)
    {
        Root.World.StartCoroutine(Cor_Instanticate(
            time, path, parent, action1, action2
        ));
    }

    protected void Destroy(Transform prefab, float time)
    {
        Root.World.StartCoroutine(Cor_Destroy(prefab, time));
    }

    protected void Attack(Enemy target, float multiply)
    {
        Ingame.Battle.Attack(target, multiply);
    }

    protected void AttackDelay(Enemy target, float multiply, float time)
    {
        Root.World.StartCoroutine(Cor_Attack(target, multiply, time));
    }

    protected void CoolDown(float time)
    {
        Root.World.StartCoroutine(Cor_CoolDown(time));
    }

    private IEnumerator Cor_Instanticate(float time, string path, Transform parent, System.Action<Transform> action1, System.Action action2)
    {
        yield return new WaitForSeconds(time);

        Root.World.StartCoroutine(Res.InstantiateAssetAsCoroutineThen<Transform>(
            path,
            parent,
            action1,
            action2
            ));
    }

    private IEnumerator Cor_Destroy(Transform prefab, float time)
    {
        yield return new WaitForSeconds(time);

        prefab.gameObject.Release();
    }

    private IEnumerator Cor_Attack(Enemy target, float multiply, float time)
    {
        yield return new WaitForSeconds(time);

        Ingame.Battle.Attack(target, multiply);
    }

    private IEnumerator Cor_CoolDown(float time)
    {
        isUsed = true;

        yield return new WaitForSeconds(time);

        isUsed = false;
    }
}
