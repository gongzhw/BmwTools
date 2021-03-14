using System;
using System.Collections.Generic;
using System.IO;
using BMW.Frameworks;
using BMW.Frameworks.WebRequest;
using BMW.Frameworks.Logs;
using BMW.Frameworks.Config;

namespace BMW.Frameworks
{
    public class PhotoProxy
    {
        //资源服务地址(在应用中赋值，不使用此服务的应用可以不在配置文件添加这项内容)
        private static string _resourceServiceUrl=string.Empty;
        private static readonly NLogLogger Logger = new NLogLogger();

        #region 封装对资源服务的操作

        public static string AddPicture(string fileName, string contentType, string picModes, string[] customParam, Stream sm, out int resourceId)
        {
            string strReturn = string.Empty;
            const string addUrl = "InsertPic";

            resourceId = 0;
            try
            {
                List<AbstractPostData> parameterList = new List<AbstractPostData>();

                //自定义尺寸类型（来自枚举）
                AbstractPostData paramterEntity = new FormPostData("type", picModes);
                parameterList.Add(paramterEntity);

                //原始文件名
                paramterEntity = new FormPostData("fileName", fileName);
                parameterList.Add(paramterEntity);

                //自定义的参数
                if (customParam != null)
                {
                    for (int i = 0; i < customParam.Length; i++)
                    {
                        string[] paramTemp = customParam[i].Split('=');
                        paramterEntity = new FormPostData(paramTemp[0], paramTemp[1]);
                        parameterList.Add(paramterEntity);
                    }
                }

                //其它参数必须在file之前,否则服务端无法得到
                sm.Position = 0;
                paramterEntity = new FilePostData("file", fileName, contentType, sm);
                parameterList.Add(paramterEntity);

                var actionResult = PostPic(parameterList, addUrl, true);

                if (!string.IsNullOrEmpty(actionResult))
                {
                    PostResult pr = GetPostResult(actionResult);

                    if (pr.success)
                    {
                        resourceId = pr.id;
                        strReturn = pr.url;
                    }
                    else
                    {
                        strReturn = pr.message;
                        string strlogger = $"AddPicture添加失败,从服务返回的Message:{pr.message}";
                        Logger.Error(strlogger);
                    }
                }
                return strReturn;
            }
            catch (Exception ex)
            {
                Logger.Error($"调用AddPicture发生异常,ErrorMessage:{ex.Message}");
                return ex.Message;
            }
        }

        public static string AddPicture(string fileName, string contentType, string picModes, string[] customParam, Stream sm, bool setPosition = false)
        {
            string strReturn = string.Empty;
            const string addUrl = "InsertPic";

            try
            {
                List<AbstractPostData> parameterList = new List<AbstractPostData>();

                //自定义尺寸类型（来自枚举）
                AbstractPostData paramterEntity = new FormPostData("type", picModes);
                parameterList.Add(paramterEntity);

                //原始文件名
                paramterEntity = new FormPostData("fileName", fileName);
                parameterList.Add(paramterEntity);

                //自定义的参数
                if (customParam != null)
                {
                    for (int i = 0; i < customParam.Length; i++)
                    {
                        string[] paramTemp = customParam[i].Split('=');
                        paramterEntity = new FormPostData(paramTemp[0], paramTemp[1]);
                        parameterList.Add(paramterEntity);
                    }
                }

                //其它参数必须在file之前,否则服务端无法得到
                if (setPosition)
                {
                    sm.Position = 0;
                }
                paramterEntity = new FilePostData("file", fileName, contentType, sm);
                parameterList.Add(paramterEntity);

                var actionResult = PostPic(parameterList, addUrl, true);

                if (!string.IsNullOrEmpty(actionResult))
                {
                    PostResult pr = GetPostResult(actionResult);

                    if (pr.success)
                    {
                        strReturn = pr.url;
                    }
                    else
                    {
                        strReturn = pr.message;
                        string strlogger = $"AddPicture添加失败,从服务返回的Message:{pr.message}";
                        Logger.Error(strlogger);
                    }
                }
                return strReturn;
            }
            catch (Exception ex)
            {
                Logger.Error($"调用AddPicture发生异常,ErrorMessage:{ex.Message}");
                return ex.Message;
            }
        }

