using UnityEngine;
using Supernova.Unity;
using Supernova.Utils;
using DG.Tweening;

public class Ability_AutoArrow : Ability
{
    public string projectilePath = "Assets/res/prefab/game/Ability/AutoArrowProjectile.prefab";
    public string flashPath = "Assets/res/prefab/game/Ability/AutoArrowFlash.prefab";
    public string hitPath = "Assets/res/prefab/game/Ability/AutoArrowHit.prefab";
    public float hitTime = 0.5f;
    public int count = 5;
    public float multiply = 0.25f;

    public Ability_AutoArrow() : base()
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
        if (monster.IsDead)
        {
            Debug.Log("Monster id dead.."); return;
        }

        for(int i = 0; i < count; ++i)
        {
            InstantiateDelay(
                i * 0.1f, 
                projectilePath,
                Root.World.transform,
                (prefab) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
                {
                    prefab.transform.position = character.transform.position + new Vector3(0, 0.5f, 0);

                    Vector3[] points = new Vector3[]
                    {
                         new Vector3(0, 0.5f, 0) + -character.transform.forward + character.transform.right * Random.Range(-1.0f, 1.0f),
                         new Vector3(0, 0.5f, 0) + character.transform.position + character.transform.up * Random.Range(0, 0.5f) + character.transform.right * Random.Range(-1.0f, 1.0f),
                         monster.HeartPoint.transform.position
                    };
                    prefab.DOPath(points, hitTime, PathType.CatmullRom)
                    .SetEase(Ease.Linear)
                    .onComplete += () =>
                    {
                        Vector3 completePosition = prefab.transform.position;
                        Instantiate(
                            hitPath,
                            Root.World.transform,
                            (prefab2) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
                            {
                                prefab2.transform.position = completePosition;
                                Destroy(prefab2, 1);
                            },
                            () =>
                            {

                            });
                        Instantiate(
                            flashPath,
                            Root.World.transform,
                            (prefab2) => // instantiate에 성공하면 prefab 파라메터로 오브젝트가 전달됩니다.
                            {
                                prefab2.transform.position = completePosition;
                                Destroy(prefab2, 1);
                            },
                            () =>
                            {

                            });

                        Attack(monster, multiply);
                        Destroy(prefab, 0);
                    };
                },
                () => // instantiate에 실패하면 이 콜백이 호출됩니다.
                {
                    Log.Warning("투사체 생성에 실패하였습니다.");
                }
            );
        }
    }
}
