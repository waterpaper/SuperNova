using Supernova.Api;
using UnityEngine;

namespace Supernova.Unity
{
    /// <summary>
    /// 필요한 데이터를 미리 정의하고 처리합니다.
    /// </summary>
    public class Root : MonoBehaviour
    {
        public static SceneManager SceneManager { get; protected set; }
        public static PopupManager PopupManager { get; protected set; }
        public static NetworkManager NetworkManager { get; protected set; }
        public static World World { get; protected set; }
        public static GameInfo GameInfo { get; set; }
        public static State State { get; protected set; }
        public static EventListeners EventListeners { get; protected set; }


        private void Awake()
        {
            World = GameObject.Find("WORLD").GetComponent<World>();
            SceneManager = GameObject.Find("UICANVAS").transform.GetChild(0).GetComponent<SceneManager>();
            PopupManager = GameObject.Find("UICANVAS").transform.GetChild(1).GetComponent<PopupManager>();
            NetworkManager = new();
            NetworkManager.Initialize();
            EventListeners = new();

            // State 초기화
            State = new State(new MainStateLogic(), Root.World);
        }

        private void OnDestroy()
        {
            NetworkManager.Destroy();
        }
    }
}