        public static PostResult AddQRCode(string fileName, string contents)
        {
            PostResult pr = new PostResult();
            const string addUrl = "CreateQR";

            try
            {
                List<AbstractPostData> parameterList = new List<AbstractPostData>();

                //原始文件名
                AbstractPostData paramterEntity = new FormPostData("fn", fileName);
                parameterList.Add(paramterEntity);

                paramterEntity = new FormPostData("qrc", contents);
                parameterList.Add(paramterEntity);

                var actionResult = PostQRCode(parameterList, addUrl, false);

                if (!string.IsNullOrEmpty(actionResult))
                {
                    pr = GetPostResult(actionResult);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"调用AddQRCode发生异常,ErrorMessage:{ex.Message}");
                pr.message = $"调用AddQRCode发生异常,ErrorMessage:{ex.Message}";
                pr.success = false;
            }
            return pr;
        }

        public static PostResult CutPic(string filePath, int xPosition, int yPosition, int width, int height, int quality)
        {
            PostResult pr = new PostResult();
            string addUrl = "CutImages";

            try
            {
                List<AbstractPostData> parameterList = new List<AbstractPostData>();

                //原始文件名
                AbstractPostData paramterEntity = new FormPostData("filePath", filePath);
                parameterList.Add(paramterEntity);

                paramterEntity = new FormPostData("xPosition", xPosition.ToString());
                parameterList.Add(paramterEntity);
                paramterEntity = new FormPostData("yPosition", yPosition.ToString());
                parameterList.Add(paramterEntity);

                paramterEntity = new FormPostData("width", width.ToString());
                parameterList.Add(paramterEntity);
                paramterEntity = new FormPostData("height", height.ToString());
                parameterList.Add(paramterEntity);

                paramterEntity = new FormPostData("quality", xPosition.ToString());
                parameterList.Add(paramterEntity);

                var actionResult = PostQRCode(parameterList, addUrl, false);

                if (!string.IsNullOrEmpty(actionResult))
                {
                    pr = GetPostResult(actionResult);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"调用CutHeadPic发生异常,ErrorMessage:{ex.Message}");
                pr.message = $"调用CutHeadPic发生异常,ErrorMessage:{ex.Message}";
                pr.success = false;
            }
            return pr;
        }

        private static string PostPic(IList<AbstractPostData> postDatas, string strUrl, bool isStream)
        {
            if (string.IsNullOrEmpty(_resourceServiceUrl))
            {
                _resourceServiceUrl =ConfigHelper.GetConfigString("Photo.Resource.Url");
            }
            strUrl = _resourceServiceUrl + strUrl;

            RequestDispatcher dispatcher;
            if (isStream)
            {
                dispatcher = new MultipartRequestDispatcher(postDatas, strUrl);
            }
            else
            {
                dispatcher = new XwwwRequestDispatcher(postDatas, strUrl);
            }
            return dispatcher.ForwardForString();
        }

        private static string PostQRCode(IList<AbstractPostData> postDatas, string strUrl, bool isStream)
        {
            if (string.IsNullOrEmpty(_resourceServiceUrl))
            {
                _resourceServiceUrl = ConfigHelper.GetConfigString("Photo.Resource.Url");
            }
            strUrl = _resourceServiceUrl + strUrl;

            RequestDispatcher dispatcher;
            if (isStream)
            {
                dispatcher = new MultipartRequestDispatcher(postDatas, strUrl);
            }
            else
            {
                dispatcher = new XwwwRequestDispatcher(postDatas, strUrl);
            }
            return dispatcher.ForwardForString();
        }

        #region 通过计算,从图片原始url得到真实url地址

        /// <summary>
        /// 计算图片url地址
        /// </summary>
        /// <param name="urlBase"></param>
        /// <param name="picMode"></param>
        /// <returns></returns>
        public static string GetImgUrl(string urlBase, PictureType picMode)
        {
            if (!string.IsNullOrEmpty(urlBase))
            {
                if (urlBase.IndexOf("[model]") != -1)
                {
                    //新图片地址
                    urlBase = urlBase.Replace("[model]", picMode.ToString());
                }
            }
            return urlBase;
        }

        public static string GetImgUrl(string urlBase, string picMode)
        {
            if (!string.IsNullOrEmpty(urlBase))
            {
                if (urlBase.IndexOf("[model]") != -1)
                {
                    //新图片地址
                    urlBase = urlBase.Replace("[model]", picMode);
                }
            }
            return urlBase;
        }

