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
		/// <param name="brokerPort">Broker port</param>
		public MqttClient(string brokerHostName, int brokerPort)
			: this(brokerHostName, brokerPort, false)
		{ }

		protected MqttClient(string brokerHostName, int brokerPort, bool customChannel)
		{
			this.Init(brokerHostName, brokerPort);
			if (!customChannel)
				this.InitChannel();
		}

		private void InitChannel()
		{
			// create network channel
			this.channel = new MqttNetworkChannel(this.brokerHostName, this.brokerPort);
		}

		private bool ReceiveThreadIsFatalException(Exception e)
		{
			return false;
		}

		private void SendReceiveConnectionResetUpdate(Exception e)
		{ }

		private bool IsInflightMessage(string key)
		{
			(this.session.InflightMessages.ContainsKey(key));
		}

		private bool WaitServerHandleResponse(int timeout, string target)
		{
			if (target == "broker")
			{
				// wait for answer from broker
				return this.syncEndReceiving.WaitOne(timeout);
			}
			else if (target == "keepAlive")
			{
				return this.keepAliveEvent.WaitOne(timeout);
			}
			else if (target == "inflight")
			{
				return this.inflightWaitHandle.WaitOne(timeout);
			}
			return false;
		}
	}
}
#endif
