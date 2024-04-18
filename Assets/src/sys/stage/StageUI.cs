using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity
{
    public class StageUI : MonoBehaviour
    {
        private Text _stageText;

        void Start()
        {
            _stageText = GetComponentInChildren<Text>();

            Root.State.General.Kill.Subscribe(kill => {
                SetText(kill);
            }).AddTo(this);
        }

        void SetText(long stage)
        {
            _stageText.text = string.Format("Stage\n{0} - {1}", stage / 10 + 1, stage % 10 + 1);
        }
    }
}
