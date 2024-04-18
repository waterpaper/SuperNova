using UnityEngine;
using DG.Tweening;
using System.Collections;
using Supernova.Unity;
using Supernova.Utils;

public class DropCoin : MonoBehaviour
{
    public string coinPath = "Assets/res/prefab/game/dropItem/Coin.prefab";
    public string soulPath = "Assets/res/prefab/game/dropItem/Soul.prefab";

    [SerializeField] private float dropTime = 0.5f, getTime = 0.3f;
    [SerializeField] private int coinCount = 3, soulCount = 2;

    public void Setting(Character character, Enemy monster, long coin, long soul)
    {
        StartCoroutine(CratePrefab(character, monster, coin, soul));
    }

    IEnumerator CratePrefab(Character character, Enemy monster, long coin, long soul)
    {
        for (int i = 0; i < coinCount; ++i)
        {
            yield return Res.InstantiateAssetAsCoroutineThen<Transform>(coinPath, monster.transform, (prefab) =>
                {
                    prefab.transform.position = monster.transform.position + new Vector3(0, 1.0f, 0);
                    StartCoroutine(Play(character, monster, prefab));
                },
                () =>
                {
                    Log.Warning("coin instantiate fail.");
                }

            );
        };

        if(soul > 0)
        {
            for (int i = 0; i < soulCount; ++i)
            {
                yield return Res.InstantiateAssetAsCoroutineThen<Transform>(soulPath, monster.transform, (prefab) =>
                {
                    prefab.transform.position = monster.transform.position + new Vector3(0, 1.0f, 0);
                    StartCoroutine(Play(character, monster, prefab));
                },
                    () =>
                    {
                        Log.Warning("soul instantiate fail.");
                    }

                );
            };
        }

        StartCoroutine(AddItem(coin, soul));
    }

    IEnumerator Play(Character character, Enemy enemy, Transform trs)
    {
        float ranX = Random.Range(0.9f, 1.5f);
        float ranZ = Random.Range(0.0f, 0.7f) - 0.2f;
        float minus = Random.Range(0.0f, 1.0f) - 0.5f;

        if (minus < 0)
            ranX = -ranX;

        Vector3[] dropPoints = new Vector3[2];
        dropPoints[0] = new Vector3(ranX, 0.5f, ranZ + enemy.transform.position.z);
        dropPoints[1] = enemy.transform.InverseTransformDirection(character.transform.position + Vector3.up);

        var tween = trs.DOMove(dropPoints[0], dropTime)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {

        });

        yield return tween.WaitForCompletion();
        yield return new WaitForSeconds(0.8f);

        trs.DOMove(dropPoints[1], getTime)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                dropPoints[1] = enemy.transform.InverseTransformDirection(character.transform.position + Vector3.up);
            })
            .OnComplete(() =>
            {
                Destroy(trs.gameObject);
            });
    }

    IEnumerator AddItem(long coin, long soul)
    {
        yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.PlayEffect("DropGold");

        yield return new WaitForSeconds(getTime + dropTime + 0.8f - 0.3f);

        Root.State.Currency.Gold.Value += coin;
        Root.State.Currency.Soul.Value += soul;

        //DropItemUIManager.Instance.UICreate(1, coin);
        //DropItemUIManager.Instance.UICreate(2, soul);

        SoundManager.Instance.PlayEffect("GetGold");
        yield return null;
    }
}
