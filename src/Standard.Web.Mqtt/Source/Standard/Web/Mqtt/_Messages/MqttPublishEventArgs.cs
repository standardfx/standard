#if NETMF4
using Microsoft.SPOT;
#else
using System;
#endif

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// Event arguments for a <c>PUBLISH</c> message received from broker.
    /// </summary>
    public class MqttPublishEventArgs : EventArgs
    {
        #region Properties...

        /// <summary>
        /// Message topic
        /// </summary>
        public string Topic
        {
            get { return this.topic; }
            internal set { this.topic = value; }
        }

        /// <summary>
        /// Message data
        /// </summary>
        public byte[] Message
        {
            get { return this.message; }
            internal set { this.message = value; }
        }

        /// <summary>
        /// Duplicate message flag
        /// </summary>
        public bool DupFlag
        {
            get { return this.dupFlag; }
            set { this.dupFlag = value; }
        }

        /// <summary>
        /// Quality of Service level
        /// </summary>
        public byte QosLevel
        {
            get { return this.qosLevel; }
            internal set { this.qosLevel = value; }
        }

        /// <summary>
        /// Retain message flag
        /// </summary>
        public bool Retain
        {
            get { return this.retain; }
            internal set { this.retain = value; }
        }

        #endregion

        // message topic
        private string topic;
        // message data
        private byte[] message;
        // duplicate delivery
        private bool dupFlag;
        // quality of service level
        private byte qosLevel;
        // retain flag
        private bool retain;       

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message data</param>
        /// <param name="dupFlag">Duplicate delivery flag</param>
        /// <param name="qosLevel">Quality of Service level</param>
        /// <param name="retain">Retain flag</param>
        public MqttPublishEventArgs(string topic, byte[] message, bool dupFlag, byte qosLevel, bool retain)
        {
            this.topic = topic;
            this.message = message;
            this.dupFlag = dupFlag;
            this.qosLevel = qosLevel;
            this.retain = retain;
        }
    }
}
