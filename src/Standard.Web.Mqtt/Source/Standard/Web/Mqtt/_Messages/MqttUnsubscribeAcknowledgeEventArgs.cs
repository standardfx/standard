#if NETMF4
using Microsoft.SPOT;
#else
using System;
#endif

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// Event arguments for unsubscribed topic.
    /// </summary>
    public class MqttUnsubscribeAcknowledgeEventArgs : EventArgs
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

        #endregion

        // message identifier
        private ushort messageId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageId">Message identifier for unsubscribed topic</param>
        public MqttUnsubscribeAcknowledgeEventArgs(ushort messageId)
        {
            this.messageId = messageId;
        }
    }
}
