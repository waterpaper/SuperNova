using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class PopupSettings : UIPopup<PopupSettingsArgs>
    {
        [SerializeField]
        private Button buttonHighQuality, buttonBgm, buttonSfx, buttonReset;
        [SerializeField]
        private Text textHighQuality, textBgm, textSfx;
        [SerializeField]
        private Button buttonOK;

        private readonly ReactiveProperty<bool> highQuality = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> bgm = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> sfx = new ReactiveProperty<bool>();

        public override void Init(PopupSettingsArgs args)
        {
            buttonOK.OnClickAsObservable().Subscribe(_ => { this.Close(); }).AddTo(this);

            highQuality.Value = GameSettings.HighQuality;
            bgm.Value = GameSettings.Bgm;
            sfx.Value = GameSettings.Sfx;

            highQuality
                .Subscribe(p =>
                {
                    PlayerPrefs.SetInt("setting.hq", p ? 1 : 0);
                    textHighQuality.text = p ? "켜짐" : "꺼짐";
                })
                .AddTo(this);

            bgm
                .Subscribe(p =>
                {
                    PlayerPrefs.SetInt("setting.bgm", p ? 1 : 0);
                    textBgm.text = p ? "켜짐" : "꺼짐";
                })
                .AddTo(this);

            sfx
                .Subscribe(p =>
                {
                    PlayerPrefs.SetInt("setting.sfx", p ? 1 : 0);
                    textSfx.text = p ? "켜짐" : "꺼짐";
                })
                .AddTo(this);

            buttonHighQuality
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    highQuality.Value = !highQuality.Value;
                })
               .AddTo(this);

            buttonBgm
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    bgm.Value = !bgm.Value;
                })
               .AddTo(this);

            buttonSfx
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    sfx.Value = !sfx.Value;
                })
               .AddTo(this);

            buttonReset
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    Root.PopupManager.ShowPopup(new PopupYesNoArgs("게임 데이터를 삭제하면 게임을 다시 실행해야합니다. 계속하시겠습니까?", yes =>
                    {
                        if (!yes) { return; }

                        Root.State.Reset();
                        Application.Quit();
                    }));
                })
                .AddTo(this);
        }
    }

    public class PopupSettingsArgs : IPopupArgs
    {
        public string PrefabName => "Settings";

        public PopupSettingsArgs()
        {
        }
    }
}