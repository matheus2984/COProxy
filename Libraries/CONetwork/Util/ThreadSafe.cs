using System.Collections.Generic;

namespace CONetwork.Util
{
    public static partial class Extentions
    {
        public static void SafeAdd<T>(this List<T> list, T item)
        {
            lock (list)
            {
                list.Add(item);
            }
        }

        public static void SafeRemove<T>(this List<T> list, T item)
        {
            lock (list)
            {
                list.Remove(item);
            }
        }

        public static void ThreadSafeQueue<T>(this Queue<T> queue, T value)
        {
            lock (queue)
            {
                queue.Enqueue(value);
            }
        }

        public static T ThreadSafeDequeue<T>(this Queue<T> queue)
        {
            lock (queue)
            {
                if (queue.Count > 0)
                    return queue.Dequeue();
                return default(T);
            }
        }

        public static void ThreadSafeAdd<T, T2>(this Dictionary<T, T2> dictionary, T key, T2 value)
        {
            lock (dictionary)
            {
                if (dictionary.ContainsKey(key))
                    dictionary[key] = value;
                else
                    dictionary.Add(key, value);
            }
        }

        public static void ThreadSafeRemove<T, T2>(this Dictionary<T, T2> dictionary, T key)
        {
            lock (dictionary)
            {
                dictionary.Remove(key);
            }
        }

        public static T2[] ThreadSafeValueArray<T, T2>(this Dictionary<T, T2> dictionary)
        {
            lock (dictionary)
            {
                var values = new T2[dictionary.Count];
                dictionary.Values.CopyTo(values, 0);
                return values;
            }
        }
    }
}