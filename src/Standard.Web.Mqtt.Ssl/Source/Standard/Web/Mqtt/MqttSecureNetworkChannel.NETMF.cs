#if NETMF

using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates; 
using Microsoft.SPOT.Net.Security;

namespace Standard.Web.Mqtt
{
    partial class MqttSecureNetworkChannel
    {
        private SslStream sslStream;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket">Socket opened with the client</param>
        /// <param name="serverCert">Server X509 certificate for secure connection</param>
        /// <param name="sslProtocol">SSL/TLS protocol version</param>
        public MqttSecureNetworkChannel(Socket socket, X509Certificate serverCert, MqttSslProtocols sslProtocol)
			:base(socket)
        {
            this.serverCert = serverCert;
            this.sslProtocol = sslProtocol;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="remoteHostName">Remote Host name</param>
        /// <param name="caCert">CA certificate</param>
        /// <param name="clientCert">Client certificate</param>
        /// <param name="sslProtocol">SSL/TLS protocol version</param>
		public MqttSecureNetworkChannel(string remoteHostName, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol)
            : this(remoteHostName, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, caCert, clientCert, sslProtocol)
		{ }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="remoteHostName">Remote Host name</param>
        /// <param name="remotePort">Remote port</param>
        /// <param name="caCert">CA certificate</param>
        /// <param name="clientCert">Client certificate</param>
        /// <param name="sslProtocol">SSL/TLS protocol version</param>
        public MqttSecureNetworkChannel(string remoteHostName, int remotePort, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol)
			:base(remoteHostName, remotePort)
        {
            InitSecurity(caCert, clientCert, sslProtocol);
        }

        /// <summary>
        /// Data available on the channel
        /// </summary>
        public override bool DataAvailable
        {
            get
            {
                return this.sslStream.DataAvailable;
            }
        }
        
        /// <summary>
        /// Connect to remote server
        /// </summary>
        public override void Connect()
        {
			base.Connect();

			// create SSL stream
            this.sslStream = new SslStream(this.socket);

            // server authentication (SSL/TLS handshake)
            this.sslStream.AuthenticateAsClient(this.remoteHostName,
                this.clientCert,
                new X509Certificate[] { this.caCert },
                SslVerification.CertificateRequired,
                SslHelper.ToSslPlatformEnum(this.sslProtocol));
        }

        /// <summary>
        /// Close the network channel
        /// </summary>
        public override void Close()
        {
            this.sslStream.Close();
			base.Close();
        }
    }
}

#endif
