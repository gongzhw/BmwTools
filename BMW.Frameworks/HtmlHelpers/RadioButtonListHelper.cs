using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BMW.Frameworks.HtmlHelpers
{
    public static class RadioButtonHelpers
    {
        public static MvcHtmlString RadioButtonList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList)
        {
            return RadioButtonList(helper, name, selectList, new { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper">this</param>
        /// <param name="name">名称</param>
        /// <param name="selectList">list的item列表，支持传入list<int>作为默认绑定值</param>
        /// <param name="htmlAttributes">事件</param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            IDictionary<string, object> HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            List<SelectListItem> list = new List<SelectListItem>();
            int selectedValues = Utils.StrToInt((selectList as SelectList).SelectedValue, -1);

            foreach (SelectListItem item in selectList)
            {
                item.Selected = Utils.StrToInt(item.Value, 0) == selectedValues; //selectedValues != null ? selectedValues.Contains(Utils.StrToInt(item.Value, 0)) : false;
                list.Add(item);
            }

            selectList = list;

            HtmlAttributes.Add("type", "radio");

            HtmlAttributes.Add("name", name);
            HtmlAttributes.Add("style", "margin:0 0 0 0px;line-height:25px; font-size:12px; vertical-align:-3px;border:none;");

            StringBuilder stringBuilder = new StringBuilder();

            foreach (SelectListItem selectItem in selectList)
            {
                IDictionary<string, object> newHtmlAttributes = HtmlAttributes.DeepCopy();
                newHtmlAttributes.Add("value", selectItem.Value);
                newHtmlAttributes.Add("id", name + "_" + selectItem.Value);
                if (selectItem.Selected)
                {
                    newHtmlAttributes.Add("checked", "checked");
                }

                TagBuilder tagBuilder = new TagBuilder("input");
                tagBuilder.MergeAttributes<string, object>(newHtmlAttributes);
                string inputAllHtml = tagBuilder.ToString(TagRenderMode.SelfClosing);
                stringBuilder.AppendFormat(@"<label style=""margin:0 10px 0 0px; font-size:12px;""> {0}  {1}</label>",
                   inputAllHtml, selectItem.Text);
            }
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        private static IDictionary<string, object> DeepCopy(this IDictionary<string, object> ht)
        {
            Dictionary<string, object> _ht = new Dictionary<string, object>();

            foreach (var p in ht)
            {
                _ht.Add(p.Key, p.Value);
            }
            return _ht;
        }

        /// <summary>
        /// 将一个T类型的数组Int[] 或者String[] 转化成1,2 格式，去掉最后一个逗号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetCheckBoxListValueList<T>(T[] list)
        {
            if (list == null) return "";
            string slist = null;
            for (int i = 0; i < list.Length; i++)
            {
                if (i != list.Length - 1)
                    slist += list[i] + ",";
                else
                    slist += list[i];
            }
            return slist;
        }
    }

}