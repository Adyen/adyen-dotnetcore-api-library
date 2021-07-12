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
using Adyen.Constants.HPPConstants;
using Adyen.Model.Hpp;
using Adyen.Util;
using System;
using System.Collections.Generic;

namespace Adyen.Service
{
    public class HostedPaymentPages : AbstractService
    {
        public HostedPaymentPages(Client client)
            : base(client)
        {
        }

        public string DirectoryLookup(Dictionary<string, string> postParameters)
        {
            var config = this.Client.Config;
            var clientInterface = this.Client.HttpClient;
            var endpoint = config.HppEndpoint + "/directory.shtml";
            return clientInterface.Post(endpoint, postParameters);
        }

        public Dictionary<string, string> GetPostParametersFromDlRequest(DirectoryLookupRequest request)
        {
            var config = this.Client.Config;

            var postParameters = new Dictionary<string, string>
            {
                {Fields.CurrencyCode, request.CurrencyCode },
                {Fields.MerchantReference, request.MerchantReference},
                {Fields.SessionValidity, request.SessionValidity},
                {Fields.CountryCode, request.CountryCode}
            };

            if (!string.IsNullOrEmpty(request.MerchantAccount))
            {
                postParameters.Add(Fields.MerchantAccount, request.MerchantAccount);
            }
            else
            {
                postParameters.Add(Fields.MerchantAccount, config.MerchantAccount);
            }
            postParameters.Add(Fields.PaymentAmount, request.PaymentAmount);

            if (!string.IsNullOrEmpty(request.SkinCode))
            {
                postParameters.Add(Fields.SkinCode, request.SkinCode);
            }
            else
            {
                postParameters.Add(Fields.SkinCode, config.SkinCode);
            }

           

            var hmacValidator = new HmacValidator();

            var dataToSign = hmacValidator.BuildSigningString(postParameters);

            string hmacKey;
            if (!string.IsNullOrEmpty(request.HmacKey))
            {
                hmacKey = request.HmacKey;
            }
            else
            {
                hmacKey = config.HmacKey;
            }

            var merchantSig = hmacValidator.CalculateHmac(dataToSign, hmacKey);
            postParameters.Add(Fields.MerchantSig, merchantSig);

            return postParameters;
        }

        public List<PaymentMethod> GetPaymentMethods(DirectoryLookupRequest request)
        {
            try
            {
                var postParameters = GetPostParametersFromDlRequest(request);
                var jsonResult = DirectoryLookup(postParameters);
                var directoryLookupResult = Util.JsonOperation.Deserialize<DirectoryLookupResult>(jsonResult);

                return directoryLookupResult.PaymentMethods;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
