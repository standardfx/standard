#if NETMF4
using Microsoft.SPOT;
#else
using System;
#endif

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// Event arguments class for published message.
    /// </summary>
    public class MqttPublishCompleteEventArgs : EventArgs
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
        /// Message published (or failed due to retries)
        /// </summary>
        public bool IsPublished
        {
            get { return this.isPublished; }
            internal set { this.isPublished = value; }
        }

        #endregion

        // message identifier
        ushort messageId;

        // published flag
        bool isPublished;

        /// <summary>
        /// Constructor (published message)
        /// </summary>
        /// <param name="messageId">Message identifier published</param>
        public MqttPublishCompleteEventArgs(ushort messageId) 
            : this(messageId, true)
		{ }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageId">Message identifier</param>
        /// <param name="isPublished">Publish flag</param>
        public MqttPublishCompleteEventArgs(ushort messageId, bool isPublished)
        {
            this.messageId = messageId;
            this.isPublished = isPublished;
        }
    }
}
