using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Supernova.Utils;

namespace Supernova.Unity.UI
{
    public class SceneMain : UIScene
    {
        [SerializeField]
        private Slider sliderProgress;

        private void Awake()
        {
            this.DownloadAssetsAsCoroutine();
        }

        /// <summary>
        /// 초기 addressable 로드 및 json데이터를 로드합니다.(초기 데이터)
        /// </summary>
        private async void DownloadAssetsAsCoroutine()
        {
            // Res 초기화
            await Res.InitAsCoroutine().ToUniTask();

            // 에셋 다운로드
            Log.Info("에셋 다운로드 시작");
            var loading = Res.DownloadAssetsAsAsyncOperation("default");
            while (!loading.IsDone)
            {
                this.sliderProgress.value = loading.GetDownloadStatus().Percent;
                await UniTask.WaitForSeconds(0.1f);
            }
            Log.Info("에셋 다운로드 완료");

            // 게임 정의 초기화
            await Res.LoadAssetAsCoroutineThen<TextAsset>("Assets/res/data/gameInfo.json", textAsset =>
            {
                var text = textAsset.text;
                Log.Info(text);
                Root.GameInfo = GameInfo.GameParseFrom(new GameInfo(), text);
            });

            await Res.LoadAssetAsCoroutineThen<TextAsset>("Assets/res/data/prefabInfo.json", textAsset =>
            {
                var text = textAsset.text;
                Log.Info(text);
                Root.GameInfo = GameInfo.PrefabParseFrom(Root.GameInfo, text);
            });

            Root.SceneManager.LoadScene("Login");
        }
    }
}