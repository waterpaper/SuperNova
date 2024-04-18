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
        /// ������Ʈ �Ӽ��� �����մϴ�.
        /// </summary>
        /// <param name="path"> ������Ʈ�� ������ ��ũ �ּ� �Դϴ�. </param>
        /// <param name="method"> �������� �޼ҵ� �Դϴ�. </param>
        /// <param name="contentType"> ������ �������� Ÿ���Դϴ�. </param>
        /// <param name="bytesName"> ������ �������� Ÿ���� �������� �ʾҴµ�, ���� Byte�������� �������� �� �Ӽ��� ����ϴ� Ŭ���� �ɹ� ������ Byte[]�� ���� �ʵ��� �̸� �Դϴ�. </param>
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