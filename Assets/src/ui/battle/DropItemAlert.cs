using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace Supernova.Unity
{
    public class DropItemAlert : MonoBehaviour
    {
        private RawImage _itemIcon;
        private Text _text;
        private RectTransform _trans;

        private void Start()
        {
            _itemIcon = transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
            _text = transform.GetChild(1).GetComponent<Text>();

            _trans = GetComponent<RectTransform>();
            _trans.localPosition = new Vector3(0, -90, 0);
            gameObject.SetActive(false);
        }

        public void Setting(long itemCode)
        {
            _trans.localPosition = new Vector3(0, -90, 0);
            gameObject.SetActive(true);

            var itemInfo = Root.GameInfo.ItemInfos[itemCode];
            this._text.text = itemInfo.Name + "를 획득하셧습니다.";
            
            Root.World.StartCoroutine(Res.LoadAssetAsCoroutineThen<Texture2D>(Constants.GetItemIconResPath(itemCode), texture =>
            {
                _itemIcon.texture = texture;
            }));


            _trans.DOLocalMoveY(60, 0.5f)
           .SetEase(Ease.Linear)
           .OnComplete(() =>
           {
               StartCoroutine(StayAlert());
           });
        }

        IEnumerator StayAlert()
        {
            yield return new WaitForSeconds(3.0f);

            _trans.DOLocalMoveY(-90, 0.5f)
          .SetEase(Ease.Linear)
          .OnComplete(() =>
          {
              gameObject.SetActive(false);
              _itemIcon.texture = null;
          });
        }

    }
}
