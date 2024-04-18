using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class PopupAccessorySlot : UIPopup<PopupAccessorySlotArgs>
    {
        [SerializeField]
        private Button buttonL, buttonR;
        [SerializeField]
        private RawImage accLIcon, accRIcon;
        [SerializeField]
        private Button buttonNo;

        public override void Init(PopupAccessorySlotArgs args)
        {
            buttonL.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    args.OnClose(PopupAccessorySlotResult.Left);
                    this.Close();
                })
                .AddTo(this);

            buttonR.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    args.OnClose(PopupAccessorySlotResult.Right);
                    this.Close();
                })
                .AddTo(this);

            buttonNo.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    args.OnClose(PopupAccessorySlotResult.Cancel);
                    this.Close();
                })
                .AddTo(this);

            var accl = Root.State.Inventory.Items.FirstOrDefault(p => p.Guid == Root.State.Inventory.AccessoryLeft.Value);
            if (accl != null)
            {
                StartCoroutine(Res.LoadAssetAsCoroutineThen<Texture2D>(Constants.GetItemIconResPath(accl.ItemID), texture =>
                {
                    accLIcon.texture = texture;
                }));
            }

            var accr = Root.State.Inventory.Items.FirstOrDefault(p => p.Guid == Root.State.Inventory.AccessoryRight.Value);
            if (accr != null)
            {
                StartCoroutine(Res.LoadAssetAsCoroutineThen<Texture2D>(Constants.GetItemIconResPath(accr.ItemID), texture =>
                {
                    accRIcon.texture = texture;
                }));
            }
        }
    }

    public class PopupAccessorySlotArgs : IPopupArgs
    {
        public string PrefabName => "AccessorySlot";
        public Action<PopupAccessorySlotResult> OnClose { get; }

        public PopupAccessorySlotArgs(Action<PopupAccessorySlotResult> onClose)
        {
            OnClose = onClose;
        }
    }

    public enum PopupAccessorySlotResult
    {
        Cancel,
        Left,
        Right,
    }
}