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
    public class TranslationResult
    {
        public DetectedLanguage DetectedLanguage { get; set; }
        public TextResult SourceText { get; set; }
        public Translation[] Translations { get; set; }
    }

    public class DetectedLanguage
    {
        public string Language { get; set; }
        public float Score { get; set; }
    }

    public class TextResult
    {
        public string Text { get; set; }
        public string Script { get; set; }
    }

    public class Translation
    {
        public string Text { get; set; }
        public TextResult Transliteration { get; set; }
        public string To { get; set; }
        public Alignment Alignment { get; set; }
        public SentenceLength SentLen { get; set; }
    }

    public class Alignment
    {
        public string Proj { get; set; }
    }

    public class SentenceLength
    {
        public int[] SrcSentLen { get; set; }
        public int[] TransSentLen { get; set; }
    }
    public class MicrosoftTrans : ITrans
    {
        public ITranslator translator { get; set; }
        public IOcror ocror { get; set; }
        public TransStrategy Strategy => TransStrategy.Microsoft;

        public MicrosoftTrans()
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
            public int? error_code { get; set; }
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
                clientId = GlobalData.Config.TransConfig.BaiduTranslateID;
                clientSecret = GlobalData.Config.TransConfig.MicrosoftTranslateKEY;
            }
            // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
            // 返回token示例
            public static String TOKEN = "24.adda70c11b9786206253ddb70affdc46.2592000.1493524354.282335-1234567";

            // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
            private static String clientId = "百度云应用的AK";
            // 百度云中开通对应服务应用的 Secret Key
            private static String clientSecret = "GoolgeTranslateKey";
            public string to { get; set; }

            public async Task<string> TranslateTextRequest(string subscriptionKey, string endpoint, string route, string inputText)
            {
                object[] body = new object[] { new { Text = inputText } };
                var requestBody = JsonSerializer.Serialize(body);

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // Build the request.
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(endpoint + route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                    // Send the request and get response.
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string.
                    string result = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    TranslationResult[] deserializedOutput = JsonSerializer.Deserialize<TranslationResult[]>(result, options);
                    //// Iterate over the deserialized results.
                    //foreach (TranslationResult o in deserializedOutput)
                    //{
                    //    // Print the detected input languge and confidence score.
                    //    Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", o.DetectedLanguage.Language, o.DetectedLanguage.Score);
                    //    // Iterate over the results and print each translation.
                    //    foreach (Translation t in o.Translations)
                    //    {
                    //        Console.WriteLine("Translated to {0}: {1}", t.To, t.Text);
                    //    }
                    //}
                    if (deserializedOutput == null)
                        return "None";
                    return string.Join(';', deserializedOutput.Select(q=> string.Join(';', q.Translations?.Select(p => p.Text??"")??new List<string>() { "" })));
                }
            }
            private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com";
            public async Task<string> Translate(MyResult src)
            {
                // This is our main function.
                // Output languages are defined in the route.
                // For a complete list of options, see API reference.
                // https://docs.microsoft.com/azure/cognitive-services/translator/reference/v3-0-translate
                string route = $"/translate?api-version=3.0&to={to}";
                var result = await TranslateTextRequest(clientSecret, endpoint, route, src.text);
                if (result == null)
                    return "None";
                return result;
            }

            public void setTo(string to)
            {
                this.to = to;
            }

            public Task<string> Translate()
            {
                throw new NotImplementedException();
            }
        }
    }
}
