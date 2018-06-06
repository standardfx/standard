#if WINRT
using System;
using System.Net;
using System.Threading;
using System.Collections;
using System.IO;

// platform specific
using Windows.Networking.Sockets;

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
		/// <param name="sslProtocol">SSL/TLS protocol version</param>
		public MqttSecureClient(string brokerHostName, int brokerPort, MqttSslProtocols sslProtocol)
			: base(brokerHostName, brokerPort, true)
		{
			this.InitSecureChannel(sslProtocol);
		}

		/// <summary>
		/// MqttClient initialization
		/// </summary>
		/// <param name="sslProtocol">SSL/TLS protocol version</param>
		private void InitSecureChannel(MqttSslProtocols sslProtocol)
		{
			// create network channel
			this.channel = new MqttSecureNetworkChannel(this.brokerHostName, this.brokerPort, sslProtocol);
		}
	}
}
#endif
