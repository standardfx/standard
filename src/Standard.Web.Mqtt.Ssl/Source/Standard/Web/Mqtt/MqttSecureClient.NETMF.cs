#if NETMF4
using System;
using System.Net;
using System.Threading;
using System.Collections;
using System.IO;

// MicroFramework 4 and SSL
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.Security;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Net.Security;

namespace Standard.Web.Mqtt
{
	/// <summary>
	/// MQTT secure client
	/// </summary>
	partial class MqttSecureClient
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="brokerHostName">Broker Host Name or IP Address</param>
		/// <param name="brokerPort">Broker port</param>
		/// <param name="secure">Using secure connection</param>
		/// <param name="sslProtocol">SSL/TLS protocol version</param>
		/// <param name="caCert">CA certificate for secure connection</param>
		/// <param name="clientCert">Client certificate</param>
		public MqttSecureClient(string brokerHostName, int brokerPort, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol)
			: base(brokerHostName, brokerPort, true)
		{
			this.InitSecureChannel(caCert, clientCert, sslProtocol);
		}

		/// <summary>
		/// MqttClient initialization
		/// </summary>
		/// <param name="caCert">CA certificate for secure connection</param>
		/// <param name="clientCert">Client certificate</param>
		/// <param name="sslProtocol">SSL/TLS protocol version</param>
		private void InitSecureChannel(X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol)
		{
			// create network channel
			this.channel = new MqttNetworkChannel(this.brokerHostName, this.brokerPort, caCert, clientCert, sslProtocol);
		}
	}
}
#endif
