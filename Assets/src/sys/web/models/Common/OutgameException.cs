using System;

namespace Supernova.Api
{
    class NetworkException : Exception
    {
        private int code;

        public NetworkException(int code, string message) : base(message)
        {
            this.code = code;
        }

        public virtual int Code { get => code; }
    }

    public class InternetDisconnectionException : Exception
    {
        public InternetDisconnectionException() { }

        public InternetDisconnectionException(string message)
            : base(message) { }

        public InternetDisconnectionException(string message, Exception inner)
            : base(message, inner) { }
    }
}
