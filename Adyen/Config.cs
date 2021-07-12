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
using System.Net;
using Environment = Adyen.Model.Enum.Environment;

namespace Adyen
{
    public class Config
    {
        //Merchant details
        public string Username { get; set; }
        public string Password { get; set; }
        public string MerchantAccount { get; set; }
        public Environment Environment { get; set; }
        public string Endpoint { get; set; }
        public string MarketPayEndpoint { get; set; }
        public string ApplicationName { get; set; }
        public IWebProxy Proxy { get; set; }
        // Please note that http request timeout is milliseconds.
        public int HttpRequestTimeout { get; set; }
        //HPP specific
        public string HppEndpoint { get; set; }
        public string SkinCode{ get; set; }
        public string HmacKey { get; set; }
        public string CheckoutEndpoint { get;set; }

        //Terminal cloud api
        public string XApiKey { get; set; }
        public string CloudApiEndPoint { get; set; }
        [Obsolete("This is deprecated property by Adyen.")]
        public bool SkipCertValidation { get; set; } = false;

        //POS Terminal Management
        public string PosTerminalManagementEndpoint { get; set; }


        public bool HasPassword => !string.IsNullOrEmpty(Password);
        public bool HasApiKey => !string.IsNullOrEmpty(XApiKey);
    }
}