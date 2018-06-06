using System;
using System.Net;
using System.Threading;
using System.Collections;
using System.IO;
using Standard;
using System.Text;

// platform specific code
#if WINRT
using Windows.Networking.Sockets;
#elif NETMF
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.SPOT;
#elif NETFX || NETSTANDARD
// Any other framework and no SSL
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
#endif

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// MQTT client.
    /// </summary>
    public partial class MqttClient
    {
        #region Public events
        
        /// <summary>
        /// Delegate that defines event handler for PUBLISH message received
        /// </summary>
        public delegate void PublishEventHandler(object sender, MqttPublishEventArgs e);

        /// <summary>
        /// Delegate that defines event handler for published message
        /// </summary>
        public delegate void PublishCompleteEventHandler(object sender, MqttPublishCompleteEventArgs e);

        /// <summary>
        /// Delagate that defines event handler for subscribed topic
        /// </summary>
        public delegate void SubscribeAcknowledgeEventHandler(object sender, MqttSubscribeAcknowledgeEventArgs e);

        /// <summary>
        /// Delagate that defines event handler for unsubscribed topic
        /// </summary>
        public delegate void UnsubscribeAcknowledgeEventHandler(object sender, MqttUnsubscribeAcknowledgeEventArgs e);

        /// <summary>
        /// Delegate that defines event handler for cliet/peer disconnection.
        /// </summary>
        public delegate void ConnectionClosedEventHandler(object sender, EventArgs e);

        // event for PUBLISH message received
        public event PublishEventHandler PublishReceived;

        // event for published message
        public event PublishCompleteEventHandler Published;

        // event for subscribed topic
        public event SubscribeAcknowledgeEventHandler Subscribed;
  
		// event for unsubscribed topic
        public event UnsubscribeAcknowledgeEventHandler Unsubscribed;
        
		// event for peer/client disconnection
        public event ConnectionClosedEventHandler ConnectionClosed;

		#endregion // Public events

		#region Private fields

		// broker hostname (or ip address) and port
		protected string brokerHostName;
		protected int brokerPort;

		// running status of threads
		protected bool isRunning;
		// event for raising received message event
		protected AutoResetEvent receiveEventWaitHandle;

		// event for starting process inflight queue asynchronously
		protected AutoResetEvent inflightWaitHandle;

		// event for signaling synchronous receive
		protected AutoResetEvent syncEndReceiving;
		// message received
		protected MqttMessage msgReceived;

		// exeption thrown during receiving
		protected Exception exReceiving;

		// keep alive period (in ms)
		protected int keepAlivePeriod;
		// events for signaling on keep alive thread
		protected AutoResetEvent keepAliveEvent;
		protected AutoResetEvent keepAliveEventEnd;
		// last communication time in ticks
		protected int lastCommTime;

		// channel to communicate over the network
		protected IMqttNetworkChannel channel;

		// inflight messages queue
		protected Queue inflightQueue;
		// internal queue for received messages about inflight messages
		protected Queue internalQueue;
		// internal queue for dispatching events
		protected Queue eventQueue;
		// session
		protected MqttClientSession session;

		// reference to avoid access to singleton via property
		protected MqttSettings settings;

		// current message identifier generated
		protected ushort messageIdCounter = 0;

		// connection is closing due to peer
		protected bool isConnectionClosing;

		#endregion // Private fields

		#region Public properties

		/// <summary>
		/// Connection status between client and broker
		/// </summary>
		public bool IsConnected { get; protected set; }

        /// <summary>
        /// Client identifier
        /// </summary>
        public string ClientId { get; protected set; }

        /// <summary>
        /// Clean session flag
        /// </summary>
        public bool CleanSession { get; protected set; }

        /// <summary>
        /// Will flag
        /// </summary>
        public bool WillFlag { get; protected set; }

        /// <summary>
        /// Will QOS level
        /// </summary>
        public byte WillQosLevel { get; protected set; }

        /// <summary>
        /// Will topic
        /// </summary>
        public string WillTopic { get; protected set; }

        /// <summary>
        /// Will message
        /// </summary>
        public string WillMessage { get; protected set; }

        /// <summary>
        /// MQTT protocol version
        /// </summary>
        public MqttProtocolVersion ProtocolVersion { get; set; }

        /// <summary>
        /// MQTT client settings
        /// </summary>
        public MqttSettings Settings
        {
            get { return this.settings; }
        }

		#endregion // Public properties

		#region Init

		/// <summary>
		/// MqttClient initialization
		/// </summary>
		/// <param name="brokerHostName">Broker Host Name or IP Address</param>
		/// <param name="brokerPort">Broker port</param>
		protected void Init(string brokerHostName, int brokerPort)
        {
            // set default MQTT protocol version (default is 3.1.1)
            this.ProtocolVersion = MqttProtocolVersion.Version_3_1_1;

            this.brokerHostName = brokerHostName;
            this.brokerPort = brokerPort;

            // reference to MQTT settings
            this.settings = MqttSettings.Factory;
            // set settings port based on secure connection or not
            this.settings.Port = this.brokerPort;

            this.syncEndReceiving = new AutoResetEvent(false);
            this.keepAliveEvent = new AutoResetEvent(false);

            // queue for handling inflight messages (publishing and acknowledge)
            this.inflightWaitHandle = new AutoResetEvent(false);
            this.inflightQueue = new Queue();

            // queue for received message
            this.receiveEventWaitHandle = new AutoResetEvent(false);
            this.eventQueue = new Queue();
            this.internalQueue = new Queue();

            // session
            this.session = null;
        }

        #endregion // Init
        
        #region Public fields - Method

        /// <summary>
        /// Connect to broker
        /// </summary>
        /// <returns>Return code of CONNACK message from broker</returns>
        public virtual byte Connect()
        {
            return this.Connect(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Connect to broker
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <returns>Return code of CONNACK message from broker</returns>
        public virtual byte Connect(string clientId)
        {
            return this.Connect(clientId, null, null, false, MqttConnectMessage.QOS_LEVEL_AT_MOST_ONCE, false, null, null, true, MqttConnectMessage.KEEP_ALIVE_PERIOD_DEFAULT);
        }

        /// <summary>
        /// Connect to broker
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Return code of CONNACK message from broker</returns>
        public virtual byte Connect(string clientId, string username, string password)
        {
            return this.Connect(clientId, username, password, false, MqttConnectMessage.QOS_LEVEL_AT_MOST_ONCE, false, null, null, true, MqttConnectMessage.KEEP_ALIVE_PERIOD_DEFAULT);
        }

        /// <summary>
        /// Connect to broker
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="cleanSession">Clean sessione flag</param>
        /// <param name="keepAlivePeriod">Keep alive period</param>
        /// <returns>Return code of CONNACK message from broker</returns>
        public virtual byte Connect(string clientId, string username, string password, bool cleanSession, ushort keepAlivePeriod)
        {
            return this.Connect(clientId, username, password, false, MqttConnectMessage.QOS_LEVEL_AT_MOST_ONCE, false, null, null, cleanSession, keepAlivePeriod);
        }

                /// <summary>
        /// Connect to broker
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="willRetain">Will retain flag</param>
        /// <param name="willQosLevel">Will QOS level</param>
        /// <param name="willFlag">Will flag</param>
        /// <param name="willTopic">Will topic</param>
        /// <param name="willMessage">Will message</param>
        /// <param name="cleanSession">Clean sessione flag</param>
        /// <param name="keepAlivePeriod">Keep alive period</param>
        /// <returns>Return code of CONNACK message from broker</returns>
        public virtual byte Connect(string clientId, string username, string password, bool willRetain, QosLevel willQosLevel, bool willFlag, string willTopic, string willMessage, bool cleanSession, ushort keepAlivePeriod)
        {
            return this.Connect(clientId, username, password, willRetain, (byte)willQosLevel, willFlag, willTopic, willMessage, cleanSession, keepAlivePeriod);
        }

        /// <summary>
        /// Connect to broker
        /// </summary>
        /// <param name="clientId">Client identifier</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="willRetain">Will retain flag</param>
        /// <param name="willQosLevel">Will QOS level</param>
        /// <param name="willFlag">Will flag</param>
        /// <param name="willTopic">Will topic</param>
        /// <param name="willMessage">Will message</param>
        /// <param name="cleanSession">Clean sessione flag</param>
        /// <param name="keepAlivePeriod">Keep alive period</param>
        /// <returns>Return code of CONNACK message from broker</returns>
        public virtual byte Connect(string clientId, string username, string password, bool willRetain, byte willQosLevel, bool willFlag, string willTopic, string willMessage, bool cleanSession, ushort keepAlivePeriod)
        {
			// create CONNECT message
			MqttConnectMessage connect = new MqttConnectMessage(clientId,
                username,
                password,
                willRetain,
                willQosLevel,
                willFlag,
                willTopic,
                willMessage,
                cleanSession,
                keepAlivePeriod,
                (byte)this.ProtocolVersion);

            try
            {
                // connect to the broker
                this.channel.Connect();
            }
            catch (Exception ex)
            {
                throw new MqttConnectionException("An error has occured while trying to connect to the broker.", ex);
            }

            this.lastCommTime = 0;
            this.isRunning = true;
            this.isConnectionClosing = false;

			// start thread for receiving messages from broker
			new Thread(this.ReceiveThread).Start();
            
            MqttConnectAcknowledgeMessage connack = (MqttConnectAcknowledgeMessage)this.SendReceive(connect);
            // if connection accepted, start keep alive timer and 
            if (connack.ReturnCode == MqttConnectAcknowledgeMessage.CONN_ACCEPTED)
            {
                // set all client properties
                this.ClientId = clientId;
                this.CleanSession = cleanSession;
                this.WillFlag = willFlag;
                this.WillTopic = willTopic;
                this.WillMessage = willMessage;
                this.WillQosLevel = willQosLevel;

                this.keepAlivePeriod = keepAlivePeriod * 1000; // convert in ms

                // restore previous session
                this.RestoreSession();

                // keep alive period equals zero means turning off keep alive mechanism
                if (this.keepAlivePeriod != 0)
                {
					// start thread for sending keep alive message to the broker
					new Thread(this.KeepAliveThread).Start();
                }

				// start thread for raising received message event from broker
				new Thread(this.DispatchEventThread).Start();

                // start thread for handling inflight messages queue to broker asynchronously (publish and acknowledge)
				new Thread(this.ProcessInflightThread).Start();

				this.IsConnected = true;
            }
            return connack.ReturnCode;
        }

        /// <summary>
        /// Disconnect from broker
        /// </summary>
        public virtual void Disconnect()
        {
            MqttDisconnectMessage disconnect = new MqttDisconnectMessage();
            this.Send(disconnect);

            // close client
            this.OnConnectionClosing();
        }

        /// <summary>
        /// Close client
        /// </summary>
        public virtual void Close()
        {
            // stop receiving thread
            this.isRunning = false;

            // wait end receive event thread
            if (this.receiveEventWaitHandle != null)
                this.receiveEventWaitHandle.Set();

            // wait end process inflight thread
            if (this.inflightWaitHandle != null)
                this.inflightWaitHandle.Set();

            // unlock keep alive thread and wait
            this.keepAliveEvent.Set();

            if (this.keepAliveEventEnd != null)
                this.keepAliveEventEnd.WaitOne();

            // clear all queues
            this.inflightQueue.Clear();
            this.internalQueue.Clear();
            this.eventQueue.Clear();

            // close network channel
            this.channel.Close();

            this.IsConnected = false;
        }

        /// <summary>
        /// Execute ping to broker for keep alive
        /// </summary>
        /// <returns>PINGRESP message from broker</returns>
        public virtual MqttPingResponseMessage Ping()
        {
            MqttPingRequestMessage pingreq = new MqttPingRequestMessage();
            try
            {
                // broker must send PINGRESP within timeout equal to keep alive period
                return (MqttPingResponseMessage)this.SendReceive(pingreq, this.keepAlivePeriod);
            }
            catch (Exception e)
            {
				MqttLogger.WriteLine(LogLevel.Error, "Exception on ping: {0}", e.ToString());

				// client must close connection
				this.OnConnectionClosing();
                return null;
            }
        }

        /// <summary>
        /// Subscribe for message topics
        /// </summary>
        /// <param name="topic">Topic to subscribe</param>
        /// <returns>Message Id related to SUBSCRIBE message</returns>
        public virtual ushort Subscribe(string topic)
        {
            return Subscribe(new string[] { topic }, new byte[] { MqttMessage.QOS_LEVEL_AT_MOST_ONCE });
        }

        /// <summary>
        /// Subscribe for message topics
        /// </summary>
        /// <param name="topics">List of topics to subscribe</param>
        /// <returns>Message Id related to SUBSCRIBE message</returns>
        public virtual ushort Subscribe(string[] topics)
        {
            return Subscribe(topics, MqttMessage.QOS_LEVEL_AT_MOST_ONCE);
        }

        /// <summary>
        /// Subscribe for message topics
        /// </summary>
        /// <param name="topic">Topic to subscribe</param>
        /// <param name="qosLevel">QOS level related to topic</param>
        /// <returns>Message Id related to SUBSCRIBE message</returns>
        public virtual ushort Subscribe(string topic, QosLevel qosLevel)
        {
            return Subscribe(new string[] { topic }, new byte[] { (byte)qosLevel });
        }

        /// <summary>
        /// Subscribe for message topics
        /// </summary>
        /// <param name="topics">List of topics to subscribe</param>
        /// <param name="qosLevel">QOS level related to all topics</param>
        /// <returns>Message Id related to SUBSCRIBE message</returns>
        public virtual ushort Subscribe(string[] topics, QosLevel qosLevel)
        {
            byte[] qosBytes = new byte[topics.Length];
            for (int i = 0; i < topics.Length; i++)
            {
                qosBytes[i] = (byte)qosLevel;
            }

            return Subscribe(topics, qosBytes);
        }

        /// <summary>
        /// Subscribe for message topics
        /// </summary>
        /// <param name="topics">List of topics to subscribe</param>
        /// <param name="qosLevels">QOS levels related to topics</param>
        /// <returns>Message Id related to SUBSCRIBE message</returns>
        public virtual ushort Subscribe(string[] topics, QosLevel[] qosLevels)
        {
            byte[] qosBytes = new byte[qosLevels.Length];
            for (int i = 0; i < qosLevels.Length; i++)
            {
                qosBytes[i] = (byte)qosLevels[i];
            }

            return Subscribe(topics, qosBytes);
        }

        /// <summary>
        /// Subscribe for message topics
        /// </summary>
        /// <param name="topics">List of topics to subscribe</param>
        /// <param name="qosLevels">QOS levels related to topics</param>
        /// <returns>Message Id related to SUBSCRIBE message</returns>
        public virtual ushort Subscribe(string[] topics, byte[] qosLevels)
        {
            MqttSubscribeMessage subscribe = new MqttSubscribeMessage(topics, qosLevels);
            subscribe.MessageId = this.GetMessageId();

            // enqueue subscribe request into the inflight queue
            this.EnqueueInflight(subscribe, MqttMessageFlow.ToPublish);

            return subscribe.MessageId;
        }

        /// <summary>
        /// Unsubscribe for message topics
        /// </summary>
        /// <param name="topic">Name of topic to unsubscribe</param>
        /// <returns>Message Id in UNSUBACK message from broker</returns>
        public virtual ushort Unsubscribe(string topic)
        {
            return Unsubscribe(new string[] { topic });
        }

        /// <summary>
        /// Unsubscribe for message topics
        /// </summary>
        /// <param name="topics">List of topics to unsubscribe</param>
        /// <returns>Message Id in UNSUBACK message from broker</returns>
        public virtual ushort Unsubscribe(string[] topics)
        {
            MqttUnsubscribeMessage unsubscribe = new MqttUnsubscribeMessage(topics);
            unsubscribe.MessageId = this.GetMessageId();

            // enqueue unsubscribe request into the inflight queue
            this.EnqueueInflight(unsubscribe, MqttMessageFlow.ToPublish);

            return unsubscribe.MessageId;
        }

        /// <summary>
        /// Publish a message asynchronously (QoS Level 0 and not retained)
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message data (payload)</param>
        /// <returns>Message Id related to PUBLISH message</returns>
        public virtual ushort Publish(string topic, byte[] message)
        {
            return this.Publish(topic, message, MqttMessage.QOS_LEVEL_AT_MOST_ONCE, false);
        }

        /// <summary>
        /// Publish a message asynchronously (QoS Level 0 and not retained)
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message data (payload) in UTF8 encoding.</param>
        /// <returns>Message Id related to PUBLISH message</returns>
        public virtual ushort Publish(string topic, string message)
        {
            return this.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMessage.QOS_LEVEL_AT_MOST_ONCE, false);
        }

        /// <summary>
        /// Publish a message asynchronously
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message data (payload) in UTF8 encoding.</param>
        /// <param name="qosLevel">QoS Level</param>
        /// <param name="retain">Retain flag</param>
        /// <returns>Message Id related to PUBLISH message</returns>
        public virtual ushort Publish(string topic, string message, QosLevel qosLevel, bool retain)
        {
            return Publish(topic, Encoding.UTF8.GetBytes(message), (byte)qosLevel, retain);
        }

        /// <summary>
        /// Publish a message asynchronously
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message data (payload)</param>
        /// <param name="qosLevel">QoS Level</param>
        /// <param name="retain">Retain flag</param>
        /// <returns>Message Id related to PUBLISH message</returns>
        public virtual ushort Publish(string topic, byte[] message, QosLevel qosLevel, bool retain)
        {
            return Publish(topic, message, (byte)qosLevel, retain);
        }

        /// <summary>
        /// Publish a message asynchronously
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message data (payload)</param>
        /// <param name="qosLevel">QoS Level</param>
        /// <param name="retain">Retain flag</param>
        /// <returns>Message Id related to PUBLISH message</returns>
        public virtual ushort Publish(string topic, byte[] message, byte qosLevel, bool retain)
        {
            MqttPublishMessage publish = new MqttPublishMessage(topic, message, false, qosLevel, retain);
            publish.MessageId = this.GetMessageId();

            // enqueue message to publish into the inflight queue
            bool enqueue = this.EnqueueInflight(publish, MqttMessageFlow.ToPublish);

            // message enqueued
            if (enqueue)
                return publish.MessageId;
            // infligh queue full, message not enqueued
            else
                throw new MqttClientException(MqttClientErrorCode.InflightQueueFull);
        }

		#endregion

		#region Protected events

		/// <summary>
		/// Wrapper method for raising events
		/// </summary>
		/// <param name="internalEvent">Internal event</param>
		protected virtual void OnInternalEvent(InternalEvent internalEvent)
        {
            lock (this.eventQueue)
            {
                this.eventQueue.Enqueue(internalEvent);
            }

            this.receiveEventWaitHandle.Set();
        }

		/// <summary>
		/// Wrapper method for raising closing connection event
		/// </summary>
		protected virtual void OnConnectionClosing()
        {
            if (!this.isConnectionClosing)
            {
                this.isConnectionClosing = true;
                this.receiveEventWaitHandle.Set();
            }
        }

		/// <summary>
		/// Wrapper method for raising PUBLISH message received event
		/// </summary>
		/// <param name="publish">PUBLISH message received</param>
		protected virtual void OnPublishReceived(MqttPublishMessage publish)
        {
            if (this.PublishReceived != null)
            {
                this.PublishReceived(this,
                    new MqttPublishEventArgs(publish.Topic, publish.Message, publish.DupFlag, publish.QosLevel, publish.Retain));
            }
        }

		/// <summary>
		/// Wrapper method for raising published message event
		/// </summary>
		/// <param name="messageId">Message identifier for published message</param>
		/// <param name="isPublished">Publish flag</param>
		protected virtual void OnPublished(ushort messageId, bool isPublished)
        {
            if (this.Published != null)
            {
                this.Published(this,
                    new MqttPublishCompleteEventArgs(messageId, isPublished));
            }
        }

		/// <summary>
		/// Wrapper method for raising subscribed topic event
		/// </summary>
		/// <param name="suback">SUBACK message received</param>
		protected virtual void OnSubscribed(MqttSubscribeAcknowledgeMessage suback)
        {
            if (this.Subscribed != null)
            {
                this.Subscribed(this,
                    new MqttSubscribeAcknowledgeEventArgs(suback.MessageId, suback.GrantedQoSLevels));
            }
        }

		/// <summary>
		/// Wrapper method for raising unsubscribed topic event
		/// </summary>
		/// <param name="messageId">Message identifier for unsubscribed topic</param>
		protected virtual void OnUnsubscribed(ushort messageId)
        {
            if (this.Unsubscribed != null)
            {
                this.Unsubscribed(this,
                    new MqttUnsubscribeAcknowledgeEventArgs(messageId));
            }
        }

        /// <summary>
        /// Wrapper method for peer/client disconnection
        /// </summary>
        protected virtual void OnConnectionClosed()
        {
            if (this.ConnectionClosed != null)
                this.ConnectionClosed(this, EventArgs.Empty);
        }

		#endregion // Protected events

		#region Private fields and method

		/// <summary>
		/// Send a message
		/// </summary>
		/// <param name="msgBytes">Message bytes</param>
		private void Send(byte[] msgBytes)
        {
            try
            {
                // send message
                this.channel.Send(msgBytes);

                // update last message sent ticks
                this.lastCommTime = Environment.TickCount;
            }
            catch (Exception e)
            {
				MqttLogger.WriteLine(LogLevel.Error, "Exception on send: {0}", e.ToString());
				throw new MqttCommunicationException(e);
            }
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="msg">Message</param>
        private void Send(MqttMessage msg)
        {
			this.Send(msg.GetBytes((byte)this.ProtocolVersion));
        }

        /// <summary>
        /// Send a message to the broker and wait answer
        /// </summary>
        /// <param name="msgBytes">Message bytes</param>
        /// <returns>MQTT message response</returns>
        private MqttMessage SendReceive(byte[] msgBytes)
        {
            return this.SendReceive(msgBytes, MqttSettings.MQTT_DEFAULT_TIMEOUT);
        }

        /// <summary>
        /// Send a message to the broker and wait answer
        /// </summary>
        /// <param name="msg">Message</param>
        /// <returns>MQTT message response</returns>
        private MqttMessage SendReceive(MqttMessage msg)
        {
            return this.SendReceive(msg, MqttSettings.MQTT_DEFAULT_TIMEOUT);
        }

        /// <summary>
        /// Send a message to the broker and wait answer
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="timeout">Timeout for receiving answer</param>
        /// <returns>MQTT message response</returns>
        private MqttMessage SendReceive(MqttMessage msg, int timeout)
        {
			return this.SendReceive(msg.GetBytes((byte)this.ProtocolVersion), timeout);
        }

        /// <summary>
        /// Send a message to the broker and wait answer
        /// </summary>
        /// <param name="msgBytes">Message bytes</param>
        /// <param name="timeout">Timeout for receiving answer</param>
        /// <returns>MQTT message response</returns>
        private MqttMessage SendReceive(byte[] msgBytes, int timeout)
        {
            // reset handle before sending
            this.syncEndReceiving.Reset();
            try
            {
                // send message
                this.channel.Send(msgBytes);

                // update last message sent ticks
                this.lastCommTime = Environment.TickCount;
            }
            catch (Exception e)
            {
                SendReceiveConnectionResetUpdate(e);
				MqttLogger.WriteLine(LogLevel.Error, "Exception on sendreive: {0}", e.ToString());
				throw new MqttCommunicationException(e);
            }

            // wait for answer from broker
            if (WaitServerHandleResponse(timeout, "broker"))
            {
                // message received without exception
                if (this.exReceiving == null)
                    return this.msgReceived;
                // receiving thread catched exception
                else
                    throw this.exReceiving;
            }
            else
            {
                // throw timeout exception
                throw new MqttCommunicationException();
            }
        }

        /// <summary>
        /// Enqueue a message into the inflight queue
        /// </summary>
        /// <param name="msg">Message to enqueue</param>
        /// <param name="flow">Message flow (publish, acknowledge)</param>
        /// <returns>Message enqueued or not</returns>
        private bool EnqueueInflight(MqttMessage msg, MqttMessageFlow flow)
        {
            // enqueue is needed (or not)
            bool enqueue = true;

            // if it is a PUBLISH message with QoS Level 2
            if ((msg.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE) &&
                (msg.QosLevel == MqttMessage.QOS_LEVEL_EXACTLY_ONCE))
            {
                lock (this.inflightQueue)
                {
                    // if it is a PUBLISH message already received (it is in the inflight queue), the publisher
                    // re-sent it because it didn't received the PUBREC. In this case, we have to re-send PUBREC

                    // NOTE : I need to find on message id and flow because the broker could be publish/received
                    //        to/from client and message id could be the same (one tracked by broker and the other by client)
                    MqttMessageContextFinder msgCtxFinder = new MqttMessageContextFinder(msg.MessageId, MqttMessageFlow.ToAcknowledge);
                    MqttMessageContext msgCtx = (MqttMessageContext)this.inflightQueue.FirstOrDefault(msgCtxFinder.Find);

                    // the PUBLISH message is alredy in the inflight queue, we don't need to re-enqueue but we need
                    // to change state to re-send PUBREC
                    if (msgCtx != null)
                    {
                        msgCtx.State = MqttMessageState.QueuedQos2;
                        msgCtx.Flow = MqttMessageFlow.ToAcknowledge;
                        enqueue = false;
                    }
                }
            }

            if (enqueue)
            {
				// set a default state
				MqttMessageState state = MqttMessageState.QueuedQos0;

                // based on QoS level, the messages flow between broker and client changes
                switch (msg.QosLevel)
                {
                    // QoS Level 0
                    case MqttMessage.QOS_LEVEL_AT_MOST_ONCE:
                        state = MqttMessageState.QueuedQos0;
                        break;

                    // QoS Level 1
                    case MqttMessage.QOS_LEVEL_AT_LEAST_ONCE:
                        state = MqttMessageState.QueuedQos1;
                        break;

                    // QoS Level 2
                    case MqttMessage.QOS_LEVEL_EXACTLY_ONCE:
                        state = MqttMessageState.QueuedQos2;
                        break;
                }

                // [v3.1.1] SUBSCRIBE and UNSUBSCRIBE aren't "officially" QOS = 1
                //          so QueuedQos1 state isn't valid for them
                if (msg.Type == MqttMessage.MQTT_MSG_SUBSCRIBE_TYPE)
                    state = MqttMessageState.SendSubscribe;
                else if (msg.Type == MqttMessage.MQTT_MSG_UNSUBSCRIBE_TYPE)
                    state = MqttMessageState.SendUnsubscribe;

                // queue message context
                MqttMessageContext msgContext = new MqttMessageContext()
                {
                    Message = msg,
                    State = state,
                    Flow = flow,
                    Attempt = 0
                };

                lock (this.inflightQueue)
                {
                    // check number of messages inside inflight queue 
                    enqueue = (this.inflightQueue.Count < this.settings.InflightQueueSize);

                    if (enqueue)
                    {
                        // enqueue message and unlock send thread
                        this.inflightQueue.Enqueue(msgContext);
						MqttLogger.WriteLine(LogLevel.Queuing, "enqueued {0}", msg);

                        // PUBLISH message
                        if (msg.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE)
                        {
                            // to publish and QoS level 1 or 2
                            if ((msgContext.Flow == MqttMessageFlow.ToPublish) &&
                                ((msg.QosLevel == MqttMessage.QOS_LEVEL_AT_LEAST_ONCE) || (msg.QosLevel == MqttMessage.QOS_LEVEL_EXACTLY_ONCE)))
                            {
                                if (this.session != null)
                                    this.session.InflightMessages.Add(msgContext.Key, msgContext);
                            }
                            // to acknowledge and QoS level 2
                            else if ((msgContext.Flow == MqttMessageFlow.ToAcknowledge) && (msg.QosLevel == MqttMessage.QOS_LEVEL_EXACTLY_ONCE))
                            {
                                if (this.session != null)
                                    this.session.InflightMessages.Add(msgContext.Key, msgContext);
                            }
                        }
                    }
                }
            }

            this.inflightWaitHandle.Set();

            return enqueue;
        }

        /// <summary>
        /// Enqueue a message into the internal queue
        /// </summary>
        /// <param name="msg">Message to enqueue</param>
        private void EnqueueInternal(MqttMessage msg)
        {
            // enqueue is needed (or not)
            bool enqueue = true;

            // if it is a PUBREL message (for QoS Level 2)
            if (msg.Type == MqttMessage.MQTT_MSG_PUBREL_TYPE)
            {
                lock (this.inflightQueue)
                {
                    // if it is a PUBREL but the corresponding PUBLISH isn't in the inflight queue,
                    // it means that we processed PUBLISH message and received PUBREL and we sent PUBCOMP
                    // but publisher didn't receive PUBCOMP so it re-sent PUBREL. We need only to re-send PUBCOMP.

                    // NOTE : I need to find on message id and flow because the broker could be publish/received
                    //        to/from client and message id could be the same (one tracked by broker and the other by client)
                    MqttMessageContextFinder msgCtxFinder = new MqttMessageContextFinder(msg.MessageId, MqttMessageFlow.ToAcknowledge);
                    MqttMessageContext msgCtx = (MqttMessageContext)this.inflightQueue.FirstOrDefault(msgCtxFinder.Find);

                    // the PUBLISH message isn't in the inflight queue, it was already processed so
                    // we need to re-send PUBCOMP only
                    if (msgCtx == null)
                    {
                        MqttPublishCompleteMessage pubcomp = new MqttPublishCompleteMessage();
                        pubcomp.MessageId = msg.MessageId;

                        this.Send(pubcomp);

                        enqueue = false;
                    }
                }
            }
            // if it is a PUBCOMP message (for QoS Level 2)
            else if (msg.Type == MqttMessage.MQTT_MSG_PUBCOMP_TYPE)
            {
                lock (this.inflightQueue)
                {
                    // if it is a PUBCOMP but the corresponding PUBLISH isn't in the inflight queue,
                    // it means that we sent PUBLISH message, sent PUBREL (after receiving PUBREC) and already received PUBCOMP
                    // but publisher didn't receive PUBREL so it re-sent PUBCOMP. We need only to ignore this PUBCOMP.

                    // NOTE : I need to find on message id and flow because the broker could be publish/received
                    //        to/from client and message id could be the same (one tracked by broker and the other by client)
                    MqttMessageContextFinder msgCtxFinder = new MqttMessageContextFinder(msg.MessageId, MqttMessageFlow.ToPublish);
                    MqttMessageContext msgCtx = (MqttMessageContext)this.inflightQueue.FirstOrDefault(msgCtxFinder.Find);

                    // the PUBLISH message isn't in the inflight queue, it was already sent so we need to ignore this PUBCOMP
                    if (msgCtx == null)
                    {
                        enqueue = false;
                    }
                }
            }
            // if it is a PUBREC message (for QoS Level 2)
            else if (msg.Type == MqttMessage.MQTT_MSG_PUBREC_TYPE)
            {
                lock (this.inflightQueue)
                {
                    // if it is a PUBREC but the corresponding PUBLISH isn't in the inflight queue,
                    // it means that we sent PUBLISH message more times (retries) but broker didn't send PUBREC in time
                    // the publish is failed and we need only to ignore this PUBREC.

                    // NOTE : I need to find on message id and flow because the broker could be publish/received
                    //        to/from client and message id could be the same (one tracked by broker and the other by client)
                    MqttMessageContextFinder msgCtxFinder = new MqttMessageContextFinder(msg.MessageId, MqttMessageFlow.ToPublish);
                    MqttMessageContext msgCtx = (MqttMessageContext)this.inflightQueue.FirstOrDefault(msgCtxFinder.Find);

                    // the PUBLISH message isn't in the inflight queue, it was already sent so we need to ignore this PUBREC
                    if (msgCtx == null)
                    {
                        enqueue = false;
                    }
                }
            }

            if (enqueue)
            {
                lock (this.internalQueue)
                {
                    this.internalQueue.Enqueue(msg);
					MqttLogger.WriteLine(LogLevel.Queuing, "enqueued {0}", msg);
                    this.inflightWaitHandle.Set();
                }
            }
        }

        /// <summary>
        /// Thread for receiving messages
        /// </summary>
        private void ReceiveThread()
        {
            int readBytes = 0;
            byte[] fixedHeaderFirstByte = new byte[1];
            byte msgType;

            while (this.isRunning)
            {
                try
                {
                    // read first byte (fixed header)
                    readBytes = this.channel.Receive(fixedHeaderFirstByte);

                    if (readBytes > 0)
                    {
                        // extract message type from received byte
                        msgType = (byte)((fixedHeaderFirstByte[0] & MqttMessage.MSG_TYPE_MASK) >> MqttMessage.MSG_TYPE_OFFSET);

                        switch (msgType)
                        {
                            // CONNECT message received
                            case MqttMessage.MQTT_MSG_CONNECT_TYPE:
                                throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);
                                
                            // CONNACK message received
                            case MqttMessage.MQTT_MSG_CONNACK_TYPE:
                                this.msgReceived = MqttConnectAcknowledgeMessage.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this.channel);
								MqttLogger.WriteLine(LogLevel.Frame, "recv {0}", this.msgReceived);
								this.syncEndReceiving.Set();
                                break;

                            // PINGREQ message received
                            case MqttMessage.MQTT_MSG_PINGREQ_TYPE:
                                throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                            // PINGRESP message received
                            case MqttMessage.MQTT_MSG_PINGRESP_TYPE:
                                this.msgReceived = MqttPingResponseMessage.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this.channel);
								MqttLogger.WriteLine(LogLevel.Frame, "recv {0}", this.msgReceived);
								this.syncEndReceiving.Set();
                                break;

                            // SUBSCRIBE message received
                            case MqttMessage.MQTT_MSG_SUBSCRIBE_TYPE:
                                throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                            // SUBACK message received
                            case MqttMessage.MQTT_MSG_SUBACK_TYPE:
                                // enqueue SUBACK message received (for QoS Level 1) into the internal queue
                                MqttSubscribeAcknowledgeMessage suback = MqttSubscribeAcknowledgeMessage.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this.channel);
								MqttLogger.WriteLine(LogLevel.Frame, "recv {0}", suback);

								// enqueue SUBACK message into the internal queue
								this.EnqueueInternal(suback);
                                break;

                            // PUBLISH message received
                            case MqttMessage.MQTT_MSG_PUBLISH_TYPE:
                                MqttPublishMessage publish = MqttPublishMessage.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this.channel);
								MqttLogger.WriteLine(LogLevel.Frame, "recv {0}", publish);

								// enqueue PUBLISH message to acknowledge into the inflight queue
								this.EnqueueInflight(publish, MqttMessageFlow.ToAcknowledge);
                                break;

                            // PUBACK message received
                            case MqttMessage.MQTT_MSG_PUBACK_TYPE:
                                // enqueue PUBACK message received (for QoS Level 1) into the internal queue
                                MqttPublishAcknowledgeMessage puback = MqttPublishAcknowledgeMessage.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this.channel);
								MqttLogger.WriteLine(LogLevel.Frame, "recv {0}", puback);

								// enqueue PUBACK message into the internal queue
								this.EnqueueInternal(puback);
                                break;

                            // PUBREC message received
                            case MqttMessage.MQTT_MSG_PUBREC_TYPE:
                                // enqueue PUBREC message received (for QoS Level 2) into the internal queue
                                MqttPublishRecordedMessage pubrec = MqttPublishRecordedMessage.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this.channel);
								MqttLogger.WriteLine(LogLevel.Frame, "recv {0}", pubrec);

								// enqueue PUBREC message into the internal queue
								this.EnqueueInternal(pubrec);
                                break;

                            // PUBREL message received
                            case MqttMessage.MQTT_MSG_PUBREL_TYPE:
                                // enqueue PUBREL message received (for QoS Level 2) into the internal queue
                                MqttPublishReleaseMessage pubrel = MqttPublishReleaseMessage.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this.channel);
								MqttLogger.WriteLine(LogLevel.Frame, "recv {0}", pubrel);

								// enqueue PUBREL message into the internal queue
								this.EnqueueInternal(pubrel);
                                break;
                                
                            // PUBCOMP message received
                            case MqttMessage.MQTT_MSG_PUBCOMP_TYPE:
                                // enqueue PUBCOMP message received (for QoS Level 2) into the internal queue
                                MqttPublishCompleteMessage pubcomp = MqttPublishCompleteMessage.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this.channel);
								MqttLogger.WriteLine(LogLevel.Frame, "recv {0}", pubcomp);

								// enqueue PUBCOMP message into the internal queue
								this.EnqueueInternal(pubcomp);
                                break;

                            // UNSUBSCRIBE message received
                            case MqttMessage.MQTT_MSG_UNSUBSCRIBE_TYPE:
                                throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                            // UNSUBACK message received
                            case MqttMessage.MQTT_MSG_UNSUBACK_TYPE:
                                // enqueue UNSUBACK message received (for QoS Level 1) into the internal queue
                                MqttUnsubscribeAcknowledgeMessage unsuback = MqttUnsubscribeAcknowledgeMessage.Parse(fixedHeaderFirstByte[0], (byte)this.ProtocolVersion, this.channel);
								MqttLogger.WriteLine(LogLevel.Frame, "recv {0}", unsuback);

								// enqueue UNSUBACK message into the internal queue
								this.EnqueueInternal(unsuback);
                                break;

                            // DISCONNECT message received
                            case MqttDisconnectMessage.MQTT_MSG_DISCONNECT_TYPE:
                                throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                            default:
                                throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);
                        }

                        this.exReceiving = null;
                    }
                    // zero bytes read, peer gracefully closed socket
                    else
                    {
                        // wake up thread that will notify connection is closing
                        this.OnConnectionClosing();
                    }
                }
                catch (Exception e)
                {
                    ReceiveThreadExceptionHandler(e);
                }
            }
        }
        
        private void ReceiveThreadExceptionHandler(Exception e)
        {
			MqttLogger.WriteLine(LogLevel.Error, "exception {0}", e.ToString());
			this.exReceiving = new MqttCommunicationException(e);

            bool close = false;
            if (e.GetType() == typeof(MqttClientException))
            {
                // [v3.1.1] scenarios the receiver MUST close the network connection
                MqttClientException ex = e as MqttClientException;
                close = ((ex.ErrorCode == MqttClientErrorCode.InvalidFlagBits) || 
                    (ex.ErrorCode == MqttClientErrorCode.InvalidProtocolName) ||
                    (ex.ErrorCode == MqttClientErrorCode.InvalidConnectFlags));
            }
            else
            {
                close = ReceiveThreadIsFatalException(e);
            }
                    
            if (close)
            {
                // wake up thread that will notify connection is closing
                this.OnConnectionClosing();
            }            
        }

        /// <summary>
        /// Process inflight messages queue
        /// </summary>
        private void ProcessInflightThread()
        {
            MqttMessageContext msgContext = null;
            MqttMessage msgInflight = null;
            MqttMessage msgReceived = null;
            InternalEvent internalEvent = null;
            bool acknowledge = false;
            int timeout = Timeout.Infinite;
            int delta;
            bool msgReceivedProcessed = false;

            try
            {
                while (this.isRunning)
                {
                    // wait on message queueud to inflight
                    WaitServerHandleResponse(timeout, "inflight");

                    // it could be unblocked because Close() method is joining
                    if (this.isRunning)
                    {
                        lock (this.inflightQueue)
                        {
                            // message received and peeked from internal queue is processed
                            // NOTE : it has the corresponding message in inflight queue based on messageId
                            //        (ex. a PUBREC for a PUBLISH, a SUBACK for a SUBSCRIBE, ...)
                            //        if it's orphan we need to remove from internal queue
                            msgReceivedProcessed = false;
                            acknowledge = false;
                            msgReceived = null;

                            // set timeout tu MaxValue instead of Infinte (-1) to perform
                            // compare with calcultad current msgTimeout
                            timeout = Int32.MaxValue;

                            // a message inflight could be re-enqueued but we have to
                            // analyze it only just one time for cycle
                            int count = this.inflightQueue.Count;
                            // process all inflight queued messages
                            while (count > 0)
                            {
                                count--;
                                acknowledge = false;
                                msgReceived = null;

                                // check to be sure that client isn't closing and all queues are now empty !
                                if (!this.isRunning)
                                    break;

                                // dequeue message context from queue
                                msgContext = (MqttMessageContext)this.inflightQueue.Dequeue();

                                // get inflight message
                                msgInflight = (MqttMessage)msgContext.Message;

                                switch (msgContext.State)
                                {
                                    case MqttMessageState.QueuedQos0:
                                        // QoS 0, PUBLISH message to send to broker, no state change, no acknowledge
                                        if (msgContext.Flow == MqttMessageFlow.ToPublish)
                                        {
                                            this.Send(msgInflight);
                                        }
                                        // QoS 0, no need acknowledge
                                        else if (msgContext.Flow == MqttMessageFlow.ToAcknowledge)
                                        {
                                            internalEvent = new InternalMessageEvent(msgInflight);
                                            // notify published message from broker (no need acknowledged)
                                            this.OnInternalEvent(internalEvent);
                                        }
										MqttLogger.WriteLine(LogLevel.Queuing, "processed {0}", msgInflight);
										break;

                                    case MqttMessageState.QueuedQos1:
                                    // [v3.1.1] SUBSCRIBE and UNSIBSCRIBE aren't "officially" QOS = 1
                                    case MqttMessageState.SendSubscribe:
                                    case MqttMessageState.SendUnsubscribe:
                                        // QoS 1, PUBLISH or SUBSCRIBE/UNSUBSCRIBE message to send to broker, state change to wait PUBACK or SUBACK/UNSUBACK
                                        if (msgContext.Flow == MqttMessageFlow.ToPublish)
                                        {
                                            msgContext.Timestamp = Environment.TickCount;
                                            msgContext.Attempt++;

                                            if (msgInflight.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE)
                                            {
                                                // PUBLISH message to send, wait for PUBACK
                                                msgContext.State = MqttMessageState.WaitForPuback;
                                                // retry ? set dup flag [v3.1.1] only for PUBLISH message
                                                if (msgContext.Attempt > 1)
                                                    msgInflight.DupFlag = true;
                                            }
                                            else if (msgInflight.Type == MqttMessage.MQTT_MSG_SUBSCRIBE_TYPE)
                                                // SUBSCRIBE message to send, wait for SUBACK
                                                msgContext.State = MqttMessageState.WaitForSuback;
                                            else if (msgInflight.Type == MqttMessage.MQTT_MSG_UNSUBSCRIBE_TYPE)
                                                // UNSUBSCRIBE message to send, wait for UNSUBACK
                                                msgContext.State = MqttMessageState.WaitForUnsuback;

                                            this.Send(msgInflight);

                                            // update timeout : minimum between delay (based on current message sent) or current timeout
                                            timeout = (this.settings.DelayOnRetry < timeout) ? this.settings.DelayOnRetry : timeout;

                                            // re-enqueue message (I have to re-analyze for receiving PUBACK, SUBACK or UNSUBACK)
                                            this.inflightQueue.Enqueue(msgContext);
                                        }
                                        // QoS 1, PUBLISH message received from broker to acknowledge, send PUBACK
                                        else if (msgContext.Flow == MqttMessageFlow.ToAcknowledge)
                                        {
                                            MqttPublishAcknowledgeMessage puback = new MqttPublishAcknowledgeMessage();
                                            puback.MessageId = msgInflight.MessageId;

                                            this.Send(puback);

                                            internalEvent = new InternalMessageEvent(msgInflight);
                                            // notify published message from broker and acknowledged
                                            this.OnInternalEvent(internalEvent);
											MqttLogger.WriteLine(LogLevel.Queuing, "processed {0}", msgInflight);
										}
										break;

                                    case MqttMessageState.QueuedQos2:
                                        // QoS 2, PUBLISH message to send to broker, state change to wait PUBREC
                                        if (msgContext.Flow == MqttMessageFlow.ToPublish)
                                        {
                                            msgContext.Timestamp = Environment.TickCount;
                                            msgContext.Attempt++;
                                            msgContext.State = MqttMessageState.WaitForPubrec;
                                            // retry ? set dup flag
                                            if (msgContext.Attempt > 1)
                                                msgInflight.DupFlag = true;

                                            this.Send(msgInflight);

                                            // update timeout : minimum between delay (based on current message sent) or current timeout
                                            timeout = (this.settings.DelayOnRetry < timeout) ? this.settings.DelayOnRetry : timeout;

                                            // re-enqueue message (I have to re-analyze for receiving PUBREC)
                                            this.inflightQueue.Enqueue(msgContext);
                                        }
                                        // QoS 2, PUBLISH message received from broker to acknowledge, send PUBREC, state change to wait PUBREL
                                        else if (msgContext.Flow == MqttMessageFlow.ToAcknowledge)
                                        {
                                            MqttPublishRecordedMessage pubrec = new MqttPublishRecordedMessage();
                                            pubrec.MessageId = msgInflight.MessageId;

                                            msgContext.State = MqttMessageState.WaitForPubrel;

                                            this.Send(pubrec);

                                            // re-enqueue message (I have to re-analyze for receiving PUBREL)
                                            this.inflightQueue.Enqueue(msgContext);
                                        }
                                        break;

                                    case MqttMessageState.WaitForPuback:
                                    case MqttMessageState.WaitForSuback:
                                    case MqttMessageState.WaitForUnsuback:
                                        // QoS 1, waiting for PUBACK of a PUBLISH message sent or
                                        //        waiting for SUBACK of a SUBSCRIBE message sent or
                                        //        waiting for UNSUBACK of a UNSUBSCRIBE message sent or
                                        if (msgContext.Flow == MqttMessageFlow.ToPublish)
                                        {
                                            acknowledge = false;
                                            lock (this.internalQueue)
                                            {
                                                if (this.internalQueue.Count > 0)
                                                    msgReceived = (MqttMessage)this.internalQueue.Peek();
                                            }

                                            // it is a PUBACK message or a SUBACK/UNSUBACK message
                                            if (msgReceived != null)
                                            {
                                                // PUBACK message or SUBACK/UNSUBACK message for the current message
                                                if (((msgReceived.Type == MqttMessage.MQTT_MSG_PUBACK_TYPE) && (msgInflight.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE) && (msgReceived.MessageId == msgInflight.MessageId)) ||
                                                    ((msgReceived.Type == MqttMessage.MQTT_MSG_SUBACK_TYPE) && (msgInflight.Type == MqttMessage.MQTT_MSG_SUBSCRIBE_TYPE) && (msgReceived.MessageId == msgInflight.MessageId)) ||
                                                    ((msgReceived.Type == MqttMessage.MQTT_MSG_UNSUBACK_TYPE) && (msgInflight.Type == MqttMessage.MQTT_MSG_UNSUBSCRIBE_TYPE) && (msgReceived.MessageId == msgInflight.MessageId)))
                                                {
                                                    lock (this.internalQueue)
                                                    {
                                                        // received message processed
                                                        this.internalQueue.Dequeue();
                                                        acknowledge = true;
                                                        msgReceivedProcessed = true;
														MqttLogger.WriteLine(LogLevel.Queuing, "dequeued {0}", msgReceived);
													}

													// if PUBACK received, confirm published with flag
													if (msgReceived.Type == MqttMessage.MQTT_MSG_PUBACK_TYPE)
                                                        internalEvent = new PublishCompleteEvent(msgReceived, true);
                                                    else
                                                        internalEvent = new InternalMessageEvent(msgReceived);

                                                    // notify received acknowledge from broker of a published message or subscribe/unsubscribe message
                                                    this.OnInternalEvent(internalEvent);

                                                    // PUBACK received for PUBLISH message with QoS Level 1, remove from session state
                                                    if ((msgInflight.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE) &&
                                                        (this.session != null) && IsInflightMessage(msgContext.Key))
                                                    {
                                                        this.session.InflightMessages.Remove(msgContext.Key);
                                                    }

													MqttLogger.WriteLine(LogLevel.Queuing, "processed {0}", msgInflight);
												}
											}

                                            // current message not acknowledged, no PUBACK or SUBACK/UNSUBACK or not equal messageid 
                                            if (!acknowledge)
                                            {
                                                delta = Environment.TickCount - msgContext.Timestamp;
                                                // check timeout for receiving PUBACK since PUBLISH was sent or
                                                // for receiving SUBACK since SUBSCRIBE was sent or
                                                // for receiving UNSUBACK since UNSUBSCRIBE was sent
                                                if (delta >= this.settings.DelayOnRetry)
                                                {
                                                    // max retry not reached, resend
                                                    if (msgContext.Attempt < this.settings.AttemptsOnRetry)
                                                    {
                                                        msgContext.State = MqttMessageState.QueuedQos1;

                                                        // re-enqueue message
                                                        this.inflightQueue.Enqueue(msgContext);

                                                        // update timeout (0 -> reanalyze queue immediately)
                                                        timeout = 0;
                                                    }
                                                    else
                                                    {
                                                        // if PUBACK for a PUBLISH message not received after retries, raise event for not published
                                                        if (msgInflight.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE)
                                                        {
                                                            // PUBACK not received in time, PUBLISH retries failed, need to remove from session inflight messages too
                                                            if ((this.session != null) && IsInflightMessage(msgContext.Key))
                                                            {
                                                                this.session.InflightMessages.Remove(msgContext.Key);
                                                            }

                                                            internalEvent = new PublishCompleteEvent(msgInflight, false);

                                                            // notify not received acknowledge from broker and message not published
                                                            this.OnInternalEvent(internalEvent);
                                                        }
                                                        // NOTE : not raise events for SUBACK or UNSUBACK not received for the user no event raised means subscribe/unsubscribe failed
                                                    }
                                                }
                                                else
                                                {
                                                    // re-enqueue message (I have to re-analyze for receiving PUBACK, SUBACK or UNSUBACK)
                                                    this.inflightQueue.Enqueue(msgContext);

                                                    // update timeout
                                                    int msgTimeout = (this.settings.DelayOnRetry - delta);
                                                    timeout = (msgTimeout < timeout) ? msgTimeout : timeout;
                                                }
                                            }
                                        }
                                        break;

                                    case MqttMessageState.WaitForPubrec:
                                        // QoS 2, waiting for PUBREC of a PUBLISH message sent
                                        if (msgContext.Flow == MqttMessageFlow.ToPublish)
                                        {
                                            acknowledge = false;
                                            lock (this.internalQueue)
                                            {
                                                if (this.internalQueue.Count > 0)
                                                    msgReceived = (MqttMessage)this.internalQueue.Peek();
                                            }

                                            // it is a PUBREC message
                                            if ((msgReceived != null) && (msgReceived.Type == MqttMessage.MQTT_MSG_PUBREC_TYPE))
                                            {
                                                // PUBREC message for the current PUBLISH message, send PUBREL, wait for PUBCOMP
                                                if (msgReceived.MessageId == msgInflight.MessageId)
                                                {
                                                    lock (this.internalQueue)
                                                    {
                                                        // received message processed
                                                        this.internalQueue.Dequeue();
                                                        acknowledge = true;
                                                        msgReceivedProcessed = true;
														MqttLogger.WriteLine(LogLevel.Queuing, "dequeued {0}", msgReceived);
													}

													MqttPublishReleaseMessage pubrel = new MqttPublishReleaseMessage();
                                                    pubrel.MessageId = msgInflight.MessageId;

                                                    msgContext.State = MqttMessageState.WaitForPubcomp;
                                                    msgContext.Timestamp = Environment.TickCount;
                                                    msgContext.Attempt = 1;

                                                    this.Send(pubrel);

                                                    // update timeout : minimum between delay (based on current message sent) or current timeout
                                                    timeout = (this.settings.DelayOnRetry < timeout) ? this.settings.DelayOnRetry : timeout;

                                                    // re-enqueue message
                                                    this.inflightQueue.Enqueue(msgContext);
                                                }
                                            }

                                            // current message not acknowledged
                                            if (!acknowledge)
                                            {
                                                delta = Environment.TickCount - msgContext.Timestamp;
                                                // check timeout for receiving PUBREC since PUBLISH was sent
                                                if (delta >= this.settings.DelayOnRetry)
                                                {
                                                    // max retry not reached, resend
                                                    if (msgContext.Attempt < this.settings.AttemptsOnRetry)
                                                    {
                                                        msgContext.State = MqttMessageState.QueuedQos2;

                                                        // re-enqueue message
                                                        this.inflightQueue.Enqueue(msgContext);

                                                        // update timeout (0 -> reanalyze queue immediately)
                                                        timeout = 0;
                                                    }
                                                    else
                                                    {
                                                        // PUBREC not received in time, PUBLISH retries failed, need to remove from session inflight messages too
                                                        if ((this.session != null) && IsInflightMessage(msgContext.Key))
                                                        {
                                                            this.session.InflightMessages.Remove(msgContext.Key);
                                                        }

                                                        // if PUBREC for a PUBLISH message not received after retries, raise event for not published
                                                        internalEvent = new PublishCompleteEvent(msgInflight, false);
                                                        // notify not received acknowledge from broker and message not published
                                                        this.OnInternalEvent(internalEvent);
                                                    }
                                                }
                                                else
                                                {
                                                    // re-enqueue message
                                                    this.inflightQueue.Enqueue(msgContext);

                                                    // update timeout
                                                    int msgTimeout = (this.settings.DelayOnRetry - delta);
                                                    timeout = (msgTimeout < timeout) ? msgTimeout : timeout;
                                                }
                                            }
                                        }
                                        break;

                                    case MqttMessageState.WaitForPubrel:
                                        // QoS 2, waiting for PUBREL of a PUBREC message sent
                                        if (msgContext.Flow == MqttMessageFlow.ToAcknowledge)
                                        {
                                            lock (this.internalQueue)
                                            {
                                                if (this.internalQueue.Count > 0)
                                                    msgReceived = (MqttMessage)this.internalQueue.Peek();
                                            }

                                            // it is a PUBREL message
                                            if ((msgReceived != null) && (msgReceived.Type == MqttMessage.MQTT_MSG_PUBREL_TYPE))
                                            {
                                                // PUBREL message for the current message, send PUBCOMP
                                                if (msgReceived.MessageId == msgInflight.MessageId)
                                                {
                                                    lock (this.internalQueue)
                                                    {
                                                        // received message processed
                                                        this.internalQueue.Dequeue();
                                                        msgReceivedProcessed = true;
														MqttLogger.WriteLine(LogLevel.Queuing, "dequeued {0}", msgReceived);
													}

													MqttPublishCompleteMessage pubcomp = new MqttPublishCompleteMessage();
                                                    pubcomp.MessageId = msgInflight.MessageId;

                                                    this.Send(pubcomp);

                                                    internalEvent = new InternalMessageEvent(msgInflight);
                                                    // notify published message from broker and acknowledged
                                                    this.OnInternalEvent(internalEvent);

                                                    // PUBREL received (and PUBCOMP sent) for PUBLISH message with QoS Level 2, remove from session state
                                                    if ((msgInflight.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE) &&
                                                        (this.session != null) && IsInflightMessage(msgContext.Key))
                                                    {
                                                        this.session.InflightMessages.Remove(msgContext.Key);
                                                    }

													MqttLogger.WriteLine(LogLevel.Queuing, "processed {0}", msgInflight);
												}
												else
                                                {
                                                    // re-enqueue message
                                                    this.inflightQueue.Enqueue(msgContext);
                                                }
                                            }
                                            else
                                            {
                                                // re-enqueue message
                                                this.inflightQueue.Enqueue(msgContext);
                                            }
                                        }
                                        break;

                                    case MqttMessageState.WaitForPubcomp:
                                        // QoS 2, waiting for PUBCOMP of a PUBREL message sent
                                        if (msgContext.Flow == MqttMessageFlow.ToPublish)
                                        {
                                            acknowledge = false;
                                            lock (this.internalQueue)
                                            {
                                                if (this.internalQueue.Count > 0)
                                                    msgReceived = (MqttMessage)this.internalQueue.Peek();
                                            }

                                            // it is a PUBCOMP message
                                            if ((msgReceived != null) && (msgReceived.Type == MqttMessage.MQTT_MSG_PUBCOMP_TYPE))
                                            {
                                                // PUBCOMP message for the current message
                                                if (msgReceived.MessageId == msgInflight.MessageId)
                                                {
                                                    lock (this.internalQueue)
                                                    {
                                                        // received message processed
                                                        this.internalQueue.Dequeue();
                                                        acknowledge = true;
                                                        msgReceivedProcessed = true;
														MqttLogger.WriteLine(LogLevel.Queuing, "dequeued {0}", msgReceived);
													}

													internalEvent = new PublishCompleteEvent(msgReceived, true);
                                                    // notify received acknowledge from broker of a published message
                                                    this.OnInternalEvent(internalEvent);

                                                    // PUBCOMP received for PUBLISH message with QoS Level 2, remove from session state
                                                    if ((msgInflight.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE) &&
                                                        (this.session != null) && IsInflightMessage(msgContext.Key))
                                                    {
                                                        this.session.InflightMessages.Remove(msgContext.Key);
                                                    }
													MqttLogger.WriteLine(LogLevel.Queuing, "processed {0}", msgInflight);
												}
											}
                                            // it is a PUBREC message
                                            else if ((msgReceived != null) && (msgReceived.Type == MqttMessage.MQTT_MSG_PUBREC_TYPE))
                                            {
                                                // another PUBREC message for the current message due to a retransmitted PUBLISH
                                                // I'm in waiting for PUBCOMP, so I can discard this PUBREC
                                                if (msgReceived.MessageId == msgInflight.MessageId)
                                                {
                                                    lock (this.internalQueue)
                                                    {
                                                        // received message processed
                                                        this.internalQueue.Dequeue();
                                                        acknowledge = true;
                                                        msgReceivedProcessed = true;
														MqttLogger.WriteLine(LogLevel.Queuing, "dequeued {0}", msgReceived);

														// re-enqueue message
														this.inflightQueue.Enqueue(msgContext);
                                                    }
                                                }
                                            }

                                            // current message not acknowledged
                                            if (!acknowledge)
                                            {
                                                delta = Environment.TickCount - msgContext.Timestamp; 
                                                // check timeout for receiving PUBCOMP since PUBREL was sent
                                                if (delta >= this.settings.DelayOnRetry)
                                                {
                                                    // max retry not reached, resend
                                                    if (msgContext.Attempt < this.settings.AttemptsOnRetry)
                                                    {
                                                        msgContext.State = MqttMessageState.SendPubrel;

                                                        // re-enqueue message
                                                        this.inflightQueue.Enqueue(msgContext);

                                                        // update timeout (0 -> reanalyze queue immediately)
                                                        timeout = 0;
                                                    }
                                                    else
                                                    {
                                                        // PUBCOMP not received, PUBREL retries failed, need to remove from session inflight messages too
                                                        if ((this.session != null) && IsInflightMessage(msgContext.Key))
                                                        {
                                                            this.session.InflightMessages.Remove(msgContext.Key);
                                                        }

                                                        // if PUBCOMP for a PUBLISH message not received after retries, raise event for not published
                                                        internalEvent = new PublishCompleteEvent(msgInflight, false);
                                                        // notify not received acknowledge from broker and message not published
                                                        this.OnInternalEvent(internalEvent);
                                                    }
                                                }
                                                else
                                                {
                                                    // re-enqueue message
                                                    this.inflightQueue.Enqueue(msgContext);

                                                    // update timeout
                                                    int msgTimeout = (this.settings.DelayOnRetry - delta);
                                                    timeout = (msgTimeout < timeout) ? msgTimeout : timeout;
                                                }
                                            }
                                        }
                                        break;

                                    case MqttMessageState.SendPubrec:
                                        // TODO : impossible ? --> QueuedQos2 ToAcknowledge
                                        break;

                                    case MqttMessageState.SendPubrel:
                                        // QoS 2, PUBREL message to send to broker, state change to wait PUBCOMP
                                        if (msgContext.Flow == MqttMessageFlow.ToPublish)
                                        {
                                            MqttPublishReleaseMessage pubrel = new MqttPublishReleaseMessage();
                                            pubrel.MessageId = msgInflight.MessageId;

                                            msgContext.State = MqttMessageState.WaitForPubcomp;
                                            msgContext.Timestamp = Environment.TickCount;
                                            msgContext.Attempt++;
                                            // retry ? set dup flag [v3.1.1] no needed
                                            if (this.ProtocolVersion == MqttProtocolVersion.Version_3_1)
                                            {
                                                if (msgContext.Attempt > 1)
                                                    pubrel.DupFlag = true;
                                            }
                                            
                                            this.Send(pubrel);

                                            // update timeout : minimum between delay (based on current message sent) or current timeout
                                            timeout = (this.settings.DelayOnRetry < timeout) ? this.settings.DelayOnRetry : timeout;

                                            // re-enqueue message
                                            this.inflightQueue.Enqueue(msgContext);
                                        }
                                        break;

                                    case MqttMessageState.SendPubcomp:
                                        // TODO : impossible ?
                                        break;
                                        
                                    case MqttMessageState.SendPuback:
                                        // TODO : impossible ? --> QueuedQos1 ToAcknowledge
                                        break;
                                        
                                    default:
                                        break;
                                }
                            }

                            // if calculated timeout is MaxValue, it means that must be Infinite (-1)
                            if (timeout == Int32.MaxValue)
                                timeout = Timeout.Infinite;

                            // if message received is orphan, no corresponding message in inflight queue
                            // based on messageId, we need to remove from the queue
                            if ((msgReceived != null) && !msgReceivedProcessed)
                            {
                                this.internalQueue.Dequeue();
								MqttLogger.WriteLine(LogLevel.Queuing, "dequeued orphan {0}", msgReceived);
							}
						}
                    }
                }
            }
            catch (MqttCommunicationException e)
            {
                // possible exception on Send, I need to re-enqueue not sent message
                if (msgContext != null)
                    // re-enqueue message
                    this.inflightQueue.Enqueue(msgContext);

				MqttLogger.WriteLine(LogLevel.Error, "exception {0}", e.ToString());

				// raise disconnection client event
				this.OnConnectionClosing();
            }
        }

        /// <summary>
        /// Thread for handling keep alive message
        /// </summary>
        private void KeepAliveThread()
        {
            int delta = 0;
            int wait = this.keepAlivePeriod;
            
            // create event to signal that current thread is end
            this.keepAliveEventEnd = new AutoResetEvent(false);

            while (this.isRunning)
            {
                // waiting...
                WaitServerHandleResponse(wait, "keepAlive");

                if (this.isRunning)
                {
                    delta = Environment.TickCount - this.lastCommTime;

                    // if timeout exceeded ...
                    if (delta >= this.keepAlivePeriod)
                    {
                        // ... send keep alive
						this.Ping();
						wait = this.keepAlivePeriod;
                    }
                    else
                    {
                        // update waiting time
                        wait = this.keepAlivePeriod - delta;
                    }
                }
            }

            // signal thread end
            this.keepAliveEventEnd.Set();
        }
        
        /// <summary>
        /// Thread for raising event
        /// </summary>
        private void DispatchEventThread()
        {
            while (this.isRunning)
            {
                if ((this.eventQueue.Count == 0) && !this.isConnectionClosing)
                    // wait on receiving message from client
                    this.receiveEventWaitHandle.WaitOne();

                // check if it is running or we are closing client
                if (this.isRunning)
                {
                    // get event from queue
                    InternalEvent internalEvent = null;
                    lock (this.eventQueue)
                    {
                        if (this.eventQueue.Count > 0)
                            internalEvent = (InternalEvent)this.eventQueue.Dequeue();
                    }

                    // it's an event with a message inside
                    if (internalEvent != null)
                    {
                        MqttMessage msg = ((InternalMessageEvent)internalEvent).Message;

                        if (msg != null)
                        {
                            switch (msg.Type)
                            {
                                // CONNECT message received
                                case MqttMessage.MQTT_MSG_CONNECT_TYPE:
                                    throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                                // SUBSCRIBE message received
                                case MqttMessage.MQTT_MSG_SUBSCRIBE_TYPE:
                                    throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                                // SUBACK message received
                                case MqttMessage.MQTT_MSG_SUBACK_TYPE:
                                    // raise subscribed topic event (SUBACK message received)
                                    this.OnSubscribed((MqttSubscribeAcknowledgeMessage)msg);
                                    break;

                                // PUBLISH message received
                                case MqttMessage.MQTT_MSG_PUBLISH_TYPE:
                                    // PUBLISH message received in a published internal event, no publish succeeded
                                    if (internalEvent.GetType() == typeof(PublishCompleteEvent))
                                        this.OnPublished(msg.MessageId, false);
                                    else
                                        // raise PUBLISH message received event 
                                        this.OnPublishReceived((MqttPublishMessage)msg);
                                    break;

                                // PUBACK message received
                                case MqttMessage.MQTT_MSG_PUBACK_TYPE:
                                    // raise published message event
                                    // (PUBACK received for QoS Level 1)
                                    this.OnPublished(msg.MessageId, true);
                                    break;

                                // PUBREL message received
                                case MqttMessage.MQTT_MSG_PUBREL_TYPE:
                                    // raise message received event 
                                    // (PUBREL received for QoS Level 2)
                                    this.OnPublishReceived((MqttPublishMessage)msg);
                                    break;

                                // PUBCOMP message received
                                case MqttMessage.MQTT_MSG_PUBCOMP_TYPE:
                                    // raise published message event
                                    // (PUBCOMP received for QoS Level 2)
                                    this.OnPublished(msg.MessageId, true);
                                    break;

                                // UNSUBSCRIBE message received from client
                                case MqttMessage.MQTT_MSG_UNSUBSCRIBE_TYPE:
                                    throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);

                                // UNSUBACK message received
                                case MqttMessage.MQTT_MSG_UNSUBACK_TYPE:
                                    // raise unsubscribed topic event
                                    this.OnUnsubscribed(msg.MessageId);
                                    break;

                                // DISCONNECT message received from client
                                case MqttDisconnectMessage.MQTT_MSG_DISCONNECT_TYPE:
                                    throw new MqttClientException(MqttClientErrorCode.WrongBrokerMessage);
                            }
                        }
                    }
                    
                    // all events for received messages dispatched, check if there is closing connection
                    if ((this.eventQueue.Count == 0) && this.isConnectionClosing)
                    {
                        // client must close connection
                        this.Close();

                        // client raw disconnection
                        this.OnConnectionClosed();
                    }
                }
            }
        }

        /// <summary>
        /// Restore session
        /// </summary>
        private void RestoreSession()
        {
            // if not clean session
            if (!this.CleanSession)
            {
                // there is a previous session
                if (this.session != null)
                {
                    lock (this.inflightQueue)
                    {
                        foreach (MqttMessageContext msgContext in this.session.InflightMessages.Values)
                        {
                            this.inflightQueue.Enqueue(msgContext);

                            // if it is a PUBLISH message to publish
                            if ((msgContext.Message.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE) &&
                                (msgContext.Flow == MqttMessageFlow.ToPublish))
                            {
                                // it's QoS 1 and we haven't received PUBACK
                                if ((msgContext.Message.QosLevel == MqttMessage.QOS_LEVEL_AT_LEAST_ONCE) &&
                                    (msgContext.State == MqttMessageState.WaitForPuback))
                                {
                                    // we haven't received PUBACK, we need to resend PUBLISH message
                                    msgContext.State = MqttMessageState.QueuedQos1;
                                }
                                // it's QoS 2
                                else if (msgContext.Message.QosLevel == MqttMessage.QOS_LEVEL_EXACTLY_ONCE)
                                {
                                    // we haven't received PUBREC, we need to resend PUBLISH message
                                    if (msgContext.State == MqttMessageState.WaitForPubrec)
                                    {
                                        msgContext.State = MqttMessageState.QueuedQos2;
                                    }
                                    // we haven't received PUBCOMP, we need to resend PUBREL for it
                                    else if (msgContext.State == MqttMessageState.WaitForPubcomp)
                                    {
                                        msgContext.State = MqttMessageState.SendPubrel;
                                    }
                                }
                            }
                        }
                    }

                    // unlock process inflight queue
                    this.inflightWaitHandle.Set();
                }
                else
                {
                    // create new session
                    this.session = new MqttClientSession(this.ClientId);
                }
            }
            // clean any previous session
            else
            {
                if (this.session != null)
                    this.session.Clear();
            }
        }

        /// <summary>
        /// Generate the next message identifier
        /// </summary>
        /// <returns>Message identifier</returns>
        private ushort GetMessageId()
        {
            // if 0 or max UInt16, it becomes 1 (first valid messageId)
            this.messageIdCounter = ((this.messageIdCounter % UInt16.MaxValue) != 0) ? (ushort)(this.messageIdCounter + 1) : (ushort)1;
            return this.messageIdCounter;
        }

		#endregion // Private fields and method

		/// <summary>
		/// Finder class for PUBLISH message inside a queue
		/// </summary>
		internal class MqttMessageContextFinder
        {
            // PUBLISH message id
            internal ushort MessageId { get; set; }
            // message flow into inflight queue
            internal MqttMessageFlow Flow { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="messageId">Message Id</param>
            /// <param name="flow">Message flow inside inflight queue</param>
            internal MqttMessageContextFinder(ushort messageId, MqttMessageFlow flow)
            {
                this.MessageId = messageId;
                this.Flow = flow;
            }

            internal bool Find(object item)
            {
				MqttMessageContext msgCtx = (MqttMessageContext)item;
                return ((msgCtx.Message.Type == MqttMessage.MQTT_MSG_PUBLISH_TYPE) &&
                    (msgCtx.Message.MessageId == this.MessageId) &&
                    msgCtx.Flow == this.Flow);
            }
        }
    }
}
