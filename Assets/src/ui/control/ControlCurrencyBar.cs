using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Supernova.Utils;

namespace Supernova.Unity.UI
{
    public class ControlCurrencyBar : MonoBehaviour
    {
        [SerializeField]
        private Text textGold, textSoul, textRuby;
        [SerializeField]
        private Button buttonSoulShop, buttonRubyShop;
        private decimal curGold;

        private void Awake()
        {
            buttonSoulShop
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    Log.Warning("not implemented");
                })
                .AddTo(this);

            buttonRubyShop
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    Log.Warning("not implemented");

                    Log.Warning("아이템을 추가했스빈다.");
                    Root.State.Inventory.Items.Add(ItemInstance.Create(501));
                })
                .AddTo(this);

            Root.State.Currency.Soul.Subscribe(soul => textSoul.text = string.Format("{0:n0}", soul)).AddTo(this);
            Root.State.Currency.Ruby.Subscribe(ruby => textRuby.text = string.Format("{0:n0}", ruby)).AddTo(this);

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    curGold += (Root.State.Currency.Gold.Value - curGold) * 0.25m;
                    textGold.text = string.Format("{0:n0}", curGold);
                })
                .AddTo(this);
        }
    }
}
