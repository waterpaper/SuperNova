using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity
{
    public class Hpbar : MonoBehaviour
    {
        private Slider _hpFrontBar;
        private Slider _hpBackBar;

        private double _hpMax;
        private double _currentHp;

        private float _frontTime;
        private float _backTime;

        private IEnumerator _frontCo;
        private IEnumerator _backCo;


        public void SetHpBar(double hpMax)
        {
            if(_hpFrontBar == null)
            {
                _hpFrontBar = transform.GetChild(1).GetComponent<Slider>();
                _hpBackBar = transform.GetChild(0).GetComponent<Slider>();
            }

            _hpMax = hpMax;
            _hpFrontBar.value = 1;
            _hpBackBar.value = 1;

            _frontTime = 0;
            _backTime = 0;
        }

        public void DamageHpBar(double currentHp)
        {
            _currentHp = currentHp;

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

            while (_hpBackBar.value != 0 && _hpBackBar.value != _hpFrontBar.value)
            {
                if (_hpFrontBar.value >= _hpBackBar.value - 0.01f)
                {
                    _hpBackBar.value = _hpFrontBar.value;
                }
                else
                {
                    _hpBackBar.value = Mathf.Lerp(_hpBackBar.value, _hpFrontBar.value, 0.25f);
                }

                yield return new WaitForSeconds(0.03f);
            }

            StopCoroutine(_backCo);
            _backCo = null;
        }
    }
}