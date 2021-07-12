using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Adyen.Security;

namespace Adyen.HttpClient
{
    public static class HttpClientExtensions
    {
        public static HttpMessageHandler ConfigureHttpMessageHandler(Config config)
        {
            return new HttpClientHandler
            {
                Proxy = config.Proxy,
                UseProxy = config.Proxy != null,
                ServerCertificateCustomValidationCallback = (message, certificate, chain, policy) => ServerCertificateValidationCallback(message, certificate, chain, policy, config.Environment),
                UseDefaultCredentials = !config.HasApiKey && config.HasPassword
            };
        }

        public static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors, Adyen.Model.Enum.Environment environment)
        {
            switch (sslPolicyErrors)
            {
                case SslPolicyErrors.None:
                    return true;
                case SslPolicyErrors.RemoteCertificateNameMismatch:
                    return TerminalCommonNameValidator.ValidateCertificate(certificate.Subject, environment);
                default:
                    return false;
            }
        }
    }
}