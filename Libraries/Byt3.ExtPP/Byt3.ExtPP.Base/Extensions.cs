﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Byt3.ExtPP.Base
{
    /// <summary>
    /// The Extension class contains a multitude of useful operations on arrays and strings.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// String builder to be able to concat long arrays faster
        /// </summary>
        private static readonly StringBuilder Sb = new StringBuilder();

        /// <summary>
        /// Concats the array into a string separated by the separator
        /// </summary>
        /// <param name="arr">The array to be unpacked</param>
        /// <param name="separator">the separator that will be used when unpacking</param>
        /// <returns></returns>
        public static string Unpack(this IEnumerable<object> arr, string separator)
        {
            if (arr == null || !arr.Any())
            {
                return string.Empty;
            }
            Sb.Clear();
            object[] enumerable = arr as object[] ?? arr.ToArray();
            if (enumerable.Length == 0)
            {
                return string.Empty;
            }
            for (int i = 0; i < enumerable.Count(); i++)
            {
                Sb.Append(enumerable.ElementAt(i));
                if (i < enumerable.Count() - 1)
                {
                    Sb.Append(separator);
                }
            }

            return Sb.ToString();
        }

        /// <summary>
        /// Turns a string into an array split by the separator
        /// </summary>
        /// <param name="arr">The array to be packed</param>
        /// <param name="separator">the separator that will be used when packing</param>
        /// <returns></returns>
        public static IEnumerable<string> Pack(this string arr, string separator)
        {
            return arr.Split(new[] {separator}, StringSplitOptions.None);
        }


        /// <summary>
        /// Creates a sub array starting from start to start+length
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="arr">Source array</param>
        /// <param name="start">start index</param>
        /// <param name="length">length</param>
        /// <returns></returns>
        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> arr, int start, int length)
        {
            T[] ret = new T[length];
            T[] enumerable = arr as T[] ?? arr.ToArray();
            for (int i = start; i < start + length; i++)
            {
                ret.SetValue(enumerable.ElementAt(i), i - start);
            }

            return ret;
        }

        /// <summary>
        /// Creates a sub array starting from 0 to length
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="arr">Source array</param>
        /// <param name="length">length</param>
        /// <returns></returns>
        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> arr, int length)
        {
            return SubArray(arr, 0, length);
        }


        /// <summary>
        /// Smart way to determine if a char sequence contains only digits
        /// </summary>
        /// <param name="str">string to be checked</param>
        /// <returns>true if the string only contains chars from '0' to '9'</returns>
        public static bool IsAllDigits(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            return str.All(char.IsDigit);
        }
    }
}