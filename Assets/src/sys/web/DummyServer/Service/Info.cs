using System;

namespace Supernova.Api
{
    class Info
    {
        public static string ReceiveMessage(DummyUserData dummyData, string method, string path, string data)
        {
            switch (path)
            {
                case "info/load":
                    return PlayerInfoLoadMessage(dummyData, data);
                case "info/save":
                    return PlayerInfoSaveMessage(dummyData, data);
                default:
                    return null;
            }
        }

        private static string PlayerInfoLoadMessage(DummyUserData dummyData, string data)
        {
            Rest.Info.UserGameInfo resMessage = new(dummyData);
            return Utils.Json.ObjectToJson(resMessage);
        }

        private static string PlayerInfoSaveMessage(DummyUserData dummyData, string data)
        {
            dummyData.Save(data);

            Rest.Info.UserGameInfo resMessage = new(dummyData);
            return Utils.Json.ObjectToJson(resMessage);
        }
    }
}