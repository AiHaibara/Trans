﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Trans.Client.Data;
using Trans.Client.Tools.Helper;

namespace Trans.Client.Helper
{
    public class MyResult
    {
        public string language { get; set; }
        public string text { get; set; }
    }
    public class CustomTrans : ITrans
    {
        public ITranslator translator { get; set; }
        public IOcror ocror { get; set; }
        public TransStrategy Strategy => TransStrategy.Custom;
        public CustomTrans()
        {
            this.ocror = new Ocror();
            this.translator = new Translator();
        }
        public IOcror GetOcror()
        {
            return ocror;
        }
        public ITranslator GetTranslator()
        {
            return translator;
        }                                                              

        public static class AccessToken
        {
            static AccessToken()
            {
                clientId = GlobalData.Config.TransConfig.BaiduOCRID;
                clientSecret = GlobalData.Config.TransConfig.BaiduOCRSECRET;
            }
            // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
            // 返回token示例
            public static String TOKEN = "24.adda70c11b9786206253ddb70affdc46.2592000.1493524354.282335-1234567";

            // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
            private static String clientId = "百度云应用的AK";
            // 百度云中开通对应服务应用的 Secret Key
            private static String clientSecret = "百度云应用的SK";

            public static String getAccessToken()
            {
                String authHost = "https://aip.baidubce.com/oauth/2.0/token";
                HttpClient client = new HttpClient();
                List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
                paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
                paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

                HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
                String result = response.Content.ReadAsStringAsync().Result;
                TOKEN = JsonSerializer.Deserialize<Dictionary<string, object>>(result)["access_token"].ToString();
                Console.WriteLine(result);
                return result;
            }
        }

        public class BaiduOcrResult
        {
            public long log_id { get; set; }
            public int words_result_num { get; set; }
            public List<Words_Result> words_result { get; set; }
        }

        public class Words_Result
        {
            public string words { get; set; }
        }

        public class Ocror:IOcror
        {
            static Ocror()
            {
                AccessToken.getAccessToken();
            }
            // 通用文字识别
            public MyResult CropImage()
            {
                string token = AccessToken.TOKEN;
                string host = "http://127.0.0.1:5500/ocr";
                Encoding encoding = Encoding.Default;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
                request.Method = "post";
                request.KeepAlive = true;
                // 图片的base64编码
                string base64 = PathHelper.GetFileBase64(PathHelper.FullPath(GlobalData.SourcePath));
                //String str = HttpUtility.UrlEncode(base64);
                String str = base64;
                byte[] buffer = encoding.GetBytes(str);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string result = reader.ReadToEnd();
                Console.WriteLine("通用文字识别:");
                Console.WriteLine(result);
                var data = JsonSerializer.Deserialize<MyResult>(result);
                data.language = GlobalData.Config.Langs?.FirstOrDefault(p => p.name == TransStrategy.Custom.ToString() && p.ocr == data.language)?.trans;
                if (string.IsNullOrWhiteSpace(data.language))
                    data.language = LangType.en.ToString();
                return data;
                //if (data == null|| data.words_result==null)
                //    return "None";
                //return string.Join(';', data.words_result?.Select(p => p.words));
            }
        }

        public class TransData
        {
            public string from { get; set; }
            public string to { get; set; }
            public Trans_Result[] trans_result { get; set; }
        }

        public class Trans_Result
        {
            public string src { get; set; }
            public string dst { get; set; }
        }

        public class Translator:ITranslator
        {
            static Translator()
            {
                clientId = GlobalData.Config.TransConfig.BaiduTranslateID;
                clientSecret = GlobalData.Config.TransConfig.BaiduTranslateKEY;
            }
            // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
            // 返回token示例
            public static String TOKEN = "24.adda70c11b9786206253ddb70affdc46.2592000.1493524354.282335-1234567";

            // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
            private static String clientId = "百度云应用的AK";
            // 百度云中开通对应服务应用的 Secret Key
            private static String clientSecret = "百度云应用的SK";
            public string to { get; set; }
            public async Task<string> Translate(MyResult src)
            {
                // 原文
                string q = src.text;
                // 源语言
                string from = src.language;
                // 目标语言
                string to = this.to;
                // 改成您的APP ID
                string appId = clientId;
                Random rd = new Random();
                string salt = rd.Next(100000).ToString();
                // 改成您的密钥
                string secretKey = clientSecret;
                string sign = EncryptString(appId + q + salt + secretKey);
                string url = "http://127.0.0.1:5500/trans?";
                url += "input=" + HttpUtility.UrlEncode(q)
                    + "&to=" + HttpUtility.UrlEncode(to);
                Console.WriteLine(url);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                //request.ContentType = "text/html;charset=UTF-8";
                //request.UserAgent = null;
                //request.Timeout = 6000;
                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Stream myResponseStream = response.GetResponseStream();
                //StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.Unicode);
                //string retString = myStreamReader.ReadToEnd();
                //myStreamReader.Close();
                //myResponseStream.Close();
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                HttpClient client = new HttpClient(handler);
                HttpResponseMessage response = client.SendAsync(request).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
                //var data=JsonSerializer.Deserialize<TransData>(result);
                //if (data == null||data.trans_result==null)
                //    return "None";
                //return string.Join(';', data.trans_result?.Select(p => p.dst));
            }
            // 计算MD5值
            public static string EncryptString(string str)
            {
                MD5 md5 = MD5.Create();
                // 将字符串转换成字节数组
                byte[] byteOld = Encoding.UTF8.GetBytes(str);
                // 调用加密方法
                byte[] byteNew = md5.ComputeHash(byteOld);
                // 将加密结果转换为字符串
                StringBuilder sb = new StringBuilder();
                foreach (byte b in byteNew)
                {
                    // 将字节转换成16进制表示的字符串，
                    sb.Append(b.ToString("x2"));
                }
                // 返回加密的字符串
                return sb.ToString();
            }

            public void setTo(string to)
            {
                this.to = to;
            }

            public async Task<string> Translate()
            {
                string token = AccessToken.TOKEN;
                string host = $"{GlobalData.Config.TransConfig.CustomUri}/i2t?to={HttpUtility.UrlEncode(to)}";
                Encoding encoding = Encoding.Default;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
                request.Method = "post";
                request.KeepAlive = true;
                request.Headers.Add("x-api-key", GlobalData.Config.TransConfig.CustomKey);
                // 图片的base64编码
                string base64 = PathHelper.GetFileBase64(PathHelper.FullPath(GlobalData.SourcePath));
                //String str = HttpUtility.UrlEncode(base64);
                String str = base64;
                byte[] buffer = encoding.GetBytes(str);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string result = reader.ReadToEnd();
                Console.WriteLine(result);
                return result;
                //if (data == null|| data.words_result==null)
                //    return "None";
                //return string.Join(';', data.words_result?.Select(p => p.words));
            }
        }
    }
}
