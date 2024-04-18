using System;
using System.Collections.Generic;

namespace Supernova.Utils
{
    /// <summary>
    /// unique id�� �����ϴ� Ŭ�����Դϴ�.
    /// </summary>
    public static class GenID
    {
        private static readonly Dictionary<Type, int> idMap = new();

        /// <summary>
        /// �ش� Ÿ���� ��ġ�� �ʰ� Unique�� ID�� �����մϴ�.
        /// </summary>
        /// <typeparam name="T">Ÿ��</typeparam>
        /// <returns>������ Unique ID</returns>
        public static int Get<T>()
        {
            var type = typeof(T);
            if (!idMap.TryGetValue(type, out var id))
                idMap.Add(type, 0);

            idMap[type] = id + 1;
            return idMap[type];
        }
    }
}