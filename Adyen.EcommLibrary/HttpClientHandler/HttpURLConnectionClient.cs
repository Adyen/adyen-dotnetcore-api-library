﻿using Adyen.EcommLibrary.Constants;
using Adyen.EcommLibrary.HttpClientHandler.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Adyen.EcommLibrary.HttpClientHandler
{
    public class HttpUrlConnectionClient : IClient
    {
        public string Request(string endpoint, string json, Config config)
        {
            string responseText;
            try
            {
                var url = endpoint;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
<<<<<<< HEAD:Adyen.EcommLibrary/HttpClientHandler/HttpURLConnectionClient.cs
                //Use one of two authentication method.
                if (config.RequiresXApiKey)
                {
                    AddHeaders(config, httpWebRequest);
                }
                else
                {
                    CreateBasicAuthentication(config, httpWebRequest);
                }
=======
                AddHeaders(config, httpWebRequest);
                CreateBasicAuthentication(config, httpWebRequest);
>>>>>>> c8d8d7aa0e209a3eb009ca5104afd52e2233b372:Adyen.EcommLibrary/HttpClientHandler/HttpURLConnectionClient.cs

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var encoding = Encoding.ASCII;
                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return responseText;
        }

        public string Post(string endpoint, Dictionary<string, string> postParameters, Config config)
        {
            string responseText;
            try
            {
                var dictToString = QueryString(postParameters);
                byte[] postBytes = Encoding.ASCII.GetBytes(dictToString);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = postBytes.Length;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(postBytes, 0, postBytes.Length);
                }

                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (HttpClientException ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
            return responseText;
        }
        
        public static string QueryString(IDictionary<string, string> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
            {
                list.Add(item.Key + "=" + HttpUtility.UrlEncode(item.Value));
            }
            return string.Join("&", list);
        }

        #region private message helpers
        /// <summary>
        /// Add headers to the request message
        /// </summary>
        /// <param name="config"></param>
        /// <param name="request"></param>
        private static void AddHeaders(Config config, HttpWebRequest request)
        {
<<<<<<< HEAD:Adyen.EcommLibrary/HttpClientHandler/HttpURLConnectionClient.cs
            if (config.RequiresXApiKey)
            {
                request.Headers.Add("x-api-key", config.XApiKey);
            }
=======
            request.Headers.Add("x-api-key", config.XApiKey);

>>>>>>> c8d8d7aa0e209a3eb009ca5104afd52e2233b372:Adyen.EcommLibrary/HttpClientHandler/HttpURLConnectionClient.cs
            request.Headers.Add("Accept-Charset", "UTF-8");
            request.Headers.Add("Cache-Control", "no-cache");
            request.UserAgent = string.Format("{0} {1}{2}", config.ApplicationName, ClientConfig.UserAgentSuffix, ClientConfig.LibVersion);
        }

        /// <summary>
        /// Create the basic authentication header
        /// </summary>
        /// <param name="config"></param>
        /// <param name="request"></param>
        private static void CreateBasicAuthentication(Config config, HttpWebRequest request)
        {
            var authString = config.Username + ":" + config.Password;
            var bytes = Encoding.ASCII.GetBytes(authString);
            var credentials = Convert.ToBase64String(bytes);

            request.Headers.Add("Authorization", "Basic " + credentials);
            request.UseDefaultCredentials = true;
        }
        #endregion
    }
}