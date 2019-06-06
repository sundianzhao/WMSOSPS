using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using log4net;
using log4net.Config;
using log4net.Core;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Tools;

namespace WMSOSPS.Cloud.Code.Logger
{
    public sealed class LogHelper
    {
        /// <summary>
        /// 记录消息Queue
        /// </summary>
        private readonly ConcurrentQueue<LogMessage> _que;

        /// <summary>
        /// 信号
        /// </summary>
        private readonly ManualResetEvent _mre;

        /// <summary>
        /// 日志器
        /// </summary>
        private static readonly ILog _log = LogManager.GetLogger("MyLog");
        //private static readonly ILog _printerLog = LogManager.GetLogger("PrinterLog");
        //private static readonly ILog _PayLog = LogManager.GetLogger("PayLog");

        /// <summary>
        /// 日志
        /// </summary>
        private static LogHelper _flashLog = new LogHelper();


        private LogHelper()
        {
            //LogHelper.SetConfig(new System.IO.FileInfo(Server.MapPath(@"~\App_Data\Log4Net.config")));

            var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
            if (!configFile.Exists)
            {
                throw new Exception("未配置log4net配置文件！");
            }

            // 设置日志配置文件路径
            XmlConfigurator.Configure(configFile);

            _que = new ConcurrentQueue<LogMessage>();
            _mre = new ManualResetEvent(false);
        }

        /// <summary>
        /// 实现单例
        /// </summary>
        /// <returns></returns>
        public static LogHelper Instance()
        {
            return _flashLog;
        }

        /// <summary>
        /// 另一个线程记录日志，只在程序初始化时调用一次
        /// </summary>
        public void Register()
        {
            Thread t = new Thread(new ThreadStart(WriteLog));
            t.IsBackground = false;
            t.Start();
        }

        /// <summary>
        /// 从队列中写日志至磁盘
        /// </summary>
        private void WriteLog()
        {
            while (true)
            {
                // 等待信号通知
                _mre.WaitOne();

                LogMessage msg;
                // 判断是否有内容需要如磁盘 从列队中获取内容，并删除列队中的内容
                while (_que.Count > 0 && _que.TryDequeue(out msg))
                {
                    // 判断日志等级，然后写日志
                    switch (msg.Level)
                    {
                        case LogLevel.Debug:
                            _log.Debug(msg, msg.Exception);
                            break;
                        case LogLevel.Info:
                            _log.Info(msg, msg.Exception);
                            break;
                        case LogLevel.Error:
                            _log.Error(msg, msg.Exception);
                            break;
                        case LogLevel.Warn:
                            _log.Warn(msg, msg.Exception);
                            break;
                        case LogLevel.Fatal:
                            _log.Fatal(msg, msg.Exception);
                            break;
                    }
                }

                // 重新设置信号
                _mre.Reset();
                Thread.Sleep(1);
            }
        }


        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志文本</param>
        /// <param name="level">等级</param>
        /// <param name="modelType">模块类型</param>
        /// <param name="ex">Exception异常信息</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="viewLevel">查看等级</param>
        /// <param name="toObjct"></param>
        /// <param name="fromMethod"></param>
        public void EnqueueMessage(string message, LogLevel level, OpType operationType, ModelType modelType, Exception ex = null,
           ViewLevel viewLevel = ViewLevel.Admin, string toObjct = "", string fromMethod = "")
        {
            if ((level == LogLevel.Debug && _log.IsDebugEnabled)
             || (level == LogLevel.Error && _log.IsErrorEnabled)
             || (level == LogLevel.Fatal && _log.IsFatalEnabled)
             || (level == LogLevel.Info && _log.IsInfoEnabled)
             || (level == LogLevel.Warn && _log.IsWarnEnabled))
            //|| (level == LogLevel.Printer && _printerLog.IsErrorEnabled)
            //|| (level == LogLevel.Pay && _PayLog.IsInfoEnabled))
            {
                var ip = Net.Net.Ip;
                //var ip = "112.253.8.147";
                _que.Enqueue(new LogMessage
                {
                    //Message = GetContent(message, ex).ToString(),
                    Message = message,
                    Level = level,
                    Exception = ex,
                    UserIP = ip,
                    UserName = OperatorProvider.Provider.GetCurrent().UserCode,
                    CurrentUrl = HttpContext.Current?.Request.Url.ToString(),
                    PrevUrl = (HttpContext.Current?.Request.UrlReferrer != null
                ? HttpContext.Current?.Request.UrlReferrer.AbsoluteUri : string.Empty),
                    IPCity = Tool.GetIpLocation(ip),
                    OperationType = (int)operationType,
                    ToObjct = toObjct,
                    ViewLevel = (int)viewLevel,
                    ModelType = (int)modelType,
                    FromMethod = fromMethod
                });

                // 通知线程往磁盘中写日志
                _mre.Set();
            }
        }
        public void EnqueueMessage(string message, LogLevel level, OpType operationType, Exception ex = null,
           ViewLevel viewLevel = ViewLevel.Admin, string toObjct = "", string fromMethod = "")
        {
            if ((level == LogLevel.Debug && _log.IsDebugEnabled)
             || (level == LogLevel.Error && _log.IsErrorEnabled)
             || (level == LogLevel.Fatal && _log.IsFatalEnabled)
             || (level == LogLevel.Info && _log.IsInfoEnabled)
             || (level == LogLevel.Warn && _log.IsWarnEnabled))
            //|| (level == LogLevel.Printer && _printerLog.IsErrorEnabled)
            //|| (level == LogLevel.Pay && _PayLog.IsInfoEnabled))
            {
                var ip = Net.Net.Ip;
                //var ip = "112.253.8.147";
                _que.Enqueue(new LogMessage
                {
                    //Message = GetContent(message, ex).ToString(),
                    Message = message,
                    Level = level,
                    Exception = ex,
                    UserIP = ip,
                    UserName = OperatorProvider.Provider.GetCurrent().UserCode,
                    CurrentUrl = HttpContext.Current?.Request.Url.ToString(),
                    PrevUrl = (HttpContext.Current?.Request.UrlReferrer != null
                ? HttpContext.Current?.Request.UrlReferrer.AbsoluteUri
                : string.Empty),
                    IPCity = Tool.GetIpLocation(ip),
                    OperationType = (int)operationType,
                    ToObjct = toObjct,
                    ViewLevel = (int)viewLevel,
                    FromMethod = fromMethod
                });

                // 通知线程往磁盘中写日志
                _mre.Set();
            }
        }
        public static StringBuilder GetContent(string message, Exception ex)
        {
            var builder = new StringBuilder();
            var msg = message;
            builder.Append("\r\n【客户机IP】");
            builder.Append(WebUtils.GetIp());
            builder.Append("\r\n【当前Url】");
            builder.Append(HttpContext.Current?.Request.Url);
            builder.Append("\r\n【上一个URL】");
            builder.Append((HttpContext.Current?.Request.UrlReferrer != null
                ? HttpContext.Current?.Request.UrlReferrer.AbsoluteUri
                : string.Empty));
            builder.Append("\r\n【消息描述】");
            builder.Append("\r\n");

            msg = msg + (string.IsNullOrEmpty(msg) ? "" : "\r\n") +
                  (ex != null
                      ? "异常：" + ex.Message + "\r\n" +
                        (ex.InnerException != null ? "内部异常：" + ex.InnerException.Message + "\r\n" : "")
                      : "");
            builder.Append(msg);
            return builder;
        }


