using System;

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// Connection to the broker exception.
    /// </summary>
    public class MqttConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cred="MqttConnectionException" /> class.
        /// </summary>
        public MqttConnectionException()
            : base(RS.UnableToConnect)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cred="MqttConnectionException" /> class.
        /// </summary>
		public MqttConnectionException(string message)
			: base(message)
		{ }

        /// <summary>
        /// Initializes a new instance of the <see cred="MqttConnectionException" /> class.
        /// </summary>
        public MqttConnectionException(string message, Exception innerException)
            : base(message, innerException) 
        { }
    }
}
