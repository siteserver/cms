using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using SiteServer.Utils;

namespace WxPayAPI
{
    public class Log
    {
        /**
         * 向日志文件写入调试信息
         * @param className 类名
         * @param content 写入内容
         */
        public static void Debug(string className, string content)
        {
            WriteLog("DEBUG", className, content);
        }

        /**
        * 向日志文件写入运行时信息
        * @param className 类名
        * @param content 写入内容
        */
        public static void Info(string className, string content)
        {
            WriteLog("INFO", className, content);
        }

        /**
        * 向日志文件写入出错信息
        * @param className 类名
        * @param content 写入内容
        */
        public static void Error(string className, string content)
        {
            WriteLog("ERROR", className, content);
        }

        /**
        * 实际的写日志操作
        * @param type 日志记录类型
        * @param className 类名
        * @param content 写入内容
        */
        protected static void WriteLog(string type, string className, string content)
        {
            //string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//获取当前系统时间

            ////向日志文件写入内容
            //string write_content = time + " " + type + " " + className + ": " + content;
            
            //LogUtils.AddSystemErrorLog(new Exception(type), write_content);
        }
    }
}