using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace ApiAccessLayer
{
    class ApiInformator<DataType>
    {
        private HttpWebRequest request;
        private HttpWebResponse response;
        private StreamReader reader;

        private string path, jsonInfo;

        public ApiInformator(string apiPath)
        {
            path = apiPath;
        }

        public DataType GetInfoInType()
        {
            MakeRequest();
            GetResponse();
            ReadResponse();

            return JsonConvert.DeserializeObject<DataType>(jsonInfo);
        }

        private void MakeRequest()
        {
            request = (HttpWebRequest)WebRequest.Create(path);
        }

        private void GetResponse()
        {
            response = (HttpWebResponse)request.GetResponse();
        }

        private void ReadResponse()
        {
            reader = new StreamReader(response.GetResponseStream());
            jsonInfo = reader.ReadToEnd();
        }
        
        ~ApiInformator()
        {
            try
            {
                reader.Close();
                response.Close();
            }
            catch
            {
            }
        }
    }
}