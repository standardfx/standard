// NB: Doesn't share code with WinRT
#if !(WINRT)

using System;
using System.Net;
using System.Net.Sockets;

namespace Standard.Web.Mqtt
{
    public partial class MqttNetworkChannel : IMqttNetworkChannel
    {
		// remote host information
		protected string remoteHostName;
		protected IPAddress remoteIpAddress;
		protected int remotePort;

        // socket for communication
        protected Socket socket;
        
        /// <summary>
        /// Remote host name
        /// </summary>
        public string RemoteHostName 
        { 
            get { return this.remoteHostName; } 
        }

        /// <summary>
        /// Remote IP address
        /// </summary>
        public IPAddress RemoteIpAddress 
        { 
            get { return this.remoteIpAddress; } 
        }

        /// <summary>
        /// Remote port
        /// </summary>
        public int RemotePort 
        { 
            get { return this.remotePort; } 
        }

        /// <summary>
        /// Constructor helper
        /// </summary>
        protected virtual void Init(string remoteHostName, int remotePort)
        {
            IPAddress remoteIpAddress = null;
            try
            {
                // check if remoteHostName is a valid IP address and get it
                remoteIpAddress = IPAddress.Parse(remoteHostName);
            }
            catch
            {
            }

            // in this case the parameter remoteHostName isn't a valid IP address
            if (remoteIpAddress == null)
            {
                IPHostEntry hostEntry = DnsUtility.GetHostEntry(remoteHostName);
                if ((hostEntry != null) && (hostEntry.AddressList.Length > 0))
                {
                    // check for the first address not null
                    // it seems that with .Net Micro Framework, the IPV6 addresses aren't supported and return "null"
                    int i = 0;
                    while (hostEntry.AddressList[i] == null) i++;
                    remoteIpAddress = hostEntry.AddressList[i];
                }
                else
                {
                    throw new MqttConnectionException(RS.RemoteHostNotFound);
                }
            }

            this.remoteHostName = remoteHostName;
            this.remoteIpAddress = remoteIpAddress;
            this.remotePort = remotePort;
        }
        
        /// <summary>
        /// Send data on the network channel
        /// </summary>
        /// <param name="buffer">Data buffer to send</param>
        /// <returns>Number of byte sent</returns>
        public virtual int Send(byte[] buffer)
        {
            return this.socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        /// <summary>
        /// Receive data from the network channel with a specified timeout
        /// </summary>
        /// <param name="buffer">Data buffer for receiving data</param>
        /// <param name="timeout">Timeout on receiving (in milliseconds)</param>
        /// <returns>Number of bytes received</returns>
        public virtual int Receive(byte[] buffer, int timeout)
        {
            // check data availability (timeout is in microseconds)
            if (this.socket.Poll(timeout * 1000, SelectMode.SelectRead))
                return this.Receive(buffer);
            else
                return 0;
        }
        
        /// <summary>
        /// Receive data from the network
        /// </summary>
        /// <param name="buffer">Data buffer for receiving data</param>
        /// <returns>Number of bytes received</returns>
        public virtual int Receive(byte[] buffer)
        {
            // read all data needed (until fill buffer)
            int idx = 0, read = 0;
            while (idx < buffer.Length)
            {
                // fixed scenario with socket closed gracefully by peer/broker and
                // Read return 0. Avoid infinite loop.
                read = this.socket.Receive(buffer, idx, buffer.Length - idx, SocketFlags.None);
                if (read == 0)
                    return 0;
                idx += read;
            }
            return buffer.Length;
        }        
    }
}

#endif
