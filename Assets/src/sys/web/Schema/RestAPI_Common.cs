using System;

namespace Supernova.Api.Rest
{
    public abstract class ReqMessage<T> where T : class
    {
        public T ResMessage(string message)
        {
            return Utils.Json.JsonToObject<T>(message);
        }
    }

    [Serializable]
    public class ResError
    {
        public string message;
        public string cause;
        public string stack;
    }
}
