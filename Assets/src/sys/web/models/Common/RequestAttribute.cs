using System;

namespace Supernova.Api
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequestAttribute : Attribute
    {
        public static string GET = "GET";
        public static string POST = "POST";
        public static string DELETE = "delete";

        public static string JSON = "application/json";
        public static string FORM = "application/x-www-form-urlencoded";


        public string Path { get; protected set; }
        public string Method { get; protected set; }
        public string ContentType { get; protected set; }
        public string BytesName { get; protected set; }

        /// <summary>
        /// 리퀘스트 속성을 지정합니다.
        /// </summary>
        /// <param name="path"> 리퀘스트를 보내는 링크 주소 입니다. </param>
        /// <param name="method"> 데이터의 메소드 입니다. </param>
        /// <param name="contentType"> 보내는 콘텐츠의 타입입니다. </param>
        /// <param name="bytesName"> 보내는 콘텐츠의 타입을 지정하지 않았는데, 값을 Byte형식으로 보내려면 이 속성을 사용하는 클래스 맴버 변수중 Byte[]를 가진 필드의 이름 입니다. </param>
        public RequestAttribute(string path, NetworkMethod method, NetworkContentType contentType = NetworkContentType.Json)
        {
            Path = path;
            Method = method switch
            {
                NetworkMethod.Get => GET,
                NetworkMethod.Post => POST,
                NetworkMethod.Delete => DELETE,
                _ => throw new ArgumentOutOfRangeException(nameof(Method), Method, null),
            };

            ContentType = contentType switch
            {
                NetworkContentType.Json => JSON,
                NetworkContentType.Form => FORM,
                _ => throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null),
            };

            BytesName = null;
        }
    }
}