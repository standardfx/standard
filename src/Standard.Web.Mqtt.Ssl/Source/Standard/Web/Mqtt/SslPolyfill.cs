using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Standard.Web.Mqtt
{
	internal static class SslPolyfill
	{
#if NETSTANDARD
		public static void AuthenticateAsClient(this SslStream stream, string targetHost, X509CertificateCollection clientCertificates, bool checkCertificateRevocation)
		{
			SslProtocols enabledSslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
			stream.AuthenticateAsClientAsync(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation).Wait();
			return;
		}

		public static void AuthenticateAsClient(this SslStream stream, string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			stream.AuthenticateAsClientAsync(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation).Wait();
			return;
		}

		public static void AuthenticateAsServer(this SslStream stream, X509Certificate serverCertificate)
		{
			stream.AuthenticateAsServerAsync(serverCertificate).Wait();
			return;
		}

		public static void AuthenticateAsServer(this SslStream stream, X509Certificate serverCertificate, bool clientCertificateRequired, bool checkCertificateRevocation)
		{
			SslProtocols enabledSslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
			stream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation).Wait();
			return;
		}

		public static void AuthenticateAsServer(this SslStream stream, X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			stream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation).Wait();
			return;
		}
#endif
	}
}

