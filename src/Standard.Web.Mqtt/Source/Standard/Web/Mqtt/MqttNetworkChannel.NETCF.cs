#if NETCF

using System;
using System.Net;
using System.Net.Sockets;

namespace Standard.Web.Mqtt
{
    partial class MqttNetworkChannel : IMqttNetworkChannel
    {      
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket">Socket opened with the client</param>
        public MqttNetworkChannel(Socket socket)
        {
            this.socket = socket;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="remoteHostName">Remote Host name</param>
        public MqttNetworkChannel(string remoteHostName)
            : this(remoteHostName, MqttSettings.MQTT_BROKER_DEFAULT_PORT)
		{ }
            
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="remoteHostName">Remote Host name</param>
        /// <param name="remotePort">Remote port</param>
        public MqttNetworkChannel(string remoteHostName, int remotePort)
        {
            Init(remoteHostName, remotePort);
        }

        /// <summary>
        /// Data available on the channel
        /// </summary>
        public virtual bool DataAvailable
        {
            get
            {
                return (this.socket.Available > 0);
            }
        }
        
        /// <summary>
        /// Connect to remote server
        /// </summary>
        public virtual void Connect()
        {
            this.socket = new Socket(this.remoteIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // try connection to the broker
            this.socket.Connect(new IPEndPoint(this.remoteIpAddress, this.remotePort));
        }

        /// <summary>
        /// Close the network channel
        /// </summary>
        public virtual void Close()
        {
            this.socket.Close();
        }

        /// <summary>
        /// Accept connection from a remote client
        /// </summary>
        public virtual void Accept()
        {
			return;
        }
    }
}

#endif
