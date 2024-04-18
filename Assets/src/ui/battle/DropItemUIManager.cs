using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Supernova.Utils;

namespace Supernova.Unity
{
    public class DropItemUIManager : MonoBehaviour
    {
        public string goldFontPath = "Assets/res/prefab/game/dropItem/Font/GoldFont.prefab";
        public string soulFontPath = "Assets/res/prefab/game/dropItem/Font/SoulFont.prefab";
        public string itemAlertPath = "Assets/res/prefab/game/dropItem/ItemAlert.prefab";

        public GameObject goldPosition;
        public GameObject soulPosition;

        [SerializeField]
        private List<GameObject> goldList;
        [SerializeField]
        private List<GameObject> soulList;
        [SerializeField]
        private List<GameObject> itemList;

        private GameObject goldParent;
        private GameObject soulParent;
        private GameObject itemParent;

        private RectTransform goldRectTrans;
        private RectTransform soulRectTrans;


        private void Start()
        {
            goldList = new List<GameObject>();
            soulList = new List<GameObject>();
            itemList = new List<GameObject>();

            goldRectTrans = goldPosition.GetComponent<RectTransform>();
            soulRectTrans = soulPosition.GetComponent<RectTransform>();

            goldParent = transform.GetChild(0).gameObject;
            soulParent = transform.GetChild(1).gameObject;
            itemParent = transform.GetChild(2).gameObject;
        }

        public void UICreate(int kind, long value)
        {
            if (kind == 1 && value > 0)
            {
                StartCoroutine(CreateGold(value));
            }
            else if (kind == 2 && value > 0)
            {
                StartCoroutine(CreateSoul(value));
            }
            else if(kind == 3)
            {
                StartCoroutine(CreateItem(value));
            }
        }

        IEnumerator CreateGold(long value)
        {
            bool isSetting = false;

            for (int i = 0; i < goldList.Count; i++)
            {
                if (goldList[i].activeSelf == false)
                {
                    Vector2 pos = goldPosition.GetComponent<RectTransform>().anchoredPosition + new Vector2(goldRectTrans.rect.width - 10, -40);
                    goldList[i].GetComponent<RectTransform>().anchoredPosition = pos;

                    goldList[i].GetComponent<Text>().text = "+ " + value;
                    goldList[i].SetActive(true);
                    isSetting = true;
                    break;
                }
            };

            if (isSetting == false)
            {
                yield return Res.InstantiateAssetAsCoroutineThen<Transform>(goldFontPath, goldParent.transform, (prefab) =>
                {
                    Vector2 pos = goldPosition.GetComponent<RectTransform>().anchoredPosition + new Vector2(goldRectTrans.rect.width -10, -40);
                    prefab.GetComponent<RectTransform>().anchoredPosition = pos;

                    prefab.GetComponent<Text>().text = "+ " + value;
                    prefab.gameObject.SetActive(true);
                    goldList.Add(prefab.gameObject);
                },
                        () =>
                        {
                            Log.Warning("goldUI instantiate fail.");
                        }

                    );
            }

            Log.Info(goldPosition.GetComponent<RectTransform>().anchoredPosition);
            Log.Info(goldRectTrans.rect.width);

            yield return null;
        }

        IEnumerator CreateSoul(long value)
        {
            bool isSetting = false;

            for (int i = 0; i < soulList.Count; i++)
            {
                if (soulList[i].activeSelf == false)
                {
                    Vector2 pos = soulPosition.GetComponent<RectTransform>().anchoredPosition + new Vector2(soulRectTrans.rect.width - 30, -40);
                    soulList[i].GetComponent<RectTransform>().anchoredPosition = pos;

                    soulList[i].GetComponent<Text>().text = "+ " + value;
                    soulList[i].SetActive(true);
                    isSetting = true;
                    break;
                }
            };

            if (isSetting == false)
            {
                yield return Res.InstantiateAssetAsCoroutineThen<Transform>(soulFontPath, soulParent.transform, (prefab) =>
                {
                    Vector2 pos = soulPosition.GetComponent<RectTransform>().anchoredPosition + new Vector2(soulRectTrans.rect.width - 30, -40);
                    prefab.GetComponent<RectTransform>().anchoredPosition = pos;
                    prefab.GetComponent<Text>().text = "+ " + value;
                    prefab.gameObject.SetActive(true);
                    soulList.Add(prefab.gameObject);
                },
                        () =>
                        {
                            Log.Warning("soulUI instantiate fail.");
                        }

                    );
            }

            Log.Info(soulPosition.transform.position);
            Log.Info(soulRectTrans.rect.width - 20);

            yield return null;
        }

        IEnumerator CreateItem(long value)
        {
            bool isSetting = false;

            for (int i = 0; i < itemList.Count; i++)
            {
                Debug.LogFormat("i: {0}, value: {1}", i, value);
                if (itemList[i].activeSelf == false)
                {
                    itemList[i].GetComponent<DropItemAlert>().Setting(value);
                    isSetting = true;
                    break;
                }
            };

            if (isSetting == false)
            {
                yield return Res.InstantiateAssetAsCoroutineThen<Transform>(itemAlertPath, itemParent.transform, (prefab) =>
                {
                    prefab.GetComponent<DropItemAlert>().Setting(value);
                    itemList.Add(prefab.gameObject);
                },
                        () =>
                        {
                            Log.Warning("soulUI instantiate fail.");
                        }

                    );
            }

            yield return null;
        }
    }
}
