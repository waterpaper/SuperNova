using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Supernova.Utils;

namespace Supernova.Unity
{
    public class SkillSlot : MonoBehaviour, IPointerDownHandler
    {
        private double cooltime = 0.0, skillCooltime = 1.0;
        private readonly ReactiveProperty<bool> isInvokable = new ReactiveProperty<bool>(false);
        private readonly UnityEvent<long> onInvoke = new UnityEvent<long>();
        [SerializeField]
        private RawImage RawImageIcon;
        [SerializeField]
        private Text TextCooltime;
        [SerializeField]
        private Image ImageFill;
        [SerializeField]
        private bool AutoUse;

        public ReactiveProperty<long> SkillID { get; } = new ReactiveProperty<long>(-1);
        public IReadOnlyReactiveProperty<bool> IsInvokable => isInvokable;

        public IObservable<long> OnInvokeAsObservable() => onInvoke.AsObservable();

        void Awake()
        {
            SkillID
                .Subscribe(_ =>
                {
                    if (SkillID.Value == -1) return;

                    this.cooltime = 0.0;
                    Root.World.StartCoroutine(Res.LoadAssetAsCoroutineThen<Texture2D>(string.Format("Assets/res/img/Skill/Skill_{0}.png", SkillID), texture =>
                    {
                        RawImageIcon.texture = texture;
                    }));
                })
                .AddTo(this);

            Observable.EveryUpdate()
                .Select(_ => Time.deltaTime)
                .Subscribe(dt =>
                {
                    this.isInvokable.Value = this.SkillID.Value != -1 && this.cooltime <= 0.0;
                    this.cooltime = Math.Max(0, this.cooltime - dt);
                    this.TextCooltime.text = this.cooltime > 0.0 ? string.Format("{0:0.0}", this.cooltime) : string.Empty;
                    this.ImageFill.fillAmount = (float)(this.cooltime / skillCooltime);
                })
                .AddTo(this);

            this.isInvokable
                .Where(p => this.AutoUse && p)
                .Subscribe(_ =>
                {
                    this.Invoke();
                })
                .AddTo(this);
        }

        public void Invoke()
        {
            if (!this.IsInvokable.Value) {
                Log.Error($"Skill cannot be invoked during cooltime. (SkillID: {SkillID.Value}, Cooltime: {cooltime})");
            }

            this.onInvoke.Invoke(this.SkillID.Value);
        }

        public void SetCoolDown()
        {
            var skillInfo = Root.GameInfo.SkilInfos[this.SkillID.Value];
            this.cooltime = skillInfo.Cooltime;
            this.skillCooltime = skillInfo.Cooltime;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!this.IsInvokable.Value) { return; }

            this.Invoke();
        }
    }

    public interface ISkillFactory
    {
        ISkill CreateSkill(long skillID);
    }
}
