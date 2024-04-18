using System.Collections.Generic;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Supernova.Api
{
    public class TestMessage
    {
        public bool isDone = false;
        public string res = null;
        public UnityWebRequest req;

        public TestMessage(UnityWebRequest req)
        {
            this.req = req;
        }

        public async UniTask Done(CancellationToken token)
        {
            await UniTask.WaitUntil(() => isDone || token.IsCancellationRequested);
        }
    }

    public class DummyServer
    {
        private CancellationToken token;
        private DummyUserData dummyDBData;
        private List<TestMessage> messageQueue;

        public DummyServer(CancellationToken token)
        {
            dummyDBData = new();
            messageQueue = new();
            this.token = token;

            Update().Forget();
            dummyDBData.LoadAsync().Forget();
        }

        public async UniTaskVoid Update()
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                    break;

                for (int i = 0; i < messageQueue.Count; i++)
                {
                    MessageParsing(messageQueue[i]);
                    messageQueue.Remove(messageQueue[i]);
                    break;
                }

                await UniTask.Delay(200);
            }
        }

        public async UniTask<string> Send(UnityWebRequest req)
        {
            var testMessage = new TestMessage(req);
            messageQueue.Add(testMessage);

            await testMessage.Done(token);
            return testMessage.res;
        }

        private void MessageParsing(TestMessage reqMessage)
        {
            string method = reqMessage.req.method;
            string path = reqMessage.req.url;
            string text = reqMessage.req.uploadHandler == null ? null : new System.Text.UTF8Encoding().GetString(reqMessage.req.uploadHandler.data);

            reqMessage.res = DummyServerService.ListenMessage(dummyDBData, method, path, text);
            reqMessage.isDone = true;
        }
    }
}