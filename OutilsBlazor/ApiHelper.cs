using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace OutilsBlazor
{
    public abstract class ApiHelper
    {
        public abstract string ApiKey { get; }

        public abstract string UrlBase { get;}

        public abstract string Uri { get; }

        public string Adresse => $"{UrlBase}/{Uri}";

        public TData Get<TData>(string url, List<KeyValuePair<string, object>> param = null) where TData : class, new()
        {
            if (param == null)
                param = new List<KeyValuePair<string, object>>();
            param.Add(new KeyValuePair<string, object>("key", ApiKey));

            param.ForEach(x => url += $"&{x.Key}={x.Value}");

            TData data;

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = WebRequestMethods.Http.Get;

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
            }

            return data;
        }
    }
}
