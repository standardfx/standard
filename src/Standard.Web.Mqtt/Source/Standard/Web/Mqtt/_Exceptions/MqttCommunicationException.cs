using System;

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// Exception due to error communication with broker on socket
    /// </summary>
    public class MqttCommunicationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cred="MqttCommunicationException" /> class.
        /// </summary>
        public MqttCommunicationException() 
        { }

        /// <summary>
        /// Initializes a new instance of the <see cred="MqttCommunicationException" /> class with inner exception.
        /// </summary>
        /// <param name="e">Inner Exception</param>
        public MqttCommunicationException(Exception e)
            : base(string.Empty, e) 
        { }
    }
}
