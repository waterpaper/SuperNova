using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class ControlCharacter : MonoBehaviour
    {
        [SerializeField]
        private Text textStats;

        private void Awake()
        {
            Root.State.Stat.Select(p => p.ToString().Replace(";", "\n")).Subscribe(p => textStats.text = p).AddTo(this);
        }
    }
}
