using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BMW.Frameworks.LinqHelper
{
	public static class Extensions
	{
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T element in source)
				action(element);
		}

		/// <summary>
		/// 如果类型是Nullable&lt;T&gt;，则返回T，否则返回自身
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetNonNullableType(this Type type)
		{
			if (IsNullableType(type))
			{
				return type.GetGenericArguments()[0];
			}
			return type;
		}

		/// <summary>
		/// 是否Nullable&lt;T&gt;类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsNullableType(this Type type)
		{
			return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		/// <summary>
		/// 获取Lambda表达式的参数表达式
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="S"></typeparam>
		/// <param name="expr"></param>
		/// <returns></returns>
		public static ParameterExpression[] GetParameters<T, S>(this Expression<Func<T, S>> expr)
		{
			return expr.Parameters.ToArray();
		}

        public static bool In<T>(this T t, params T[] c)
        {
            return c.Any(i => i.Equals(t));
        }

        public static bool In<T>(this T t, IEnumerable<T> c)
        {
            return c.Any(i => i.Equals(t));
        }

        public static bool NotIn<T>(this T t, params T[] c)
        {
            return !In(t, c);
        }
        public static bool NotIn<T>(this T t, IEnumerable<T> c)
        {
            return !In(t, c);
        }

	    public static T[] AsArray<T>(this T t)
	    {
	        return new[] {t};
	    }

        public static bool IsBetween<T>(this T t, T lowerBound, T upperBound, bool includeLowerBound = false, bool includeUpperBound = false)
        where T : IComparable<T>
        {
            if (t == null) throw new ArgumentNullException("t");

            var lowerCompareResult = t.CompareTo(lowerBound);
            var upperCompareResult = t.CompareTo(upperBound);

            return (includeLowerBound && lowerCompareResult == 0) 
                    || (includeUpperBound && upperCompareResult == 0)
                    || (lowerCompareResult > 0 && upperCompareResult < 0);
        }
    }

    public static class DictionaryExtensions
    {
        /// <summary>
        /// Tries to read value and returns the value if successfully read. Otherwise return default value
        /// for value's type.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue TryGetAndReturn<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue obj;
            if (!dictionary.TryGetValue(key, out obj))
                obj = default(TValue);
            return obj;
        }
    }
}
