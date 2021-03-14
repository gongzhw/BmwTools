using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Mvc;

namespace BMW.Frameworks.HtmlHelpers
{
    public class EFHelper
    {
        /// <summary>
        /// 从object获取属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filedName"></param>
        /// <returns></returns>
        public static object GetInfo(object obj, string filedName)
        {
            PropertyInfo p = obj.GetType().GetProperty(filedName);
            return p.GetValue(obj, null);
        }

        ///// <summary>
        ///// 添加或更新实体时，实体验证未通过的错误信息
        ///// </summary>
        ///// <param name="Keys"></param>
        ///// <param name="modelError"></param>
        ///// <returns></returns>
        //public static string GetEntityErrorInfo(List<string> Keys, List<ModelError> modelError)
        //{
        //    System.Text.StringBuilder sb = new StringBuilder();
        //    sb.Append("请检查以下项目：");
        //    foreach (var key in Keys)
        //    {
        //        foreach (var error in modelError)
        //        {
        //            sb.Append(string.Format("<li>{0}</li>",error.ErrorMessage));
        //        }
        //    }
        //    return sb.ToString();
        //}
    }
}
