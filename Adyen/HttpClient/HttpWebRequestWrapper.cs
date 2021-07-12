#region License
// /*
//  *                       ######
//  *                       ######
//  * ############    ####( ######  #####. ######  ############   ############
//  * #############  #####( ######  #####. ######  #############  #############
//  *        ######  #####( ######  #####. ######  #####  ######  #####  ######
//  * ###### ######  #####( ######  #####. ######  #####  #####   #####  ######
//  * ###### ######  #####( ######  #####. ######  #####          #####  ######
//  * #############  #############  #############  #############  #####  ######
//  *  ############   ############  #############   ############  #####  ######
//  *                                      ######
//  *                               #############
//  *                               ############
//  *
//  * Adyen Dotnet API Library
//  *
//  * Copyright (c) 2020 Adyen B.V.
//  * This file is open source and available under the MIT license.
//  * See the LICENSE file for more info.
//  */
#endregion

using Adyen.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Adyen.HttpClient.Interfaces;
using Adyen.Model;

namespace Adyen.HttpClient
{
    public class HttpWebRequestWrapper : IClient
    {
        private readonly Config _config;
        private readonly Encoding _encoding = Encoding.UTF8;

        public HttpWebRequestWrapper(Config config)
        {
            _config = config;
        }

        public string Request(string endpoint, string json, bool isApiKeyRequired, RequestOptions requestOptions = null )
        {
            string responseText = null;
            var httpWebRequest = GetHttpWebRequest(endpoint, isApiKeyRequired, requestOptions );
            if (_config.HttpRequestTimeout > 0)
            {
                httpWebRequest.Timeout = _config.HttpRequestTimeout;
            }
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (var response = (HttpWebResponse) httpWebRequest.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), _encoding))
                    {
                        responseText = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                HandleWebException(e);
            }
            return responseText;
        }

        /// <summary>
        /// HttpWebRequest asynchronous.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="json"></param>
        /// <param name="isApiKeyRequired"></param>
        /// <param name="requestOptions">Optional parameter used to specify the options for the request</param>
        /// <returns>Task<string></returns>
        public async Task<string> RequestAsync(string endpoint, string json, bool isApiKeyRequired, RequestOptions requestOptions = null)
        {
            string responseText = null;
            //Set security protocol. Only TLS1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpWebRequest = GetHttpWebRequest(endpoint, isApiKeyRequired, requestOptions);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (var response = (HttpWebResponse)await httpWebRequest.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), _encoding))
                    {
                        responseText = await reader.ReadToEndAsync();
                    }
                }
            }
            catch (WebException e)
            {
                HandleWebException(e);
            }
            return responseText;
        }

        /// <summary>
        /// Handles the common http exceptions
        /// </summary>
        /// <param name="e">WebException e</param>
        private static void HandleWebException(WebException e)
        {
            string responseText = null;
            //Add check on actual timeout before returning timeout
            if (e.Response == null && e.Status == WebExceptionStatus.Timeout)
            {
                throw new HttpClientException((int)HttpStatusCode.RequestTimeout, "HTTP Exception timeout", null, "No response", e);
            }
            //May occur for incorrect certificate: in such case the certificate validation failure is mapped to an UnknownError by Microsoft
            if (e.Response == null && e.Status == WebExceptionStatus.UnknownError)
            {
                throw new HttpClientException((int)HttpStatusCode.Ambiguous, "Potential certificate mismatch", null, e.Message, e);
            }
            if (e.Response == null)
            {
                throw new HttpClientException((int)e.Status, "Web Exception", null, "No response", e);
            }
            var response = (HttpWebResponse)e.Response;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }
            throw new HttpClientException((int)response.StatusCode, "HTTP Exception", response.Headers, responseText, e);
        }

        public string Post(string endpoint, Dictionary<string, string> postParameters)
        {
            var dictToString = QueryString(postParameters);
            byte[] postBytes = Encoding.UTF8.GetBytes(dictToString);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = postBytes.Length;
            if (_config.Proxy != null)
            {
                httpWebRequest.Proxy = _config.Proxy;
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(postBytes, 0, postBytes.Length);
            }
            var response = (HttpWebResponse)httpWebRequest.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public HttpWebRequest GetHttpWebRequest(string endpoint, bool isApiKeyRequired, RequestOptions requestOptions = null)
        {
            //Add default headers
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("Accept-Charset", "UTF-8");
            httpWebRequest.Headers.Add("Cache-Control", "no-cache");
            httpWebRequest.UserAgent = $"{_config.ApplicationName} {ClientConfig.UserAgentSuffix}{ClientConfig.LibVersion}";
            if (!string.IsNullOrWhiteSpace(requestOptions?.IdempotencyKey))
            {
                httpWebRequest.Headers.Add("Idempotency-Key", requestOptions?.IdempotencyKey);
            }
            //Use one of two authentication method.
            if (isApiKeyRequired || !string.IsNullOrEmpty(_config.XApiKey))
            {
                httpWebRequest.Headers.Add("x-api-key", _config.XApiKey);
            }
            else if (!string.IsNullOrEmpty(_config.Password))
            {
                var authString = _config.Username + ":" + _config.Password;
                var bytes = Encoding.UTF8.GetBytes(authString);
                var credentials = Convert.ToBase64String(bytes);
                httpWebRequest.Headers.Add("Authorization", "Basic " + credentials);
                httpWebRequest.UseDefaultCredentials = true;
            }
            if (_config.Proxy != null)
            {
                httpWebRequest.Proxy = _config.Proxy;
            }
            httpWebRequest.ServerCertificateValidationCallback = (message, certificate, chain, policy) =>
                HttpClientExtensions.ServerCertificateValidationCallback(message, certificate, chain, policy, _config.Environment);
            return httpWebRequest;
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

        public void Dispose()
        {
        }
    }
}
