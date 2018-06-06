// NB: Doesn't share code with WinRT
#if !(WINRT)

using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates; 

// ssl support
#if NETMF4
// micro framework
using Microsoft.SPOT.Net.Security;
#else
// others (incl. CF)
using System.Net.Security; 
using System.Security.Authentication; 
#endif

namespace Standard.Web.Mqtt
{
    public partial class MqttSecureNetworkChannel : MqttNetworkChannel
    {
		// CA certificate (on client)
		private X509Certificate caCert;
        // Server certificate (on broker)
        private X509Certificate serverCert;
        // client certificate (on client)
        private X509Certificate clientCert;
        // SSL/TLS protocol version
        private MqttSslProtocol sslProtocol;
        
        /// <summary>
        /// Constructor helper
        /// </summary>
        protected void InitSecurity(X509Certificate caCert, X509Certificate clientCert, MqttSslProtocol sslProtocol)
        {
            this.caCert = caCert;
            this.clientCert = clientCert;
            this.sslProtocol = sslProtocol;            
        }
        
        /// <summary>
        /// Send data on the network channel
        /// </summary>
        /// <param name="buffer">Data buffer to send</param>
        /// <returns>Number of byte sent</returns>
        public override int Send(byte[] buffer)
        {
            this.sslStream.Write(buffer, 0, buffer.Length);
            this.sslStream.Flush();
            return buffer.Length;
        }

        /// <summary>
        /// Receive data from the network
        /// </summary>
        /// <param name="buffer">Data buffer for receiving data</param>
        /// <returns>Number of bytes received</returns>
        public override int Receive(byte[] buffer)
        {
			// read all data needed (until fill buffer)
			int idx = 0, read = 0;
			while (idx < buffer.Length)
			{
				// fixed scenario with socket closed gracefully by peer/broker and
				// Read return 0. Avoid infinite loop.
				read = this.sslStream.Read(buffer, idx, buffer.Length - idx);
				if (read == 0)
					return 0;
				idx += read;
			}
			return buffer.Length;
		}
	}
}

#endif
