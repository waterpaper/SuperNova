using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class PopupAlert : UIPopup<PopupAlertArgs>
    {
        [SerializeField]
        private Text textContent;
        [SerializeField]
        private Button buttonOK;

        public override void Init(PopupAlertArgs args)
        {
            textContent.text = args.Content;

            buttonOK.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    this.Close();
                })
                .AddTo(this);
        }
    }

    public class PopupAlertArgs : IPopupArgs
    {
        public string PrefabName => "Alert";
        public string Content { get; }

        public PopupAlertArgs(string content)
        {
            Content = content;
        }
    }
}