using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity
{
    public class BossHpbar : MonoBehaviour
    {
        private Slider _hpFrontBar;
        private Image _hpBackBar;
        private Text _hpText;

        private double _hpMax;
        private double _currentHp;

        private IEnumerator _frontCo;
        private IEnumerator _backCo;

        void Start()
        {
            _hpFrontBar = transform.GetComponent<Slider>();
            _hpBackBar = transform.GetChild(0).GetChild(1).GetComponent<Image>();
            _hpText = transform.GetChild(1).GetComponent<Text>();

            //BattleManager.Instance.BossHpbar = this;
            //this.gameObject.SetActive(false);
        }

        public void SetBossHpBar(double hpMax)
        {
            _hpMax = hpMax;
            _hpFrontBar.value = 1;
            _hpBackBar.fillAmount = 1;

            _hpText.text = hpMax.ToString();

            this.gameObject.SetActive(true);
        }

        public void DamageBossHpBar(double currentHp)
        {
            _currentHp = currentHp;
            if (_currentHp <= 0)
            {
                _currentHp = 0;
                StartCoroutine(DeleteBossHpBar());
            }

            if (_frontCo == null)
            {
                _frontCo = FrontHpControl();
                StartCoroutine(_frontCo);
            }

            if (_backCo == null)
            {
                _backCo = BackHpControl();
                StartCoroutine(_backCo);
            }

            _hpText.text = _currentHp.ToString();
        }

        IEnumerator DeleteBossHpBar()
        {
            yield return new WaitForSeconds(2.0f);

            this.gameObject.SetActive(false);
        }

        IEnumerator FrontHpControl()
        {
            while (_hpFrontBar.value != 0 && (_currentHp / _hpMax) != _hpFrontBar.value)
            {
                double per = _currentHp / _hpMax;
                if (per >= _hpFrontBar.value - 0.01f)
                {
                    _hpFrontBar.value = (float)per;
                }

                _hpFrontBar.value = Mathf.Lerp(_hpFrontBar.value, (float)per, 0.2f);

                yield return new WaitForSeconds(0.03f);
            }

            StopCoroutine(_frontCo);
            _frontCo = null;
        }

        IEnumerator BackHpControl()
        {
            yield return new WaitForSeconds(0.3f);

            while (_hpBackBar.fillAmount != 0 && _hpBackBar.fillAmount != _hpFrontBar.value)
            {
                if (_hpFrontBar.value >= _hpBackBar.fillAmount - 0.01f)
                {
                    _hpBackBar.fillAmount = _hpFrontBar.value;
                }
                else
                {
                    _hpBackBar.fillAmount = Mathf.Lerp(_hpBackBar.fillAmount, _hpFrontBar.value, 0.25f);
                }

                yield return new WaitForSeconds(0.03f);
            }

            StopCoroutine(_backCo);
            _backCo = null;
        }
    }
}