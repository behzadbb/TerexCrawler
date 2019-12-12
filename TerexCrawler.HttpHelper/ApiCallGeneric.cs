
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace erexCrawler.HttpHelper.ApiCall
{
    public class ApiCallGeneric
    {
        public static T ApiCall<T>(string url, object obj = null)
        {
            T result = default(T);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string strApiUrl = string.Format("{0}", url);
                    string json = string.Empty;
                    if (obj != null)
                        json = JsonConvert.SerializeObject(obj);
                    StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var data = client.PostAsync(strApiUrl, content).Result;
                    if (data.IsSuccessStatusCode)
                    {
                        if(string.IsNullOrEmpty(data.Content.ReadAsStringAsync().Result))
                            return default(T);

                        result = JsonConvert.DeserializeObject<T>(data.Content.ReadAsStringAsync().Result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
            return result;
        }
    }
}
