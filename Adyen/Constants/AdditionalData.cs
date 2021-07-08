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

namespace Adyen.Constants
{
    public class AdditionalData
    {
        public const string RefusalReasonRaw = "refusalReasonRaw";
        public const string PaymentMethod = "paymentMethod";
        public const string ExpiryDate = "expiryDate";
        public const string CardBin = "cardBin";
        public const string CardHolderName = "cardHolderName";
        public const string CardSummary = "cardSummary";
        public const string ThreeDOfferered = "threeDOffered";
        public const string ThreeDAuthenticated = "threeDAuthenticated";
        public const string AvsResult = "avsResult";
        public const string PaymentToken = "payment.token";
        public const string BoletoBarcodeReference = "boletobancario.barCodeReference";
        public const string BoletoData = "boletobancario.data";
        public const string BoletoDueDate = "boletobancario.dueDate";
        public const string BoletoUrl = "boletobancario.url";
        public const string BoletoExpirationDate = "boletobancario.expirationDate";
        public const string HmacSignature = "hmacSignature";
        public const string AuthorisedAmountCurrency = "authorisedAmountCurrency";
        public const string AuthorisedAmountValue = "authorisedAmountValue";
        public const string RecurringShopperReference = "recurring.shopperReference";
        public const string RecurringDetailReference = "recurring.recurringDetailReference";
        public const string AllowThreeDS2 = "allow3DS2";
        public const string PaymentLinkId = "paymentLinkId";
        public const string MerchantOrderReference = "merchantOrderReference";
    }
}
