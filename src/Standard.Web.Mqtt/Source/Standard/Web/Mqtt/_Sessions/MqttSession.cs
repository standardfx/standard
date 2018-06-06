using System.Collections;

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// MQTT session base class
    /// </summary>
    public abstract class MqttSession
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MqttSession()
            : this(null) 
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientId">Client Id to create session</param>
        public MqttSession(string clientId)
        {
            this.ClientId = clientId;
            this.InflightMessages = new Hashtable();
        }

        /// <summary>
        /// Client ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Messages inflight during session
        /// </summary>
        public Hashtable InflightMessages { get; set; }

        /// <summary>
        /// Clean session
        /// </summary>
        public virtual void Clear()
        {
            this.ClientId = null;
            this.InflightMessages.Clear();
        }
    }
}