        #region Debug
        /// <summary>
        /// Debug级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="modelType">模块类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Debug(string msg, OpType opType, ModelType modelType, Exception ex = null
            , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Debug, opType, modelType, ex, viewLevel, toObjct, fromMethod);
        }
        /// <summary>
        /// Debug级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Debug(string msg, OpType opType, Exception ex = null
            , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Debug, opType, ex, viewLevel, toObjct, fromMethod);
        }
        #endregion

        #region Error
        /// <summary>
        /// Error级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="modelType">模块类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Error(string msg, OpType opType, ModelType modelType, Exception ex = null
           , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Debug, opType, modelType, ex, viewLevel, toObjct, fromMethod);
        }
        /// <summary>
        /// Error级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Error(string msg, OpType opType, Exception ex = null
           , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Error, opType, ex, viewLevel, toObjct, fromMethod);
        }
        #endregion

        #region Fatal
        /// <summary>
        /// Fatal级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="modelType">模块类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Fatal(string msg, OpType opType, ModelType modelType, Exception ex = null
            , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Fatal, opType, modelType, ex, viewLevel, toObjct, fromMethod);
        }
        /// <summary>
        /// Fatal级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Fatal(string msg, OpType opType, Exception ex = null
            , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Fatal, opType, ex, viewLevel, toObjct, fromMethod);
        }
        #endregion

        #region Info
        /// <summary>
        /// Info级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="modelType">模块类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Info(string msg, OpType opType, ModelType modelType, Exception ex = null
            , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Info, opType, modelType, ex, viewLevel, toObjct, fromMethod);
        }
        /// <summary>
        /// Info级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Info(string msg, OpType opType, Exception ex = null
            , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Info, opType, ex, viewLevel, toObjct, fromMethod);
        }
        #endregion

        #region Warn
        /// <summary>
        /// Warn级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="modelType">模块类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Warn(string msg, OpType opType, ModelType modelType, Exception ex = null
          , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Warn, opType, modelType, ex, viewLevel, toObjct, fromMethod);
        }
        /// <summary>
        /// Warn级别
        /// </summary>
        /// <param name="msg">记录信息</param>
        /// <param name="ex">异常信息</param>
        /// <param name="opType">操作类型</param>
        /// <param name="fromMethod">来自哪个方法</param>
        /// <param name="toObjct">发送给谁</param>
        /// <param name="viewLevel">查看等级</param>
        public static void Warn(string msg, OpType opType, Exception ex = null
            , string fromMethod = "", string toObjct = "", ViewLevel viewLevel = ViewLevel.Admin)
        {
            Instance().EnqueueMessage(msg, LogLevel.Warn, opType, ex, viewLevel, toObjct, fromMethod);
        }
        #endregion
    }
}
