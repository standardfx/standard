#if WINRT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using System.Threading;

namespace Standard.Web.Mqtt
{
    partial class MqttSecureNetworkChannel
    {
        // SSL/TLS protocol version
        private MqttSslProtocol sslProtocol;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket">Socket opened with the client</param>
        public MqttSecureNetworkChannel(StreamSocket socket)
			: base(socket)
        {
            this.sslProtocol = MqttSslProtocol.None;
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="remoteHostName">Remote Host name</param>
		/// <param name="sslProtocol">SSL/TLS protocol version</param>
		public MqttSecureNetworkChannel(string remoteHostName, MqttSslProtocol sslProtocol)
			: this(remoteHostName, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, sslProtocol)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="remoteHostName">Remote Host name</param>
		/// <param name="remotePort">Remote port</param>
		/// <param name="sslProtocol">SSL/TLS protocol version</param>
		public MqttSecureNetworkChannel(string remoteHostName, int remotePort, MqttSslProtocol sslProtocol)
			:base(remoteHostName, remotePort)
        {
            this.sslProtocol = sslProtocol;

            if (secure && (sslProtocol == MqttSslProtocol.None))
                throw new ArgumentException(RS.SslProtocolVersionRequired);
        }

        public override void Connect()
        {
            this.socket = new StreamSocket();

            // connection is executed synchronously
            this.socket.ConnectAsync(this.remoteHostName,
                this.remotePort.ToString(),
                SslHelper.ToSslPlatformEnum(this.sslProtocol)).AsTask().Wait();
        }
    }
}

#endif
