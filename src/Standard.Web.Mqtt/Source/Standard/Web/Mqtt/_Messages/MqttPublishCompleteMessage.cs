namespace Standard.Web.Mqtt
{
	/// <summary>
	/// The third part of a QoS 2 flow, representing that the message has been published by the broker.
	/// </summary>
	/// <remarks>
	/// This is an implementation of the <c>PUBCOMP</c> message specification.
	/// </remarks>
	public class MqttPublishCompleteMessage : MqttMessage
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public MqttPublishCompleteMessage()
        {
            this.type = MQTT_MSG_PUBCOMP_TYPE;
        }

        public override byte[] GetBytes(byte protocolVersion)
        {
            int fixedHeaderSize = 0;
            int varHeaderSize = 0;
            int payloadSize = 0;
            int remainingLength = 0;
            byte[] buffer;
            int index = 0;

            // message identifier
            varHeaderSize += MESSAGE_ID_SIZE;

            remainingLength += (varHeaderSize + payloadSize);

            // first byte of fixed header
            fixedHeaderSize = 1;

            int temp = remainingLength;
            // increase fixed header size based on remaining length
            // (each remaining length byte can encode until 128)
            do
            {
                fixedHeaderSize++;
                temp = temp / 128;
            } while (temp > 0);

            // allocate buffer for message
            buffer = new byte[fixedHeaderSize + varHeaderSize + payloadSize];

            // first fixed header byte
            if (protocolVersion == MqttConnectMessage.PROTOCOL_VERSION_V3_1_1)
                buffer[index++] = (MQTT_MSG_PUBCOMP_TYPE << MSG_TYPE_OFFSET) | MQTT_MSG_PUBCOMP_FLAG_BITS; // [v.3.1.1]
            else
                buffer[index++] = (MQTT_MSG_PUBCOMP_TYPE << MSG_TYPE_OFFSET);

            // encode remaining length
            index = this.EncodeRemainingLength(remainingLength, buffer, index);

            // get message identifier
            buffer[index++] = (byte)((this.messageId >> 8) & 0x00FF); // MSB
            buffer[index++] = (byte)(this.messageId & 0x00FF); // LSB 

            return buffer;
        }

        /// <summary>
        /// Parse bytes for a PUBCOMP message
        /// </summary>
        /// <param name="fixedHeaderFirstByte">First fixed header byte</param>
        /// <param name="protocolVersion">Protocol Version</param>
        /// <param name="channel">Channel connected to the broker</param>
        /// <returns>PUBCOMP message instance</returns>
        public static MqttPublishCompleteMessage Parse(byte fixedHeaderFirstByte, byte protocolVersion, IMqttNetworkChannel channel)
        {
            byte[] buffer;
            int index = 0;
			MqttPublishCompleteMessage msg = new MqttPublishCompleteMessage();

            if (protocolVersion == MqttConnectMessage.PROTOCOL_VERSION_V3_1_1)
            {
                // [v3.1.1] check flag bits
                if ((fixedHeaderFirstByte & MSG_FLAG_BITS_MASK) != MQTT_MSG_PUBCOMP_FLAG_BITS)
                    throw new MqttClientException(MqttClientErrorCode.InvalidFlagBits);
            }

            // get remaining length and allocate buffer
            int remainingLength = MqttMessage.DecodeRemainingLength(channel);
            buffer = new byte[remainingLength];

            // read bytes from socket...
            channel.Receive(buffer);

            // message id
            msg.messageId = (ushort)((buffer[index++] << 8) & 0xFF00);
            msg.messageId |= (buffer[index++]);

            return msg;
        }

        public override string ToString()
        {
#if TRACE
            return this.GetTraceString(
                "PUBCOMP",
                new object[] { "messageId" },
                new object[] { this.messageId });
#else
            return base.ToString();
#endif
        }
    }
}
