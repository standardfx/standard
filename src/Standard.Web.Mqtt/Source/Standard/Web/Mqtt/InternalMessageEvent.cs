namespace Standard.Web.Mqtt
{
    /// <summary>
    /// Internal event with a message
    /// </summary>
    public class InternalMessageEvent : InternalEvent
    {
        /// <summary>
        /// Related message
        /// </summary>
        public MqttMessage Message
        {
            get { return this.msg; }
            set { this.msg = value; }
        }

        // related message
        protected MqttMessage msg;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msg">Related message</param>
        public InternalMessageEvent(MqttMessage msg)
        {
            this.msg = msg;
        }
    }
}
