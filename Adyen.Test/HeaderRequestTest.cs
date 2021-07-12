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

using Adyen.HttpClient;
using Adyen.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adyen.Test
{
    [TestClass]
    public class HeaderRequestTest
    {
        private const string _endpoint = "https://endpoint:8080";

        public HeaderRequestTest()
        {

        }
        [TestMethod]
        public void BasicAuthenticationHeadersTest()
        {
            var client = new HttpWebRequestWrapper(MockPaymentData.CreateConfingMock());
            var httpWebRequest = client.GetHttpWebRequest(_endpoint, false);

            Assert.IsNotNull(httpWebRequest.UserAgent);
            Assert.AreEqual(httpWebRequest.Address, _endpoint);
            Assert.AreEqual(httpWebRequest.Headers["Accept-Charset"], "UTF-8");
            Assert.AreEqual(httpWebRequest.Headers["Cache-Control"], "no-cache");
            Assert.AreEqual(httpWebRequest.ContentType, "application/json");

            Assert.IsNotNull(httpWebRequest.Headers["Authorization"]);
            Assert.IsTrue(httpWebRequest.UseDefaultCredentials);

            Assert.AreEqual(httpWebRequest.Headers["Cache-Control"], "no-cache");
            Assert.IsNull(httpWebRequest.Headers["x-api-key"]);
        }

        [TestMethod]
        public void ApiKeyBasedHeadersTest()
        {
            var client = new HttpWebRequestWrapper(MockPaymentData.CreateConfingApiKeyBasedMock());
            var httpWebRequest = client.GetHttpWebRequest(_endpoint, true);

            Assert.IsNotNull(httpWebRequest.UserAgent);
            Assert.AreEqual(httpWebRequest.Address, _endpoint);
            Assert.AreEqual(httpWebRequest.Headers["Accept-Charset"], "UTF-8");
            Assert.AreEqual(httpWebRequest.Headers["Cache-Control"], "no-cache");
            Assert.AreEqual(httpWebRequest.ContentType, "application/json");

            Assert.IsNull(httpWebRequest.Headers["Authorization"]);
            Assert.IsFalse(httpWebRequest.UseDefaultCredentials);

            Assert.IsNotNull(httpWebRequest.Headers["x-api-key"]);
            Assert.AreEqual(httpWebRequest.Headers["x-api-key"], "AQEyhmfxK....LAG84XwzP5pSpVd");
        }

        [TestMethod]
        public void IdempotencyKeyNotPresentInHeaderIfExcluded()
        {
            var client = new HttpWebRequestWrapper(MockPaymentData.CreateConfingApiKeyBasedMock());
            var httpWebRequest = client.GetHttpWebRequest(_endpoint, true);

            Assert.IsNull(httpWebRequest.Headers["Idempotency-Key"]);
        }

        [TestMethod]
        public void IdempotencyKeyNotPresentInHeaderIfNull()
        {
            var client = new HttpWebRequestWrapper(MockPaymentData.CreateConfingApiKeyBasedMock());
            var httpWebRequest = client.GetHttpWebRequest(_endpoint, true, null);

            Assert.IsNull(httpWebRequest.Headers["Idempotency-Key"]);
        }

        [TestMethod]
        public void IdempotencyKeyNotPresentInHeaderIfEmptyString()
        {
            var client = new HttpWebRequestWrapper(MockPaymentData.CreateConfingApiKeyBasedMock());
            var requestOptions = new RequestOptions
            {
                IdempotencyKey = string.Empty
            };

            var httpWebRequest = client.GetHttpWebRequest(_endpoint, true, requestOptions);

            Assert.IsNull(httpWebRequest.Headers["Idempotency-Key"]);
        }

        [TestMethod]
        public void IdempotencyKeyNotPresentInHeaderIfWhitespaceString()
        {
            var client = new HttpWebRequestWrapper(MockPaymentData.CreateConfingApiKeyBasedMock());
            var requestOptions = new RequestOptions
            {
                IdempotencyKey = " "
            };

            var httpWebRequest = client.GetHttpWebRequest(_endpoint, true, requestOptions);

            Assert.IsNull(httpWebRequest.Headers["Idempotency-Key"]);
        }

        [TestMethod]
        public void IdempotencyKeyPresentInHeaderIfSpecified()
        {
            var client = new HttpWebRequestWrapper(MockPaymentData.CreateConfingApiKeyBasedMock());
            var requestOptions = new RequestOptions
            {
                IdempotencyKey = "idempotencyKey"
            };

            var httpWebRequest = client.GetHttpWebRequest(_endpoint, true, requestOptions);

            Assert.IsNotNull(httpWebRequest.Headers["Idempotency-Key"]);
            Assert.AreEqual(requestOptions.IdempotencyKey, httpWebRequest.Headers["Idempotency-Key"]);
        }
    }
}
