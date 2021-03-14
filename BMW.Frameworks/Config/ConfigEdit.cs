using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;


namespace BMW.Frameworks.Config
{
    public class ConfigEditor : IDisposable
    {
        private Configuration config;
        public ConfigEditor()
            : this(HttpContext.Current.Request.ApplicationPath)
        {

        }
        public ConfigEditor(string path)
        {
            config = WebConfigurationManager.OpenWebConfiguration(path);
        }
        /// <summary> 
        /// ����Ӧ�ó������ýڵ㣬����Ѿ����ڴ˽ڵ㣬����޸ĸýڵ��ֵ��������Ӵ˽ڵ� 
        /// </summary> 
        /// <param name="key">�ڵ�����</param> 
        /// <param name="value">�ڵ�ֵ</param> 
        public void SetAppSetting(string key, string value)
        {
            AppSettingsSection appSetting = (AppSettingsSection)config.GetSection("appSettings");
            if (appSetting.Settings[key] == null)//��������ڴ˽ڵ㣬����� 
            {
                appSetting.Settings.Add(key, value);
            }
            else//������ڴ˽ڵ㣬���޸� 
            {
                appSetting.Settings[key].Value = value;
            }
        }
        /// <summary> 
        /// �������ݿ������ַ����ڵ㣬��������ڴ˽ڵ㣬�����Ӵ˽ڵ㼰��Ӧ��ֵ���������޸� 
        /// </summary> 
        /// <param name="key">�ڵ�����</param> 
        /// <param name="connectionString">�ڵ�ֵ</param> 
        public void SetConnectionString(string key, string connectionString)
        {
            ConnectionStringsSection connectionSetting = (ConnectionStringsSection)config.GetSection("connectionStrings");
            if (connectionSetting.ConnectionStrings[key] == null)//��������ڴ˽ڵ㣬����� 
            {
                ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings(key, connectionString);
                connectionSetting.ConnectionStrings.Add(connectionStringSettings);
            }
            else//������ڴ˽ڵ㣬���޸� 
            {
                connectionSetting.ConnectionStrings[key].ConnectionString = connectionString;
            }
        }
        /// <summary> 
        /// �����������޸� 
        /// </summary> 
        public void Save()
        {
            config.Save();
            config = null;
        }
        public void Dispose()
        {
            if (config != null)
            {
                config.Save();
            }
        }
    }
}
