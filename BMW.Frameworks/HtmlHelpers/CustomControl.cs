using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Linq.Expressions;

namespace BMW.Frameworks.HtmlHelpers
{
    public static class CustomControl
    {
        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxList(helper, name, selectList, new { });
        }
        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {

            IDictionary<string, object> HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            HashSet<string> set = new HashSet<string>();
            List<SelectListItem> list = new List<SelectListItem>();
            string selectedValues = Convert.ToString((selectList as SelectList).SelectedValue);
            if (!string.IsNullOrEmpty(selectedValues))
            {
                if (selectedValues.Contains(","))
                {
                    string[] tempStr = selectedValues.Split(',');
                    for (int i = 0; i < tempStr.Length; i++)
                    {
                        set.Add(tempStr[i]);
                    }
                }
                else
                {
                    set.Add(selectedValues);
                }
            }

            foreach (SelectListItem item in selectList)
            {
                item.Selected = (item.Value != null) ? set.Contains(item.Value) : set.Contains(item.Text);
                list.Add(item);
            }
            selectList = list;

            HtmlAttributes.Add("type", "checkbox");
            //HtmlAttributes.Add("id", name);
            HtmlAttributes.Add("name", name);
            //HtmlAttributes.Add("style", "margin:0 0 0 10px;line-height:25px; vertical-align:-3px;border:none;");

            StringBuilder stringBuilder = new StringBuilder();
            int j = 0;
            foreach (SelectListItem selectItem in selectList)
            {
                j += 1;
                IDictionary<string, object> newHtmlAttributes = HtmlAttributes.DeepCopy();
                newHtmlAttributes.Add("value", selectItem.Value);
                newHtmlAttributes.Add("id", name + j);
                if (selectItem.Selected)
                {
                    newHtmlAttributes.Add("checked", "checked");
                }

                TagBuilder tagBuilder = new TagBuilder("input");
                tagBuilder.MergeAttributes<string, object>(newHtmlAttributes);
                string inputAllHtml = tagBuilder.ToString(TagRenderMode.SelfClosing);
                //stringBuilder.AppendFormat(@"<label style=""margin:0 0 0 10px;""> {0}  {1}</label>",
                stringBuilder.AppendFormat(@"<label> {0}  {1}</label>",
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

        /// <summary>
        /// 单选按钮组
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="Tproperty"></typeparam>
        /// <param name="help"></param>
        /// <param name="expression"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioListFor<TModel, Tproperty>(this  HtmlHelper<TModel> help,
    Expression<Func<TModel, Tproperty>> expression,
    IEnumerable<SelectListItem> source)
        {
            string exprestr = expression.ToString();
            string radioName = exprestr.Substring(exprestr.IndexOf('.') + 1);
            if (source != null && source.Any())
            {
                StringBuilder builder = new StringBuilder();
                int i = 1;
                foreach (var item in source)
                {
                    builder.Append("<input id='Radio" + radioName + i.ToString() + "' type='radio' name='" + radioName + "' value='" + item.Value + "' /> " + item.Text + "");
                    i++;
                }
                return MvcHtmlString.Create(builder.ToString());
            }
            return MvcHtmlString.Create(string.Empty);
        }

    }
}