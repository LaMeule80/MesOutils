using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace LegoBlazor.API
{
    public class SyncHttpClient
    {
        public TData Get<TData>(string url) where TData : class, new()
        {
            TData data = null;

            HttpWebRequest request = CreateRequest(HttpMethod.Get, url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string responseJSON = myStreamReader.ReadToEnd();
                        data = JsonConvert.DeserializeObject<TData>(responseJSON);
                    }
                }

                request = null;
            }

            return data;
        }

        private HttpWebRequest CreateRequest(HttpMethod method, string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method.ToString().ToUpper();

            return request;
        }
    }
}