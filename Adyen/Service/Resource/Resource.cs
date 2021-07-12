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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adyen.Service.Resource
{
    public class Resource
    {
        private AbstractService _abstractService;
        protected string Endpoint;
        protected List<string> RequiredFields;

        public Resource(AbstractService abstractService, string endpoint, List<string> requiredFields)
        {
            _abstractService = abstractService;
            Endpoint = endpoint;
            RequiredFields = requiredFields;
        }

        public string Request(string json)
        {
            var clientInterface = this._abstractService.Client.HttpClient;
            return clientInterface.Request(this.Endpoint, json, false, null);
        }

        public async Task<string> RequestAsync(string json)
        {
            var clientInterface = this._abstractService.Client.HttpClient;
            return await clientInterface.RequestAsync(this.Endpoint, json, false, null);
        }
    }
}