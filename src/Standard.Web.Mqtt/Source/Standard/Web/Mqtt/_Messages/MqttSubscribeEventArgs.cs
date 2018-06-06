#if NETMF4
using Microsoft.SPOT;
#else
using System;
#endif

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// EventArgs class for subscribe request on topics
    /// </summary>
    public class MqttSubscribeEventArgs : EventArgs
    {
        #region Properties...

        /// <summary>
        /// Message identifier
        /// </summary>
        public ushort MessageId
        {
            get { return this.messageId; }
            internal set { this.messageId = value; }
        }

        /// <summary>
        /// Topics requested to subscribe
        /// </summary>
        public string[] Topics
        {
            get { return this.topics; }
            internal set { this.topics = value; }
        }

        /// <summary>
        /// List of QOS Levels requested
        /// </summary>
        public byte[] QoSLevels
        {
            get { return this.qosLevels; }
            internal set { this.qosLevels = value; }
        }

        #endregion

        // message identifier
        ushort messageId;
        // topics requested to subscribe
        string[] topics;
        // QoS levels requested
        byte[] qosLevels;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageId">Message identifier for subscribe topics request</param>
        /// <param name="topics">Topics requested to subscribe</param>
        /// <param name="qosLevels">List of QOS Levels requested</param>
        public MqttSubscribeEventArgs(ushort messageId, string[] topics, byte[] qosLevels)
        {
            this.messageId = messageId;
            this.topics = topics;
            this.qosLevels = qosLevels;
        }
    }
}