        #endregion

        #region 检查图片是否符合要求

        /// <summary>
        /// 图片检查
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="contentLength">字节流长度</param>
        /// <param name="maxSize">图片大小的上限</param>
        /// <param name="sm"></param>
        /// <returns></returns>
        public static string PictureCheck(string fileName, string contentType, int contentLength, int maxSize, Stream sm)
        {
            string errorMessage = string.Empty;
            string _strAllowType = ".jpg|.gif|.png|.bmp";

            //取文件扩展名
            string strPicNameWithExtend = fileName.Substring(fileName.LastIndexOf('\\') + 1); //取带有扩展名的文件名

            if (!strPicNameWithExtend.Contains("."))
            {
                errorMessage = "您上传的文件无扩展名，请重新选择！"; // 此处判断用户是否选择了图片文件
                return errorMessage;
            }
            //string strPicNameWithoutExtend = strPicNameWithExtend.Substring(0, strPicNameWithExtend.LastIndexOf('.')); //取没有扩展名的文件名
            string strExtendName = strPicNameWithExtend.Substring(strPicNameWithExtend.LastIndexOf('.')).ToLower(); //取扩展名

            // 测试图片是否符合要求
            if (sm == null)
            {
                errorMessage = "请先选择图片文件，然后上传！"; // 此处判断用户是否选择了图片文件
                return errorMessage;
            }

            if (!contentType.ToUpper().StartsWith("IMAGE/")) //如果不是图片类型
            {
                errorMessage = "您上传的不是图片文件！请上传：" + _strAllowType + "结尾的文件!";
                return errorMessage;
            }
            try
            {
                System.Drawing.Image imgOraginal = System.Drawing.Image.FromStream(sm);
            }
            catch
            {
                errorMessage = "您上传的不是图片文件！" + "请上传：" + _strAllowType + "结尾的文件!";
                return errorMessage;
            }
            if (_strAllowType.ToLower().IndexOf(strExtendName) == -1) //如果上传图片的扩展名不在允许的行列
            {
                errorMessage = "您上传得图片格式未被允许！" + "请上传：" + _strAllowType + "结尾的文件!";
                return errorMessage;
            }
            if ((contentLength / 1024) > maxSize) //如果超过了规定大小
            {
                errorMessage = "您上传的图片大小超过了" + maxSize + "KB";
                return errorMessage;
            }
            return errorMessage;
        }
        #endregion

        #endregion

        /// <summary>
        /// 上传图片时需要生成的图片大小
        /// </summary>
        public enum PictureType
        {
            o,
            m,
            l,
            t,
            s
        }

        public enum PicturePath
        {
            /// <summary>
            /// 相对路径 入库用
            /// </summary>
            x,
            /// <summary>
            /// 绝对路径 显示用
            /// </summary>
            j
        }
        /// <summary>
        /// Post图片后返回结果
        /// </summary>
        public class PostResult
        {
            public bool success { get; set; }
            public int id { get; set; }
            public string url { get; set; }
            public string message { get; set; }
        }

        /// <summary>
        /// 转换返回的结果集
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static PostResult GetPostResult(string str)
        {
            var pr = new PostResult();
            if (!string.IsNullOrEmpty(str))
            {
                string[] temp = str.Split('|');
                if (temp.Length == 4)
                {
                    pr.success = Utils.StrToBool(temp[0], false);
                    pr.id = Utils.StrToInt(temp[1], 0);
                    pr.url = temp[2];
                    pr.message = temp[3];
                }
            }
            return pr;
        }

        /// <summary>
        /// 获取图片地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="picSize"></param>
        /// <returns></returns>
        public static string FormatPicUrl(string url, string picSize)
        {
            if (string.IsNullOrEmpty(_resourceServiceUrl))
            {
                _resourceServiceUrl = ConfigHelper.GetConfigString("Photo.Resource.Url");
            }

            if (!string.IsNullOrEmpty(_resourceServiceUrl))
            {
                if (string.IsNullOrEmpty(url)) return "url 参数不能为空！";
                if (string.IsNullOrEmpty(picSize)) return "picSize 参数不能为空！";
                if (url.IndexOf("[model]") > 0)
                    return _resourceServiceUrl + url.Replace("[model]", picSize);
                else
                    return _resourceServiceUrl + url;
            }
            return "图片服务器URL空!";
        }
    }
}
