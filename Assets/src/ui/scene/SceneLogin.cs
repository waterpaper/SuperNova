using Cysharp.Threading.Tasks;
using Supernova.Utils;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Supernova.Unity.UI
{
    public class SceneLogin : UIScene
    {
        [SerializeField] private Button buttonLoginTester;

        private void Awake()
        {
            Bind();
        }
        private void OnDestroy()
        {
            UnBind();
        }

        private void Bind()
        {
            buttonLoginTester
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    LoginTester().Forget();
                })
                .AddTo(this);
        }

        private void UnBind()
        {
            buttonLoginTester.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 로그인 로직을 처리합니다.
        /// </summary>
        private async UniTaskVoid LoginTester()
        {
            buttonLoginTester.interactable = false;
            var isYes = false;
            await Root.PopupManager.ShowPopupAsCoroutine(new PopupYesNoArgs("테스트 계정으로 로그인 하시겠습니까?", result => isYes = result));
            if (!isYes) {
                buttonLoginTester.interactable = true;
                return; 
            }

            try
            {
                await Root.State.Load();
                Root.SceneManager.LoadScene("Ingame");
            }
            catch
            {
                Log.Warning("데이터 로드에 실패했습니다. 다시 시도해주세요");
                if(buttonLoginTester != null)
                    buttonLoginTester.interactable = true;
            }
        }
    }
}
