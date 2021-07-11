using System;
using System.Linq;

namespace FtpSharp.Server
{
    public static class Extensions
    {
        public static T[] Append<T>(this T[] array, T item)
        {
            if (array == null) {
                return new T[] { item };
            }
            T[] result = new T[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = item;
            return result;
        }

        public static T[] Concatenate<T>(this T[] first, T[] second)
        {
            if (first == null) {
                return second;
            }
            if (second == null) {
                return first;
            }
    
            return first.Concat(second).ToArray();
        }

        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }

        public static string Slice(this string source, int start, int end)
        {
            // Handles negative ends.
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            return source.Substring(start, len);
        }
        
    }
}