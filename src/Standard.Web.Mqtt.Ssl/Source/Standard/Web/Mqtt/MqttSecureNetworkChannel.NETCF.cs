#if NETCF

using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates; 
using System.Net.Security; 
using System.Security.Authentication; 

namespace Standard.Web.Mqtt
{
    partial class MqttSecureNetworkChannel
    {
        private SslStream sslStream;
        private NetworkStream netStream;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket">Socket opened with the client</param>
        /// <param name="serverCert">Server X509 certificate for secure connection</param>
        /// <param name="sslProtocol">SSL/TLS protocol version</param>
        public MqttSecureNetworkChannel(Socket socket, X509Certificate serverCert, MqttSslProtocol sslProtocol)
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
		public MqttSecureNetworkChannel(string remoteHostName, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocol sslProtocol)
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
		public MqttSecureNetworkChannel(string remoteHostName, int remotePort, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocol sslProtocol)
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
                return this.netStream.DataAvailable;
            }
        }
        
        /// <summary>
        /// Connect to remote server
        /// </summary>
        public override void Connect()
        {
			base.Connect();

            // create SSL stream
            this.netStream = new NetworkStream(this.socket);
            this.sslStream = new SslStream(this.netStream, false, this.userCertificateValidationCallback, this.userCertificateSelectionCallback);

            // server authentication (SSL/TLS handshake)
            X509CertificateCollection clientCertificates = null; 
            // check if there is a client certificate to add to the collection, otherwise it's null (as empty) 
            if (this.clientCert != null) 
                clientCertificates = new X509CertificateCollection(new X509Certificate[] { this.clientCert }); 

            this.sslStream.AuthenticateAsClient(this.remoteHostName, clientCertificates, SslHelper.ToSslPlatformEnum(this.sslProtocol), false); 
        }

        /// <summary>
        /// Close the network channel
        /// </summary>
        public override void Close()
        {
            this.netStream.Close();
            this.sslStream.Close();
        }

        /// <summary>
        /// Accept connection from a remote client
        /// </summary>
        public override void Accept()
        {
            this.netStream = new NetworkStream(this.socket);
            this.sslStream = new SslStream(this.netStream, false, this.userCertificateValidationCallback, this.userCertificateSelectionCallback);
            this.sslStream.AuthenticateAsServer(this.serverCert, false, SslHelper.ToSslPlatformEnum(this.sslProtocol), false);
        }
    }
}

#endif
