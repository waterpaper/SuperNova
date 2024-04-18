using System;

namespace Supernova.Api
{
    public class DummyServerService
    {
        public static string ListenMessage(DummyUserData dummyData, string method, string path, string data)
        {
            /*if (path.Contains($"{NetRestClient.PREFIX}/"))
            {
                path = path.Replace($"{NetRestClient.PREFIX}/", null);
            }*/

            if (path.Contains($"{NetRestClient.TARGET_URL}/"))
            {
                path = path.Replace($"{NetRestClient.TARGET_URL}/", null);
            }

            if (path[0].Equals('/'))
            {
                path = path.Remove(0, 1);
            }

            var paths = path.Split('/');
            string router = paths.Length != 0 ? paths[0] : "";

            switch (router)
            {
                case "info":
                    return Info.ReceiveMessage(dummyData, method, path, data);
                default:
                    return null;
            }
        }
    }
}