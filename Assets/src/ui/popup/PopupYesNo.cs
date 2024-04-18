using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class PopupYesNo : UIPopup<PopupYesNoArgs>
    {
        [SerializeField]
        private Text textContent;
        [SerializeField]
        private Button buttonYes;
        [SerializeField]
        private Button buttonNo;

        public override void Init(PopupYesNoArgs args)
        {
            textContent.text = args.Content;

            buttonYes.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    args.OnClose(true);
                    this.Close();
                })
                .AddTo(this);

            buttonNo.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    args.OnClose(false);
                    this.Close();
                })
                .AddTo(this);
        }
    }

    public class PopupYesNoArgs : IPopupArgs
    {
        public string PrefabName => "YesNo";
        public string Content { get; }
        public Action<bool> OnClose { get; }

        public PopupYesNoArgs(string content, Action<bool> onClose)
        {
            Content = content;
            OnClose = onClose;
        }
    }
}