using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using BMW.Frameworks.Config;

namespace BMW.Frameworks.Logs
{
    public class NLogLogger:ILogger {

        private Logger _logger;

        public NLogLogger() {
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// 记录日志，根据webconfig文件中配置的IsOnline字段
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            _logger.Info(message);
        }

        /// <summary>
        /// 根据站点状态（是否上线记录日志-线下记录 线上不记录 根据web.config文件isonline节点配置）
        /// </summary>
        /// <param name="message"></param>
        public void InfoByOffline(string message, bool isOnline = false)
        {
            if (!isOnline)
            {
                _logger.Info(message);
            }
        }

        public void Warn(string message) {
            _logger.Warn(message);
        }

        public void Debug(string message) {
            _logger.Debug(message);
        }

        public void Error(string message) {
            _logger.Error(message);
        }
        public void Error(Exception x) {
            Error(LogUtility.BuildExceptionMessage(x));
        }
        public void Fatal(string message) {
            _logger.Fatal(message);
        }
        public void Fatal(Exception x) {
            Fatal(LogUtility.BuildExceptionMessage(x));
        }
    }
}
