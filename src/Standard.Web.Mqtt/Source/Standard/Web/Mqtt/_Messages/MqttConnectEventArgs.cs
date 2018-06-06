#if NETMF4
using Microsoft.SPOT;
#else
using System;
#endif

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// Event arguments for CONNECT message received from client.
    /// </summary>
    public class MqttConnectEventArgs : EventArgs
    {
        /// <summary>
        /// Message received from client.
        /// </summary>
        public MqttConnectMessage Message { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttConnectEventArgs"/> class.
        /// </summary>
        /// <param name="msg">CONNECT message received from client</param>
        public MqttConnectEventArgs(MqttConnectMessage msg)
        {
            this.Message = msg;
        }
    }
} 
