using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Supernova.Utils;
using Cysharp.Threading.Tasks;

namespace Supernova.Unity.UI
{
    public class SceneIngame : UIScene
    {
        [SerializeField]
        private Button buttonHitGold, buttonSettings;
        //[SerializeField]
        //private SkillSlot[] skillSlots;

        public GameObject projectile;

        public GameObject loading;
        public Image loadingBar;

        private void Awake()
        {
            Bind();
            InitGamePrefab().Forget();
        }

        private void OnDestroy()
        {
            UnBind();
        }

        private void Bind()
        {
            // 초당 기본 골드 지급
            Observable.Interval(System.TimeSpan.FromSeconds(1.0)).Subscribe(_ => Root.State.Logic.AddIdleGold()).AddTo(this);
            Observable.Interval(System.TimeSpan.FromSeconds(10.0)).Subscribe(_ => Root.State.Save()).AddTo(this);

            // 클릭 시 골드 지급
            buttonHitGold.OnClickAsObservable().Subscribe(_ => {
                double jackpot = Root.State.FinalStat.Value.LuckRate;
                double rate = Random.Range(0.0f, 1.0f);

                bool isJackpot = false;
                if (rate < jackpot)
                {
                    isJackpot = true;
                }

                Root.State.Logic.AddHitGold(isJackpot);
                UIParticle.Instance.GoldParticle(isJackpot);
            }).AddTo(this);

            buttonSettings.OnClickAsObservable().Subscribe(_ => { Root.PopupManager.ShowPopup(new PopupSettingsArgs()); }).AddTo(this);

            // 최종 스탯 변경됨
            Root.State.FinalStat
                .Subscribe(finalStat =>
                {
                    Log.Info($"스탯 변경 됨.\n  공격력: {finalStat.Attack}\n  공격속도: {finalStat.AttackSpeed}");
                })
                .AddTo(this);
        }

        private void UnBind()
        {
            buttonHitGold.onClick.RemoveAllListeners();
            buttonSettings.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 필요 데이터를 로드하며 완료 될때까지 로딩화면을 표현합니다.
        /// </summary>
        private async UniTask InitGamePrefab()
        {
            loadingBar.fillAmount = 0;

            GameObject temp = null;
            loadingBar.fillAmount += 0.1f;

            temp = null;
            await Res.InstantiateAssetAsCoroutineThen<Transform>(BattleConfig.INGAME, Root.World.transform, (prefab) =>
            {
                temp = prefab.gameObject;
            });
            loadingBar.fillAmount += 0.2f;

            await Ingame.Battle.Load(temp.transform);
            loadingBar.fillAmount += 0.2f;

            await Res.InstantiateAssetAsCoroutineThen<Transform>(BattleConfig.TILEMANAGER, Root.World.transform, (prefab) =>
            {
                temp = prefab.gameObject;
                prefab.tag = "Environment";
            });
            loadingBar.fillAmount += 0.4f;

            var tileManager = temp.GetComponent<TileManager>();
            await tileManager.CreateMap();
            tileManager.StartSetting();

            Ingame.Battle.Move.Subscribe((value) =>
            {
                tileManager.MapMove(value);
            });

            loadingBar.fillAmount = 1;
            loading.SetActive(false);
            Log.Info("프리팹이 생성되었습니다.");

            await UniTask.WaitForSeconds(0.3f);
            Ingame.Battle.GameStart();
        }
    }
}
