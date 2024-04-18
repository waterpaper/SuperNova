using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Supernova.Api.Rest;

namespace Supernova.Api
{
    public partial class NetRestClient
    {
        public static readonly string TARGET_URL = "localhost:3001";
        public static readonly string PREFIX = "";

        private static bool HasInternet => Application.internetReachability != NetworkReachability.NotReachable;

        public static event Action OnInternetDisconnection;

        private DummyServer dummyServer;
        private CancellationTokenSource tokenSource;

        public NetRestClient()
        {
            tokenSource = new();
            dummyServer = new(tokenSource.Token);
        }

        public void Dispose()
        {
            tokenSource.Cancel();
        }

        #region API Methods
        private string URL(string path)
        {
            return string.Format("{0}/{1}", TARGET_URL, path);
        }

        private void CheckAuthoriztion(UnityWebRequest r, string token)
        {
            if (token != string.Empty)
            {
                r.SetRequestHeader("Authorization", string.Format("Bearer {0}", token));
            }
        }

        public async UniTask<T> Request<T>(ReqMessage<T> obj, string token = null) where T : class
        {
            if (!HasInternet)
            {
                Debug.LogWarning("[WebCommunicator] Internet Disconnection");
                OnInternetDisconnection?.Invoke();
                throw new InternetDisconnectionException();
            }

            var type = obj.GetType();
            var att = type.GetCustomAttribute<RequestAttribute>();
            var url = URL(att.Path);
            var method = att.Method;
            var timeoutController = new TimeoutController(tokenSource);

            try
            {
                using (var request = MethodWebRequest(method, url))
                {
                    if (!string.IsNullOrEmpty(token))
                        CheckAuthoriztion(request, token);

                    if (obj != null)
                    {
                        request.SetRequestHeader("Content-Type", att.ContentType);
                        if (att.ContentType == RequestAttribute.JSON)
                            request.uploadHandler = new UploadHandlerRaw(ToJsonBytes(obj));
                        else if (att.ContentType == RequestAttribute.FORM)
                            request.uploadHandler = new UploadHandlerRaw(ToFormBytes(obj));
                    }

                    request.downloadHandler = new DownloadHandlerBuffer();
                    //var oper = await request.SendWebRequest().WithCancellation(timeoutController.Timeout(30000));

                    /*var oper = request.SendWebRequest();
                    while (!oper.isDone)
                        await UniTask.Yield();*/
                    //
                    var text = await dummyServer.Send(request);
                    if (tokenSource.IsCancellationRequested)
                        throw new NetworkException(0, "cancel");
                    else if (text == null)
                        throw new NetworkException(0, "error");
                    return obj.ResMessage(text);
                    //

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"{method} network code : {request.responseCode}\nmessage : {request.downloadHandler.text}");
                        return obj.ResMessage(request.downloadHandler.text);
                    }

                    int errorCode = (int)request.responseCode;
                    var resErr = Utils.Json.JsonToObject<Rest.ResError>(request.downloadHandler.text);
                    Debug.LogError($"network error code : {errorCode}\nmessage : {resErr.message}");

                    request.disposeUploadHandlerOnDispose = true;
                    request.disposeDownloadHandlerOnDispose = true;
                    request.Dispose();
                    throw new NetworkException(errorCode, resErr.message);
                }
            }
            catch (OperationCanceledException) when (timeoutController.IsTimeout())
            {
                timeoutController.Dispose();
                throw new NetworkException(0, "Timeout");
            }
            catch (UnityWebRequestException e)
            {
                timeoutController.Dispose();
                /*var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(e.Text);
                var message = json.Values.ElementAt(0);
                FConsole.LogWarning($"[WebCommunicator] Response({requestIndex}) Error: {e.Error}\n");
                ResponseInfo key = new(e.ResponseCode, message);

                if (RequestErrorDefine.TryGetValue(key, out var msg))
                {
                    throw new WebRequestException(msg, key);
                }*/

                throw new NetworkException((int)e.ResponseCode, $"정의 되지 않는 예외입니다.");
            }
            catch (NetworkException e)
            {
                throw e;
            }
        }

        private UnityWebRequest MethodWebRequest(string method, string url, string form = null)
        {
            if (method == RequestAttribute.GET)
            {
                return UnityWebRequest.Get(url);
            }
            else if (method == RequestAttribute.POST)
            {
                return UnityWebRequest.Post(url, form);
            }
            else if (method == RequestAttribute.DELETE)
            {
                return UnityWebRequest.Delete(url);
            }
            else if (method == "PUT")
            {
                return UnityWebRequest.Put(url, form);
            }

            return new UnityWebRequest();
        }

        private byte[] ToJsonBytes<T>(T obj) where T : class
        {
            var json = Utils.Json.ObjectToJson(obj);
            return new UTF8Encoding().GetBytes(json);
        }

        private Byte[] ToFormBytes(object obj)
        {
            var json = Utils.Json.ObjectToJson(obj);
            var dic = ToDictionary(json);
            var form = new WWWForm();
            foreach (var pair in dic)
            {
                form.AddField(pair.Key, pair.Value.ToString());
            }
            return form.data;
        }
        private Dictionary<string, object> ToDictionary(object obj)
        {
            FieldInfo[] infos = obj.GetType().GetFields();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (FieldInfo info in infos)
            {
                dic.Add(info.Name, info.GetValue(obj));
            }

            return dic;
        }
        #endregion
    }

}
