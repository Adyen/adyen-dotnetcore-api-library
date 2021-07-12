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

using System;
using System.Threading;
using Adyen.Constants;
using Adyen.HttpClient.Interfaces;
using Adyen.HttpClient;
using Adyen.Exceptions;
using Environment = Adyen.Model.Enum.Environment;

namespace Adyen
{
    public class Client
    {
        private System.Net.Http.HttpClient _httpClient;
        private Lazy<IClient> _lazyClient;

        public Config Config { get; set; }

        public string ApplicationName { get; set; }

        public delegate void CallbackLogHandler(string message);

        public event CallbackLogHandler LogCallback;

        public Client(string username, string password, Environment environment, System.Net.Http.HttpClient httpClient = null)
        {
            Config = new Config
            {
                Username = username,
                Password = password,
                Environment = environment
            };
            _httpClient = httpClient;
            this.SetEnvironment(environment);
        }

        public Client(string xapikey, Environment environment, System.Net.Http.HttpClient httpClient = null)
        {
            Config = new Config
            {
                Environment = environment,
                XApiKey = xapikey
            };
            _httpClient = httpClient;
            this.SetEnvironment(environment);
        }

        public Client(string xapikey, Environment environment, string liveEndpointUrlPrefix, System.Net.Http.HttpClient httpClient = null)
        {
            Config = new Config
            {
                Environment = environment,
                XApiKey = xapikey
            };
            _httpClient = httpClient;
            this.SetEnvironment(environment,liveEndpointUrlPrefix);
        }

        public Client(Config config, System.Net.Http.HttpClient httpClient = null)
        {
            Config = config;
            _httpClient = httpClient;
        }

        public void SetEnvironment(Environment environment)
        {
            SetEnvironment(environment, null);
        }

        public void SetEnvironment(Environment environment, string liveEndpointUrlPrefix)
        {
            Config.Environment = environment;
            switch (environment)
            {
                case Environment.Test:
                    Config.Endpoint = ClientConfig.EndpointTest;
                    Config.HppEndpoint = ClientConfig.HppTest;
                    Config.CloudApiEndPoint = ClientConfig.CloudApiEndPointTest;
                    Config.CheckoutEndpoint = ClientConfig.CheckoutEndpointTest;
                    Config.MarketPayEndpoint = ClientConfig.MarketpayEndPointTest;
                    Config.PosTerminalManagementEndpoint = ClientConfig.PosTerminalManagementEndpointTest;
                    break;
                case Environment.Live:
                    if (string.IsNullOrEmpty(liveEndpointUrlPrefix))
                    {
                        throw new InvalidOperationException(ExceptionMessages.MissingLiveEndpointUrlPrefix);
                    }

                    Config.Endpoint = ClientConfig.EndpointProtocol + liveEndpointUrlPrefix + ClientConfig.EndpointLiveSuffix;
                    Config.HppEndpoint = ClientConfig.HppLive;
                    Config.CloudApiEndPoint = ClientConfig.CloudApiEndPointLive;
                    Config.CheckoutEndpoint = ClientConfig.EndpointProtocol + liveEndpointUrlPrefix + ClientConfig.CheckoutEndpointLiveSuffix;
                    Config.MarketPayEndpoint = ClientConfig.MarketpayEndPointLive;
                    Config.PosTerminalManagementEndpoint = ClientConfig.PosTerminalManagementEndpointLive;
                    break;
            }

            ReloadClient();
        }

        private void ReloadClient()
        {
            if (_lazyClient != null && _lazyClient.IsValueCreated)
            {
                _lazyClient.Value.Dispose();
            }

            _lazyClient = new Lazy<IClient>(() =>
                _httpClient != null
                ? new HttpClientWrapper(Config, _httpClient)
                : (IClient)new HttpWebRequestWrapper(Config),
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        public IClient HttpClient
        {
            get => _lazyClient.Value;
            set
            {
                _lazyClient = new Lazy<IClient>(() => value, LazyThreadSafetyMode.ExecutionAndPublication);
            }
        }

        public string ApiVersion
        {
            get
            {
                return ClientConfig.ApiVersion;
            }
        }

        public string RecurringApiVersion
        {
            get
            {
                return ClientConfig.RecurringApiVersion;
            }
        }

        public string LibraryVersion
        {
            get
            {
                return ClientConfig.LibVersion;

            }
        }

        public void LogLine(string message)
        {
            if (LogCallback != null)
            {
                LogCallback(message);
            }
        }
    }
}
