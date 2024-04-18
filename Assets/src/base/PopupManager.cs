using System.Collections;
using UnityEngine;
using Supernova.Utils;

namespace Supernova.Unity
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField]
        private string prefix = "Assets/res/popup";
        [SerializeField]
        private RectTransform blinder;
        private int popupCount = 0;

        public void ShowPopup<TPopupArgs>(TPopupArgs args)
            where TPopupArgs : IPopupArgs
        {
            StartCoroutine(this.ShowPopupAsCoroutine(args));
        }

        public IEnumerator ShowPopupAsCoroutine<TPopupArgs>(TPopupArgs args)
            where TPopupArgs : IPopupArgs
        {
            var path = $"{prefix}{args.PrefabName}.prefab";
            UIPopup<TPopupArgs> newPopup = null;
            yield return Res.InstantiateAssetAsCoroutineThen<UIPopup<TPopupArgs>>(path, this.transform, (popup) =>
            {
                newPopup = popup;
                popup.Init(args);
            });

            if (newPopup == null)
            {
                Log.Error("Failed to load popup. ({0})", path);
                yield break;
            }

            this.popupCount++;
            ApplyBlinder();
            newPopup.gameObject.SetActive(true);

            yield return new WaitUntil(() => newPopup.IsClosed);
            newPopup.gameObject.Release();
            this.popupCount--;
            ApplyBlinder();
        }

        private void ApplyBlinder()
        {
            blinder.gameObject.SetActive(this.popupCount > 0);
            blinder.SetSiblingIndex(popupCount - 1);
        }
    }
}