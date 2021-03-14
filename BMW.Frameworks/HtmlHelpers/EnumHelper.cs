using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;

namespace BMW.Frameworks.HtmlHelpers
{

    public class SelectItemModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    /// <summary>
    /// 枚举辅助类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary> 
        /// 获得枚举类型数据项（不包括空项）
        /// </summary> 
        /// <param name="enumType">枚举类型</param> 
        /// <returns></returns> 
        public static IList<object> GetItems(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException();

            IList<object> list = new List<object>();

            // 获取Description特性 
            Type typeDescription = typeof(DescriptionAttribute);
            // 获取枚举字段
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                    continue;

                // 获取枚举值
                int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

                // 不包括空项
               // if (value > 0)
                //{
                    string text = string.Empty;
                    object[] array = field.GetCustomAttributes(typeDescription, false);

                    if (array.Length > 0) text = ((DescriptionAttribute)array[0]).Description;
                    else text = field.Name; //没有描述，直接取值

                    //添加到列表
                    list.Add(new { Value = value, Text = text });
                //}
            }
            return list;
        }


        /// <summary> 
        /// 获得枚举类型数据项（不包括空项）
        /// </summary> 
        /// <param name="enumType">枚举类型</param> 
        /// <returns></returns> 
        public static IList<SelectItemModel> GetEnumToSelectItems(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException();

            IList<SelectItemModel> list = new List<SelectItemModel>();

            // 获取Description特性 
            Type typeDescription = typeof(DescriptionAttribute);
            // 获取枚举字段
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                    continue;

                // 获取枚举值
                int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

                // 不包括空项
                // if (value > 0)
                //{
                string text = string.Empty;
                object[] array = field.GetCustomAttributes(typeDescription, false);

                if (array.Length > 0) text = ((DescriptionAttribute)array[0]).Description;
                else text = field.Name; //没有描述，直接取值

                //添加到列表
                list.Add(new SelectItemModel() { id = value, name = text });
                //}
            }

            return list;
        }


        /// <summary>
        /// 把枚举转成list 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="addDefaultItem">是否创建默认选项</param>
        /// <param name="defaultText">默认选项的Text</param>
        /// <param name="defaultValue">默认选项的Value</param>
        /// <returns></returns>
        public static IList<object> GetItems(this Type enumType, bool addDefaultItem, string defaultText = null, string defaultValue = null)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException();

            IList<object> list = new List<object>();
            if (addDefaultItem)
            {
                if (!string.IsNullOrEmpty(defaultText) && !string.IsNullOrEmpty(defaultValue))
                {
                    list.Add(new { Value = defaultValue, Text = defaultText });
                }
                else
                {
                    list.Add(new { Value = "-=请选择=-", Text = "0" });
                }
            }

            // 获取Description特性 
            Type typeDescription = typeof(DescriptionAttribute);
            // 获取枚举字段
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                    continue;

                // 获取枚举值
                int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

                // 不包括空项
                // if (value > 0)
                //{
                string text = string.Empty;
                object[] array = field.GetCustomAttributes(typeDescription, false);

                if (array.Length > 0) text = ((DescriptionAttribute)array[0]).Description;
                else text = field.Name; //没有描述，直接取值

                //添加到列表
                list.Add(new { Value = value, Text = text });
                //}
            }
            return list;
        }


        public static IList<object> GetItems(this Type enumType, bool addDefaultItem, int[] removeValues)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException();

            IList<object> list = new List<object>();
            if (addDefaultItem)
            {
                list.Add(new { Value = "0", Text = "-=请选择=-" });
            }

            // 获取Description特性 
            Type typeDescription = typeof(DescriptionAttribute);
            // 获取枚举字段
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                    continue;

                // 获取枚举值
                int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

                // 不包括空项
                // if (value > 0)
                //{
                if (!removeValues.Contains(value))
                {
                    string text = string.Empty;
                    object[] array = field.GetCustomAttributes(typeDescription, false);

                    if (array.Length > 0) text = ((DescriptionAttribute)array[0]).Description;
                    else text = field.Name; //没有描述，直接取值

                    //添加到列表
                    list.Add(new { Value = value, Text = text });
                    //}
                }
            }
            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="removeValue"></param>
        /// <returns></returns>
        //public static IList<object> GetItems(this Type enumType,string[] removeValue)
        //{
        //    if (!enumType.IsEnum)
        //        throw new InvalidOperationException();

        //    IList<object> list = new List<object>();

        //    // 获取Description特性 
        //    Type typeDescription = typeof(DescriptionAttribute);
        //    // 获取枚举字段
        //    FieldInfo[] fields = enumType.GetFields();
        //    foreach (FieldInfo field in fields)
        //    {
        //        if (!field.FieldType.IsEnum)
        //            continue;

        //        // 获取枚举值
        //        int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

        //        // 不包括空项
        //        string text = string.Empty;
        //        object[] array = field.GetCustomAttributes(typeDescription, false);
        //        if (array.Length > 0)
        //        {
        //            text = ((DescriptionAttribute)array[0]).Description;
        //        }
        //        else
        //        {
        //            text = field.Name; //没有描述，直接取值
        //        }
        //        if (!removeValue.Contains(text))
        //        {
        //            //添加到列表
        //            list.Add(new { Value = value, Text = text });
        //        }
        //    }
        //    return list;
        //}

        public static Dictionary<int, string> GetItems(this Type enumType, string[] removeValue)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException();

            Dictionary<int, string> dic = new Dictionary<int, string>();

            // 获取Description特性 
            Type typeDescription = typeof(DescriptionAttribute);
            // 获取枚举字段
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                    continue;

                // 获取枚举值
                int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

                // 不包括空项
                string text = string.Empty;
                object[] array = field.GetCustomAttributes(typeDescription, false);
                if (array.Length > 0)
                {
                    text = ((DescriptionAttribute)array[0]).Description;
                }
                else
                {
                    text = field.Name; //没有描述，直接取值
                }
                if (!removeValue.Contains(text))
                {
                    dic.Add(value, text);
                }
            }
            return dic;
        }


        /// <summary>
        /// 给一个值 获取一个枚举中相等的文本 （Text）
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string GetItemsText(this Type enumType,int values)
        {
            string TextValue = string.Empty;

            if (!enumType.IsEnum)
                throw new InvalidOperationException();

            // 获取Description特性 
            Type typeDescription = typeof(DescriptionAttribute);
            // 获取枚举字段
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsEnum)
                    continue;

                // 获取枚举值
                int value = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);

                if (value == values)
                {
                    // 不包括空项
                    object[] array = field.GetCustomAttributes(typeDescription, false);

                    if (array.Length > 0)
                        TextValue = ((DescriptionAttribute)array[0]).Description;
                    else
                        TextValue = field.Name; //没有描述，直接取值
                    continue;
                }
            }

            return TextValue;
        }
    }

    public static class EnumExtension
    {
        static readonly ConcurrentDictionary<Enum, string> Descriptions = new ConcurrentDictionary<Enum, string>();

        public static string ToDescription(this Enum value)
        {
            if (Descriptions.ContainsKey(value))
            {
                return Descriptions[value];
            }

            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
            {
                return value.ToString();
            }
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false);
            var description = attribute != null ? attribute.Description : value.ToString();
            Descriptions.GetOrAdd(value, description);
            return description;
        }

        public static string ToDescription<T>(this int value)
        {
            if(!typeof(T).IsEnum) throw new Exception("不是枚举类型");

            var a = (T)(object)value;
            return (a as Enum).ToDescription();
        }
    }
}
