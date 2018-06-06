using System;

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// MQTT client exception
    /// </summary>
    public class MqttClientException : Exception
    {
        // error code
        private MqttClientErrorCode errorCode;

        /// <summary>
        /// Initializes a new instance of the <see cred="MqttClientException" /> class.
        /// </summary>
        /// <param name="errorCode">Error code</param>
        public MqttClientException(MqttClientErrorCode errorCode)
        {
            this.errorCode = errorCode;
        }

        /// <summary>
        /// MQTT error code.
        /// </summary>
        public MqttClientErrorCode ErrorCode
        {
            get { return this.errorCode; }
            set { this.errorCode = value; }
        }
    }

    /// <summary>
    /// MQTT client erroro code
    /// </summary>
    public enum MqttClientErrorCode
    {
        /// <summary>
        /// Will error (topic, message or QoS level)
        /// </summary>
        WillWrong = 1,

        /// <summary>
        /// Keep alive period too large
        /// </summary>
        KeepAliveWrong,

        /// <summary>
        /// Topic contains wildcards
        /// </summary>
        TopicWildcard,

        /// <summary>
        /// Topic length wrong
        /// </summary>
        TopicLength,

        /// <summary>
        /// QoS level not allowed
        /// </summary>
        QosNotAllowed,

        /// <summary>
        /// Topics list empty for subscribe
        /// </summary>
        TopicsEmpty,

        /// <summary>
        /// Qos levels list empty for subscribe
        /// </summary>
        QosLevelsEmpty,

        /// <summary>
        /// Topics / Qos Levels not match in subscribe
        /// </summary>
        TopicsQosLevelsNotMatch,

        /// <summary>
        /// Wrong message from broker
        /// </summary>
        WrongBrokerMessage,

        /// <summary>
        /// Wrong Message Id
        /// </summary>
        WrongMessageId,

        /// <summary>
        /// Inflight queue is full
        /// </summary>
        InflightQueueFull,

        /// <summary>
        /// Invalid flag bits received (v3.1.1)
        /// </summary>
        InvalidFlagBits,

        /// <summary>
        /// Invalid connect flags received (v3.1.1)
        /// </summary>
        InvalidConnectFlags,

        /// <summary>
        /// Invalid client id (v3.1.1)
        /// </summary>
        InvalidClientId,

        /// <summary>
        /// Invalid protocol name (v3.1.1)
        /// </summary>
        InvalidProtocolName
    }
}
