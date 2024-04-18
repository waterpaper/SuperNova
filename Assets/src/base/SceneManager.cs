using System.Collections;
using UnityEngine;
using Supernova.Utils;

namespace Supernova.Unity
{
    /// <summary>
    /// 씬 프리팹으로 화면 이동을 처리합니다.
    /// </summary>
    public class SceneManager : MonoBehaviour
    {
        [SerializeField]
        private string prefix = "Assets/res/scene";
        [SerializeField]
        private UIScene mainScene;
        [SerializeField]
        private GameObject loadingScreen;

        private UIScene currentScene;

        private void Awake()
        {
            var clone = Instantiate(mainScene, transform);
            currentScene = clone;
        }

        public void LoadScene(string prefabName)
        {
            StartCoroutine(this.LoadSceneAsCoroutine(prefabName));
        }

        public IEnumerator LoadSceneAsCoroutine(string prefabName)
        {
            loadingScreen?.SetActive(true);

            var path = $"{prefix}{prefabName}.prefab";
            UIScene newScene = null;
            yield return Res.InstantiateAssetAsCoroutineThen<UIScene>(path, this.transform, (scene) => newScene = scene );

            if (newScene == null)
            {
                Log.Error("Failed to load scene. ({0})", path);
                yield break;
            }

            newScene.gameObject.SetActive(true);
            DestroyImmediate(currentScene.gameObject);
            currentScene = newScene;

            loadingScreen?.SetActive(false);
        }
    }
}