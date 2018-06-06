#if NETMF
using System;
using Microsoft.SPOT.Net.Security;

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// MQTT SSL utility class
    /// </summary>
    partial class SslHelper
    {
        public static SslProtocols ToSslPlatformEnum(MqttSslProtocol mqttSslProtocol)
        {
            switch (mqttSslProtocol)
            {
                case MqttSslProtocol.None:
                    return SslProtocols.None;
                case MqttSslProtocol.SSLv3:
                    return SslProtocols.SSLv3;
                case MqttSslProtocol.TLSv1_0:
                    return SslProtocols.TLSv1;
                case MqttSslProtocol.TLSv1_1:
                case MqttSslProtocol.TLSv1_2:
                default:
                    throw new ArgumentException(RS.UnsupportedSslVersion);
            }
        }
    }
}
#endif
