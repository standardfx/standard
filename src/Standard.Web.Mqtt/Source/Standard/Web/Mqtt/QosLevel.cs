namespace Standard.Web.Mqtt
{
    public enum QosLevel : byte
    {
        // QOS levels
        AtMostOnce = MqttMessage.QOS_LEVEL_AT_MOST_ONCE,
        AtLeastOnce = MqttMessage.QOS_LEVEL_AT_LEAST_ONCE,
        ExactlyOnce = MqttMessage.QOS_LEVEL_EXACTLY_ONCE,

        // SUBSCRIBE QoS level granted failure [v3.1.1]
        GrantedFailure = MqttMessage.QOS_LEVEL_GRANTED_FAILURE
    }
} 
