using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using System.Web.Mvc.Html;

namespace BMW.Frameworks.HtmlHelpers
{
    public static class PageHelper
    {
        /// <summary>
        /// 获取分页是跳过的记录数
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public static int Skip(int pageindex, int pagesize)
        {
            int skip = 0;
            if (pageindex == 1)
                skip = 0;
            else
                skip = (pageindex - 1) * pagesize;
            return skip;
        }

        /// <summary>
        /// MVC分页 自动组合URL参数
        /// </summary>
        /// <param name="context">this.HttpContext</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="totalRecord">总记录数</param>
        /// <param name="className">css样式名称</param>
        /// <returns></returns>
        public static string webPager(HttpContextBase context, int pageIndex, int pageSize, int totalRecord,string className)
        {
            int totalPage = Math.Max((totalRecord + pageSize - 1) / pageSize, 1);
            if (totalPage <= 1) return "";

            StringBuilder sburlpar = new StringBuilder();

            int k = 0;
            string Values = string.Empty;
            bool isSeachPar = false;
            foreach (string key in context.Request.QueryString.AllKeys)
            {
                Values = context.Request.QueryString[key];
                if (!string.IsNullOrEmpty(Values) && !string.IsNullOrEmpty(key))
                {
                    if (key == "p") isSeachPar = true;
                    if (k == 0)
                        sburlpar.Append(string.Format("?{0}={1}", key, Values));
                    else
                        sburlpar.Append(string.Format("&{0}={1}", key, Values));
                    k += 1;
                }
              
            }
            string urlpar = sburlpar.ToString();

            if (isSeachPar)
            {
                if (k > 1)
                    urlpar = urlpar.Remove(urlpar.LastIndexOf("&"));
                else
                    urlpar = urlpar.Remove(urlpar.LastIndexOf("?"));
            }


            StringBuilder sb = new StringBuilder();

            if (totalRecord > 0)
            {
                if (k == 0||k==1 && isSeachPar)
                    urlpar = urlpar + "?p=";
                else
                    urlpar = urlpar + "&p=";

                sb.Append("<div class=\"" + className + "\">");

                if (pageIndex == 1)
                    sb.Append("<span>首页</span>");
                else
                    sb.Append(string.Format("<a href=\"{0}1\">首页</a>", urlpar));

                if (pageIndex > 1)
                    sb.Append(string.Format("<a href=\"{0}\">上一页</a>", urlpar + (pageIndex - 1)));
                else
                    sb.Append("<span>上一页</span>");

                //创建中间的分页
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {

                    if ((pageIndex + i - currint) >= 1 && (pageIndex + i - currint) <= totalPage)
                        if (currint == i)
                            sb.Append(string.Format(" [{0}]", pageIndex));
                        else
                            sb.Append(string.Format("<a href=\"{0}\">{1}</a>", urlpar + (pageIndex + i - currint), (pageIndex + i - currint)));
                }

                if (pageIndex < totalPage)
                    sb.Append(string.Format("<a href=\"{0}\">下一页</a>", urlpar+ (pageIndex + 1)));
                else
                    sb.Append("<span>下一页</span>");

                if (pageIndex == totalPage)
                    sb.Append("<span>末页</span>");
                else
                    sb.Append(string.Format("<a href=\"{0}\">末页</a>", urlpar + totalPage));
                sb.Append("<span>总记录数：" + totalRecord + "/总页数：" + totalPage + "</span>");
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 分页 只显示2个按钮（上一页和下一页）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecord"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static string webPager(HttpContextBase context, int pageIndex, int pageSize, int totalRecord, int currint, string className)
        {
            int totalPage = Math.Max((totalRecord + pageSize - 1) / pageSize, 1);
            if (totalPage <= 1) return "";

            StringBuilder sburlpar = new StringBuilder();

            int k = 0;
            string Values = string.Empty;
            bool isSeachPar = false;
            foreach (string key in context.Request.QueryString.AllKeys)
            {
                Values = context.Request.QueryString[key];
                if (!string.IsNullOrEmpty(Values) && !string.IsNullOrEmpty(key))
                {
                    if (key == "p") isSeachPar = true;
                    if (k == 0)
                        sburlpar.Append(string.Format("?{0}={1}", key, Values));
                    else
                        sburlpar.Append(string.Format("&{0}={1}", key, Values));
                    k += 1;
                }

            }
            string urlpar = sburlpar.ToString();

            if (isSeachPar)
            {
                if (k > 1)
                    urlpar = urlpar.Remove(urlpar.LastIndexOf("&"));
                else
                    urlpar = urlpar.Remove(urlpar.LastIndexOf("?"));
            }


            StringBuilder sb = new StringBuilder();

            if (totalRecord > 0)
            {
                if (k == 0 || k == 1 && isSeachPar)
                    urlpar = urlpar + "?p=";
                else
                    urlpar = urlpar + "&p=";

                sb.Append("<div class=\"" + className + "\">");

                if (pageIndex == 1)
                    sb.Append("<span>首页</span>");
                else
                    sb.Append(string.Format("<a href=\"{0}1\">首页</a>", urlpar));

                if (pageIndex > 1)
                    sb.Append(string.Format("<a href=\"{0}\">上一页</a>", urlpar + (pageIndex - 1)));
                else
                    sb.Append("<span>上一页</span>");

                //创建中间的分页
                if (currint > 0)
                {
                    for (int i = 0; i <= 10; i++)
                    {

                        if ((pageIndex + i - currint) >= 1 && (pageIndex + i - currint) <= totalPage)
                            if (currint == i)
                                sb.Append(string.Format(" [{0}]", pageIndex));
                            else
                                sb.Append(string.Format("<a href=\"{0}\">{1}</a>", urlpar + (pageIndex + i - currint), (pageIndex + i - currint)));
                    }
                }

                if (pageIndex < totalPage)
                    sb.Append(string.Format("<a href=\"{0}\">下一页</a>", urlpar + (pageIndex + 1)));
                else
                    sb.Append("<span>下一页</span>");

                if (pageIndex == totalPage)
                    sb.Append("<span>末页</span>");
                else
                    sb.Append(string.Format("<a href=\"{0}\">末页</a>", urlpar + totalPage));
                //sb.Append("<span>总记录数：" + totalRecord + "/总页数：" + totalPage + "</span>");
                sb.Append("</div>");
            }
            return sb.ToString();
        }


        public static string webPager(string url, int pageIndex, int pageSize, int totalRecord,bool haveInfoBar, string className)
        {
            int totalPage = Math.Max((totalRecord + pageSize - 1) / pageSize, 1);
            if (totalPage <= 1) return "";

            StringBuilder sb = new StringBuilder();

            if (totalRecord > 0)
            {
                sb.Append("<div class=\"" + className + "\">");

               sb.Append(string.Format("<a href=\"{0}\">首页</a>", string.Format(url, 1)));

                if (pageIndex > 1)
                    sb.Append(string.Format("<a href=\"{0}\">上一页</a>", string.Format(url, pageIndex - 1)));
                else
                    sb.Append("<span  class=\"disable\">上一页</span>");

                //创建中间的分页
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {
                    if ((pageIndex + i - currint) >= 1 && (pageIndex + i - currint) <= totalPage)
                        if (currint == i)
                            sb.Append(string.Format("<span class=\"current\">{0}</span>", pageIndex));
                        else
                            sb.Append(string.Format("<a href=\"{0}\">{1}</a>", string.Format(url, pageIndex + i - currint), (pageIndex + i - currint)));
                }

                if (pageIndex < totalPage)
                    sb.Append(string.Format("<a href=\"{0}\">下一页</a>",string.Format(url,pageIndex + 1)));
                else
                    sb.Append("<span class=\"disable\">下一页</span>");

                if (pageIndex == totalPage)
                    sb.Append(string.Format("<a href=\"{0}\">末页</a>", string.Format(url, pageIndex)));
                else
                    sb.Append(string.Format("<a href=\"{0}\">末页</a>", string.Format(url, totalPage)));
                if (haveInfoBar)
                {
                    sb.Append("<span>总记录数：" + totalRecord + "/总页数：" + totalPage + "</span>");
                }
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        public static string mobilePager(string url, int pageIndex, int pageSize, int totalRecord, string className)
        {
            int totalPage = Math.Max((totalRecord + pageSize - 1) / pageSize, 1);
            if (totalPage <= 1) return "";

            StringBuilder sb = new StringBuilder();

            if (totalRecord > 0)
            {
                sb.Append("<div class=\"" + className + "\">");

                if (pageIndex == 1)
                    sb.Append("<span>首页 </span>");
                else
                    sb.Append(string.Format("<a href=\"{0}\">首页 </a>", string.Format(url, 1)));

                if (pageIndex > 1)
                    sb.Append(string.Format("<a href=\"{0}\">上一页 </a>", string.Format(url, pageIndex - 1)));
                else
                    sb.Append("<span>上一页 </span>");


                if (pageIndex < totalPage)
                    sb.Append(string.Format("<a href=\"{0}\">下一页 </a>", string.Format(url, pageIndex + 1)));
                else
                    sb.Append("<span>下一页 </span>");

                if (pageIndex == totalPage)
                    sb.Append("<span>末页 </span>");
                else
                    sb.Append(string.Format("<a href=\"{0}\">末页 </a>", string.Format(url, totalPage)));

                    sb.Append("<span>总计：" + totalRecord + "条 </span>");
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        //public static string mobilePager(string url, int pageIndex, int pageSize, int totalRecord)
        //{
        //    int totalPage = Math.Max((totalRecord + pageSize - 1) / pageSize, 1);
        //    if (totalPage <= 1) return "";

        //    StringBuilder sb = new StringBuilder();

        //    if (totalRecord > 0)
        //    {
        //        sb.Append("<div class=\"new-paging\"><div class=\"new-tbl-type\">");

        //        if (pageIndex > 1)
        //            sb.Append(string.Format("<div class=\"new-tbl-cell\"><span class=\"new-a-prve\"><span><a href=\"{0}\">上一页</a></span></span></div>", string.Format(url, pageIndex - 1)));
        //        else
        //            sb.Append("<div class=\"new-tbl-cell\"><span class=\"new-a-prve\"><span>上一页</span></span></div>");


        //        if (pageIndex < totalPage)
        //            sb.Append(string.Format("<div class=\"new-tbl-cell\"><a href=\"{0}\" class=\"new-a-next\"><span>下一页</span></a></div>", string.Format(url, pageIndex + 1)));
        //        else
        //            sb.Append("<div class=\"new-tbl-cell\"><span class=\"new-a-next\">下一页 </span></div>");


        //        sb.Append("</div></div>");
        //    }
        //    return sb.ToString();
        //}

    }
}