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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Adyen.HttpClient.Interfaces;
using Adyen.Model;

namespace Adyen.HttpClient
{
    public class HttpClientWrapper : IClient
    {
        private readonly Config _config;

        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly System.Net.Http.HttpClient _httpClient;

        public HttpClientWrapper(Config config, System.Net.Http.HttpClient httpClient)
        {
            //Set security protocol. Only TLS1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _config = config;
            _httpClient = httpClient;
        }

        public string Request(string endpoint, string json, bool isApiKeyRequired, RequestOptions requestOptions = null)
        {
            return RequestAsync(endpoint, json, isApiKeyRequired, requestOptions).GetAwaiter().GetResult();
        }

        public async Task<string> RequestAsync(string endpoint, string json, bool isApiKeyRequired, RequestOptions requestOptions = null)
        {
            using (var request = GetHttpRequestMessage(endpoint, isApiKeyRequired, json, requestOptions))
            using (var httpResponseMessage = await _httpClient.SendAsync(request))
            {
                var responseText = await httpResponseMessage.Content.ReadAsStringAsync();
                httpResponseMessage.EnsureSuccessStatusCode();
                return responseText;
            }
        }

        public string Post(string endpoint, Dictionary<string, string> postParameters)
        {
            var dictToString = QueryString(postParameters);

            var content = new StringContent(dictToString, _encoding, "application/x-www-form-urlencoded");
            using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint) {Content = content})
            using (var response = _httpClient.SendAsync(request).GetAwaiter().GetResult())
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
        }

        public HttpRequestMessage GetHttpRequestMessage(string endpoint, bool isApiKeyRequired, string json, RequestOptions requestOptions)
        {
            var httpWebRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(json, _encoding, "application/json")
            };
            httpWebRequest.Headers.Add("ContentType", "application/json");
            httpWebRequest.Headers.Add("Accept-Charset", "UTF-8");
            httpWebRequest.Headers.Add("Cache-Control", "no-cache");
            httpWebRequest.Headers.Add("UserAgent", $"{_config.ApplicationName} {ClientConfig.UserAgentSuffix}{ClientConfig.LibVersion}");
            if (!string.IsNullOrWhiteSpace(requestOptions?.IdempotencyKey))
            {
                httpWebRequest.Headers.Add("Idempotency-Key", requestOptions.IdempotencyKey);
            }

            //Use one of two authentication method.
            if (isApiKeyRequired || _config.HasApiKey)
            {
                httpWebRequest.Headers.Add("x-api-key", _config.XApiKey);
            }
            else if (_config.HasPassword)
            {
                var authString = _config.Username + ":" + _config.Password;
                var bytes = Encoding.UTF8.GetBytes(authString);
                var credentials = Convert.ToBase64String(bytes);
                httpWebRequest.Headers.Add("Authorization", "Basic " + credentials);
            }

            return httpWebRequest;
        }

        public static string QueryString(IDictionary<string, string> dict)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in dict)
            {
                stringBuilder.Append(item.Key);
                stringBuilder.Append('=');
                stringBuilder.Append(HttpUtility.UrlEncode(item.Value));
                stringBuilder.Append('&');
            }

            if (stringBuilder.Length > 0)
            {
                //remove trailing &amp
                stringBuilder.Remove(stringBuilder.Length - 2, 1);
            }

            return stringBuilder.ToString();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}