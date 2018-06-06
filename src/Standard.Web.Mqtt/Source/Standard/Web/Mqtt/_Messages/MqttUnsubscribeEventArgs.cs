#if NETMF4
using Microsoft.SPOT;
#else
using System;
#endif

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// Event arguments for unsubscribe request on topics.
    /// </summary>
    public class MqttUnsubscribeEventArgs : EventArgs
    {
        // message identifier
        private ushort messageId;
        
        // topics requested to unsubscribe
        private string[] topics;

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

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageId">Message identifier for subscribed topics</param>
        /// <param name="topics">Topics requested to subscribe</param>
        public MqttUnsubscribeEventArgs(ushort messageId, string[] topics)
        {
            this.messageId = messageId;
            this.topics = topics;
        }
    }
}
