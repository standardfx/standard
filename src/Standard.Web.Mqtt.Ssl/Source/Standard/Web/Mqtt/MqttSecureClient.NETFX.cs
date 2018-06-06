#if NETFX || NETSTANDARD
using System;
using System.Net;
using System.Threading;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Net.Security;

namespace Standard.Web.Mqtt
{
    partial class MqttSecureClient
    {
        #region Constructors
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="brokerHostName">Broker Host Name or IP Address</param>
        /// <param name="brokerPort">Broker port</param>
        /// <param name="caCert">CA certificate for secure connection</param>
        /// <param name="clientCert">Client certificate</param>
        /// <param name="sslProtocol">SSL/TLS protocol version</param>
        /// <param name="userCertificateValidationCallback">A RemoteCertificateValidationCallback delegate responsible for validating the certificate supplied by the remote party</param>
        public MqttSecureClient(string brokerHostName, int brokerPort, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocol sslProtocol,
            RemoteCertificateValidationCallback userCertificateValidationCallback)
            : this(brokerHostName, brokerPort, caCert, clientCert, sslProtocol, userCertificateValidationCallback, null)
		{ }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="brokerHostName">Broker Host Name or IP Address</param>
        /// <param name="brokerPort">Broker port</param>
        /// <param name="sslProtocol">SSL/TLS protocol version</param>
        /// <param name="userCertificateValidationCallback">A RemoteCertificateValidationCallback delegate responsible for validating the certificate supplied by the remote party</param>
        /// <param name="userCertificateSelectionCallback">A LocalCertificateSelectionCallback delegate responsible for selecting the certificate used for authentication</param>
        public MqttSecureClient(string brokerHostName, int brokerPort, MqttSslProtocol sslProtocol, 
            RemoteCertificateValidationCallback userCertificateValidationCallback, 
            LocalCertificateSelectionCallback userCertificateSelectionCallback)
            : this(brokerHostName, brokerPort, null, null, sslProtocol, userCertificateValidationCallback, userCertificateSelectionCallback)
		{ }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="brokerHostName">Broker Host Name or IP Address</param>
        /// <param name="brokerPort">Broker port</param>
        /// <param name="caCert">CA certificate for secure connection</param>
        /// <param name="clientCert">Client certificate</param>
        /// <param name="sslProtocol">SSL/TLS protocol version</param>
        /// <param name="userCertificateValidationCallback">A RemoteCertificateValidationCallback delegate responsible for validating the certificate supplied by the remote party</param>
        /// <param name="userCertificateSelectionCallback">A LocalCertificateSelectionCallback delegate responsible for selecting the certificate used for authentication</param>
        public MqttSecureClient(string brokerHostName, int brokerPort, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocol sslProtocol,
            RemoteCertificateValidationCallback userCertificateValidationCallback,
            LocalCertificateSelectionCallback userCertificateSelectionCallback)
            : base(brokerHostName, brokerPort, true)
        {
			this.settings.Port = MqttSettings.MQTT_BROKER_DEFAULT_PORT;
			this.settings.SslPort = this.brokerPort;
			this.InitSecureChannel(caCert, clientCert, sslProtocol, userCertificateValidationCallback, userCertificateSelectionCallback);
        }

        #endregion

        /// <summary>
        /// MqttClient initialization
        /// </summary>
        /// <param name="caCert">CA certificate for secure connection</param>
        /// <param name="clientCert">Client certificate</param>
        /// <param name="sslProtocol">SSL/TLS protocol version</param>
        /// <param name="userCertificateSelectionCallback">A RemoteCertificateValidationCallback delegate responsible for validating the certificate supplied by the remote party</param>
        /// <param name="userCertificateValidationCallback">A LocalCertificateSelectionCallback delegate responsible for selecting the certificate used for authentication</param>
        private void InitSecureChannel(X509Certificate caCert, X509Certificate clientCert, MqttSslProtocol sslProtocol,
            RemoteCertificateValidationCallback userCertificateValidationCallback,
            LocalCertificateSelectionCallback userCertificateSelectionCallback)
        {
            // create network channel
            this.channel = new MqttSecureNetworkChannel(this.brokerHostName, this.brokerPort, caCert, clientCert, sslProtocol, userCertificateValidationCallback, userCertificateSelectionCallback);
        }
    }
}
#endif
