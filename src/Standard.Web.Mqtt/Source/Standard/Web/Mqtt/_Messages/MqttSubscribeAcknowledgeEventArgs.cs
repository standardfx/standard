#if NETMF4
using Microsoft.SPOT;
#else
using System;
#endif

namespace Standard.Web.Mqtt
{
	/// <summary>
	/// Event arguments for subscribed topics.
	/// </summary>
	public class MqttSubscribeAcknowledgeEventArgs : EventArgs
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
        /// List of granted QOS Levels
        /// </summary>
        public byte[] GrantedQoSLevels
        {
            get { return this.grantedQosLevels; }
            internal set { this.grantedQosLevels = value; }
        }

        #endregion

        // message identifier
        ushort messageId;
        // granted QOS levels
        byte[] grantedQosLevels;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageId">Message identifier for subscribed topics</param>
        /// <param name="grantedQosLevels">List of granted QOS Levels</param>
        public MqttSubscribeAcknowledgeEventArgs(ushort messageId, byte[] grantedQosLevels)
        {
            this.messageId = messageId;
            this.grantedQosLevels = grantedQosLevels;
        }
    }
}
