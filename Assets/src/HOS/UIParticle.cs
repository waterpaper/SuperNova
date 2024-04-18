using System.Collections;
using UnityEngine;
using Supernova.Unity;

public class UIParticle : MonoBehaviour
{
    static UIParticle instance;
    public static UIParticle Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(UIParticle)) as UIParticle;

                // Create new instance if one doesn't already exist.
                if (instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<UIParticle>();
                    singletonObject.name = typeof(UIParticle).ToString() + " (Singleton)";

                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    public string goldParticle = "Assets/res/prefab/game/UI/GoldParticle.prefab";

    public void GoldParticle(bool isJackpot)
    {
        if (isJackpot)
        {
            StartCoroutine(Res.InstantiateAssetAsCoroutineThen<RectTransform>(
            "Assets/res/prefab/game/UI/Jackpot.prefab",
            transform,
            (prefab) =>
            {
                prefab.anchoredPosition = new Vector2(Input.mousePosition.x - Screen.width, Input.mousePosition.y);
                // Root.World.WorldCamera.ScreenToWorldPoint(Input.mousePosition);

                StartCoroutine(DestroyParticle(prefab, 1));
            },
            () =>
            {

            }
            ));
        }
        else
        {
            StartCoroutine(Res.InstantiateAssetAsCoroutineThen<RectTransform>(
                "Assets/res/prefab/game/UI/GoldParticle.prefab",
                transform,
                (prefab) =>
                {
                    prefab.anchoredPosition = new Vector2(Input.mousePosition.x - Screen.width, Input.mousePosition.y);
                    // Root.World.WorldCamera.ScreenToWorldPoint(Input.mousePosition);

                    StartCoroutine(DestroyParticle(prefab, 1));
                },
                () =>
                {

                }
                ));
        }
    }

    private IEnumerator DestroyParticle(Transform particle, float time)
    {
        yield return new WaitForSeconds(time);

        Res.Release(particle.gameObject);
    }
}
