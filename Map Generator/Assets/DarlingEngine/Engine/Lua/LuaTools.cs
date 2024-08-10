using System.Net;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using DarlingEngine.Engine.Bundle;

namespace DarlingEngine.Engine.Lua
{
    // 逐步整理被Lua调用的一些常用函数, TODO: 增加 LuaCallCsharp 属性
    public class LuaTools
    {
        public static string HttpGet(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "text/html;charset=UTF-8";
            string result;
            try
            {
                Stream responseStream = ((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                result = text;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                result = null;
            }
            return result;
        }

        public static string LoadProtos(string protoPath)
        {
            return Encoding.UTF8.GetString(BundleManager.Instance.LoadProto(protoPath));
        }

        public static long NowTimeMilliSeconds()
        {
            return System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
