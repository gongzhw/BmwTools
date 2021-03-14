using System;
using System.Configuration;
using System.Diagnostics;
using System.Web.Caching;
using System.Web.Configuration;

namespace BMW.Frameworks.Config
{
	/// <summary>
	/// 操作web.config文件的类
	/// </summary>
	public class ConfigHelper
	{
	    /// <summary>
		/// 得到AppSettings中的配置字符串信息
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
        public static string GetConfigString(string key)
        {
            try
            {
                string cacheKey = "AppSettings-" + key;
                object objModel = Cache.CacheStrategy.GetInstance().getCacheObj(cacheKey);
                if (objModel == null)
                {
                    objModel = ConfigurationManager.AppSettings[key];
                    if (objModel != null)
                    {
                        Cache.CacheStrategy.GetInstance().AddObject(cacheKey, objModel, DateTime.Now.AddMinutes(180), TimeSpan.Zero);
                    }
                    else
                    {
                        throw new ConfigurationErrorsException($"Not Found specific configuration, the Key is '{key}'");
                    }
                }
                
                return objModel.ToString();
            }
            catch
            {
                return "";
            }
        }

		/// <summary>
		/// 得到AppSettings中的配置Bool信息
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool GetConfigBool(string key)
		{
			bool result = false;
			string cfgVal = GetConfigString(key);
			if(!string.IsNullOrEmpty(cfgVal))
			{
				try
				{
					result = bool.Parse(cfgVal);
				}
				catch(FormatException)
				{
					// Ignore format exceptions.
				}
			}
			return result;
		}
		/// <summary>
		/// 得到AppSettings中的配置Decimal信息
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static decimal GetConfigDecimal(string key)
		{
			decimal result = 0;
			string cfgVal = GetConfigString(key);
			if(!string.IsNullOrEmpty(cfgVal))
			{
				try
				{
					result = decimal.Parse(cfgVal);
				}
				catch(FormatException)
				{
					// Ignore format exceptions.
				}
			}

			return result;
		}
		/// <summary>
		/// 得到AppSettings中的配置int信息
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static int GetConfigInt(string key)
		{
			int result = 0;
			string cfgVal = GetConfigString(key);
			if(!string.IsNullOrEmpty(cfgVal))
			{
                try
                {
                    result = int.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }             
			}

			return result;
		}

        /// <summary>
        /// 更新web.config节点appSettings的值
        /// </summary>
        /// <param name="strKey">节点名</param>
        /// <param name="strValue">新值</param>
        public static void UpdateConfigAppSettings(string strKey, string strValue)
        {
            Configuration objConfig = WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection objAppsettings = (AppSettingsSection)objConfig.GetSection("appSettings");
            if (objAppsettings != null)
            {
                objAppsettings.Settings[strKey].Value = strValue;
                objConfig.Save();
            }
        }
	}
}
