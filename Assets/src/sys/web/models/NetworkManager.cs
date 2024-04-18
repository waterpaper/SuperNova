using Cysharp.Threading.Tasks;
using Supernova.Api.Rest;

namespace Supernova.Api
{
    /// <summary>
    /// network 송 수신을 도와주는 manager
    /// </summary>
    public partial class NetworkManager
    {
        private readonly string Mode = "easy";

        private NetRestClient restClient;

        public void Initialize()
        {
            restClient = new();
        }

        public void Destroy()
        {
            restClient.Dispose();
        }

        public async UniTask<T> Request<T>(ReqMessage<T> obj) where T : class
        {
            return await restClient.Request(obj);
        }
    }
}
