#if NETMF4
using System;
using System.Net;
using System.Threading;
using System.Collections;
using System.IO;

// MicroFramework 4 and no SSL
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.SPOT;

namespace Standard.Web.Mqtt
{
	/// <summary>
	/// MQTT Client
	/// </summary>
	partial class MqttClient
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="brokerHostName">Broker Host Name or IP Address</param>
		public MqttClient(string brokerHostName)
			: this(brokerHostName, MqttSettings.MQTT_BROKER_DEFAULT_PORT, false)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="brokerHostName">Broker Host Name or IP Address</param>
		public MqttClient(string brokerHostName, int brokerPort)
			: this(brokerHostName, brokerPort, false)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="brokerHostName">Broker Host Name or IP Address</param>
		/// <param name="brokerPort">Broker port</param>
		/// <param name="secure">Using secure connection</param>
		/// <param name="sslProtocol">SSL/TLS protocol version</param>
		/// <param name="caCert">CA certificate for secure connection</param>
		/// <param name="clientCert">Client certificate</param>
		protected MqttClient(string brokerHostName, int brokerPort, bool customChannel)
		{
			this.Init(brokerHostName, brokerPort);
			if (!customChannel)
				InitChannel();
		}

		/// <summary>
		/// MqttClient initialization
		/// </summary>
		/// <param name="brokerHostName">Broker Host Name or IP Address</param>
		/// <param name="brokerPort">Broker port</param>
		/// <param name="caCert">CA certificate for secure connection</param>
		/// <param name="clientCert">Client certificate</param>
		/// <param name="sslProtocol">SSL/TLS protocol version</param>
		private void InitChannel()
		{
			// create network channel
			this.channel = new MqttNetworkChannel(this.brokerHostName, this.brokerPort);
		}

		private bool ReceiveThreadIsFatalException(Exception e)
		{
			// added for SSL/TLS incoming connection that use SslStream that wraps SocketException 
			if ((e.GetType() == typeof(IOException)) || (e.GetType() == typeof(SocketException)) ||
				((e.InnerException != null) && (e.InnerException.GetType() == typeof(SocketException))))
			{
				return true;
			}
			return false;
		}

		private void SendReceiveConnectionResetUpdate(Exception e)
		{ }

		private bool IsInflightMessage(string key)
		{
			return (this.session.InflightMessages.Contains(key));
		}

		private bool WaitServerHandleResponse(int timeout, string target)
		{
			if (target == "broker")
			{
				// wait for answer from broker
				return this.syncEndReceiving.WaitOne(timeout, false);
			}
			else if (target == "keepAlive")
			{
				return this.keepAliveEvent.WaitOne(timeout, false);
			}
			else if (target == "inflight")
			{
				return this.inflightWaitHandle.WaitOne(timeout, false);
			}
			return false;
		}
	}
}
#endif
