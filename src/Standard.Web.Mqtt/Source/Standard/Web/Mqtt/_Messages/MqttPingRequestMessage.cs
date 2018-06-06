namespace Standard.Web.Mqtt
{
	/// <summary>
	/// Ping request message to test whether the connection is alive.
	/// </summary>
	/// <remarks>
	/// This is an implementation of the <c>PUBREQ</c> message specification.
	/// </remarks>
	public class MqttPingRequestMessage : MqttMessage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MqttPingRequestMessage()
        {
            this.type = MQTT_MSG_PINGREQ_TYPE;
        }

        public override byte[] GetBytes(byte protocolVersion)
        {
            byte[] buffer = new byte[2];
            int index = 0;

            // first fixed header byte
            if (protocolVersion == MqttConnectMessage.PROTOCOL_VERSION_V3_1_1)
                buffer[index++] = (MQTT_MSG_PINGREQ_TYPE << MSG_TYPE_OFFSET) | MQTT_MSG_PINGREQ_FLAG_BITS; // [v.3.1.1]
            else
                buffer[index++] = (MQTT_MSG_PINGREQ_TYPE << MSG_TYPE_OFFSET);
            buffer[index++] = 0x00;

            return buffer;
        }

        /// <summary>
        /// Parse bytes for a PINGREQ message
        /// </summary>
        /// <param name="fixedHeaderFirstByte">First fixed header byte</param>
        /// <param name="protocolVersion">Protocol Version</param>
        /// <param name="channel">Channel connected to the broker</param>
        /// <returns>PINGREQ message instance</returns>
        public static MqttPingRequestMessage Parse(byte fixedHeaderFirstByte, byte protocolVersion, IMqttNetworkChannel channel)
        {
			MqttPingRequestMessage msg = new MqttPingRequestMessage();

            if (protocolVersion == MqttConnectMessage.PROTOCOL_VERSION_V3_1_1)
            {
                // [v3.1.1] check flag bits
                if ((fixedHeaderFirstByte & MSG_FLAG_BITS_MASK) != MQTT_MSG_PINGREQ_FLAG_BITS)
                    throw new MqttClientException(MqttClientErrorCode.InvalidFlagBits);
            }

            // already know remaininglength is zero (MQTT specification),
            // so it isn't necessary to read other data from socket
            int remainingLength = MqttMessage.DecodeRemainingLength(channel);

            return msg;
        }

        public override string ToString()
        {
#if TRACE
            return this.GetTraceString(
                "PINGREQ",
                null,
                null);
#else
            return base.ToString();
#endif
        }
    }
}
