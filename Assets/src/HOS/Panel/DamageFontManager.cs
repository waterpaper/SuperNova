using System.Collections;
using UnityEngine;
using Supernova.Unity;
using DG.Tweening;
using TMPro;

public class DamageFontManager : MonoBehaviour
{
    static DamageFontManager instance;

    public static DamageFontManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(DamageFontManager)) as DamageFontManager;

                // Create new instance if one doesn't already exist.
                if (instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<DamageFontManager>();
                    singletonObject.name = typeof(DamageFontManager).ToString() + " (Singleton)";

                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    public Camera mainCamera;

    string prefix = "Assets/res/prefab/game/Damage";


    void Awkae()
    {
        mainCamera = Camera.main;
    }

    public void ShowMiss(Vector3 position)
    {
        MakeMissFont(position);
    }

    public void ShowDamage(Vector3 position, double damage, bool isCitical, double additionalDamage = double.NaN)
    {
        MakeDamageFont(position, damage, isCitical, additionalDamage);
    }

    void MakeMissFont(Vector3 position)
    {
        string name = "CriticalDamageFont";
        string path = string.Format("{0}/{1}.prefab", prefix, name);
        StartCoroutine(Res.InstantiateAssetAsCoroutineThen<TextMeshProUGUI>(
            path,
            transform,
            (prefab) =>
            {
                prefab.text = "Miss";

                Vector3 fontWorldPosition = position +
                    new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.0f, 1.0f), Random.Range(-0.5f, 0.5f));

                prefab.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                prefab.rectTransform.position = Root.World.WorldCamera.WorldToScreenPoint(fontWorldPosition);
                prefab.rectTransform.localScale = new Vector3(3.0f, 3.0f, 1);

                prefab.rectTransform.DOMoveY(prefab.rectTransform.position.y + 20, 1.0f);
                prefab.rectTransform.DOScale(Vector3.one * 2, 0.05f);
                DOTween.ToAlpha(() => prefab.color, p => prefab.color = p, 0.0f, 1.0f);

                StartCoroutine(Cor_Destroy(prefab.gameObject, 1));
            },
            () =>
            {
            }));

        //DOTween.To(() => position, x => myText.rectTransform.position = x, )
    }

    string prefabName = "NormalDamageFont";
    void MakeDamageFont(Vector3 position, double damage, bool isCritical, double additionalDamage)
    {
        string path = string.Format("{0}/{1}.prefab", prefix, prefabName);
        StartCoroutine(Res.InstantiateAssetAsCoroutineThen<DamageFont>(
            path, 
            transform, 
            (prefab) =>
            {
                prefab.SetDamage(damage, isCritical, additionalDamage);

                prefab.Animate(position);

                StartCoroutine(Cor_Destroy(prefab.gameObject, 1));
            },
            () =>
            {
            }));

        //DOTween.To(() => position, x => myText.rectTransform.position = x, )
    }

    IEnumerator Cor_Destroy(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(obj);
    }
}
