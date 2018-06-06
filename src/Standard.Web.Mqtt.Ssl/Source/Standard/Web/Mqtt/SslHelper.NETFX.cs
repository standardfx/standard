#if NETFX || NETSTANDARD
using System;
using System.Net.Security;
using System.Security.Authentication;

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
                    return SslProtocols.Ssl3;
                case MqttSslProtocol.TLSv1_0:
                    return SslProtocols.Tls;
                case MqttSslProtocol.TLSv1_1:
                    return SslProtocols.Tls11;
                case MqttSslProtocol.TLSv1_2:
                    return SslProtocols.Tls12;
                default:
                    throw new ArgumentException(RS.UnsupportedSslVersion);
            }
        }
    }
}
#endif
