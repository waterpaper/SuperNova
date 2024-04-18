using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Supernova.Utils;

namespace Supernova.Unity
{
    /// <summary>
    /// addressable 리소스 로드를 처리합니다.
    /// </summary>
    public static class Res
    {
        public static IEnumerator InitAsCoroutine()
        {
            Log.Info("Addressables - Init...");
            yield return Addressables.InitializeAsync();
            Log.Info("Addressables - Init...Done.");
        }

        public static AsyncOperationHandle DownloadAssetsAsAsyncOperation(string key)
        {
            Log.Info($"Addressables - Download...({key})");
            return Addressables.DownloadDependenciesAsync(key);
            Log.Info("Addressables - Download...Done."); 
        }

        public static IEnumerator LoadAssetAsCoroutineThen<T>(string path, Action<T> onLoaded = null, Action onFailed = null)
        {
            var loading = LoadAssetAsAsyncOperation<T>(path);
            yield return loading;
            if (loading.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(loading.Result);
            }
            else
            {
                Log.Warning("Failed to instantiate asset. ({0})", path);
                onFailed?.Invoke();
            }
        }

        public static void Release(this GameObject gameObject)
        {
            Addressables.ReleaseInstance(gameObject);
        }

        public static AsyncOperationHandle<T> LoadAssetAsAsyncOperation<T>(string path)
        {
            return Addressables.LoadAssetAsync<T>(path);
        }

        public static IEnumerator InstantiateAssetAsCoroutineThen<T>(string path, Transform parent, Action<T> onLoaded = null, Action onFailed = null)
            where T : UnityEngine.Object
        {
            var instantiating = InstantiateAssetAsAsyncOperation(path, parent);
            yield return instantiating;
            if (instantiating.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(instantiating.Result.GetComponent<T>());
            }
            else
            {
                Log.Warning("Failed to instantiate asset. ({0})", path);
                onFailed?.Invoke();
            }
        }

        public static AsyncOperationHandle<GameObject> InstantiateAssetAsAsyncOperation(string path, Transform parent)
        {
            return Addressables.InstantiateAsync(path, parent);
        }
    }
}