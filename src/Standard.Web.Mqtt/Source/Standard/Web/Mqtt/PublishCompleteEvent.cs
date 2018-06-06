namespace Standard.Web.Mqtt
{
    /// <summary>
    /// Internal event for a published message
    /// </summary>
    public class PublishCompleteEvent : InternalMessageEvent
    {
        #region Properties...

        /// <summary>
        /// Message published (or failed due to retries)
        /// </summary>
        public bool IsPublished
        {
            get { return this.isPublished; }
            internal set { this.isPublished = value; }
        }

        #endregion

        // published flag
        bool isPublished;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msg">Message published</param>
        /// <param name="isPublished">Publish flag</param>
        public PublishCompleteEvent(MqttMessage msg, bool isPublished) 
            : base(msg)
        {
            this.isPublished = isPublished;
        }
    }
}
