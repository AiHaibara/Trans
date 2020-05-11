using Google.Cloud.Translation.V2;
using System;
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
    public class GoogleTrans : ITrans
    {
        public ITranslator translator { get; set; }
        public IOcror ocror { get; set; }
        public TransStrategy Strategy => TransStrategy.Google;

        public GoogleTrans()
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
            public int? error_code { get; set; }
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
                //AccessToken.getAccessToken();
            }
            bool flag = false;
            // 通用文字识别
            public MyResult CropImage()
            {
                string token = AccessToken.TOKEN;
                string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic?access_token=" + token + "&detect_language=true";
                Encoding encoding = Encoding.Default;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
                request.Method = "post";
                request.KeepAlive = true;
                // 图片的base64编码
                string base64 = getFileBase64(PathHelper.FullPath(GlobalData.SourcePath));
                String str = "image=" + HttpUtility.UrlEncode(base64);
                byte[] buffer = encoding.GetBytes(str);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string result = reader.ReadToEnd();
                Console.WriteLine("通用文字识别:");
                Console.WriteLine(result);
                var data = JsonSerializer.Deserialize<BaiduOcrResult>(result);
                if (data.error_code != null)
                {
                    if (!flag)
                    {
                        AccessToken.getAccessToken();
                        flag = true;
                    }
                    else
                    {
                        throw new Exception("application id or secret not valid");
                    }
                    return CropImage();
                }
                flag = false;
                if (data == null || data.words_result == null)
                    return new MyResult { text = "None", language = "auto" };
                return new MyResult { text = string.Join(';', data.words_result?.Select(p => p.words)), language = "auto" };
            }

            public static String getFileBase64(String fileName)
            {
                FileStream filestream = new FileStream(fileName, FileMode.Open);
                byte[] arr = new byte[filestream.Length];
                filestream.Read(arr, 0, (int)filestream.Length);
                string baser64 = Convert.ToBase64String(arr);
                filestream.Close();
                return baser64;
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

        public class Translator: ITranslator
        {
            //public string TranslateText(MyResult src)
            //{
            //    Console.OutputEncoding = System.Text.Encoding.UTF8;
            //    TranslationClient client = TranslationClient.Create();
            //    var response = client.TranslateText(
            //        text: "Hello World.",
            //        targetLanguage: "ru",  // Russian
            //        sourceLanguage: "en");  // English
            //    Console.WriteLine(response.TranslatedText);
            //    return response.TranslatedText;
            //}
            static Translator()
            {
                clientSecret = GlobalData.Config.TransConfig.GoogleTranslateKEY;
            }
            private static String clientSecret = "GoolgeTranslateKey";
            public string to { get; set; }
            public async Task<string> Translate(MyResult src)
            {
                //Console.OutputEncoding = System.Text.Encoding.UTF8;

                TranslationClient client = TranslationClient.CreateFromApiKey(clientSecret);
                var detection = client.DetectLanguage(text: src.text);
                var response = client.TranslateText(
                    text: src.text,
                    targetLanguage: to,  
                    sourceLanguage: detection.Language);  
                //Console.WriteLine(response.TranslatedText);
                var result = response.TranslatedText;
                if (result == null)
                    return "None";
                return result;
            }
            public void setTo(string to)
            {
                this.to = to;
            }
        }
    }
}
