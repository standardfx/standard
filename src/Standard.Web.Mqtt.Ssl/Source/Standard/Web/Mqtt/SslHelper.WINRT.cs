#if WINRT
using System;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// MQTT SSL utility class
    /// </summary>
    partial class SslHelper
    {
        public static SocketProtectionLevel ToSslPlatformEnum(MqttSslProtocol mqttSslProtocol)
        {
            switch (mqttSslProtocol)
            {
                case MqttSslProtocol.None:
                    return SocketProtectionLevel.PlainSocket;
                case MqttSslProtocol.SSLv3:
                    return SocketProtectionLevel.SslAllowNullEncryption;
                case MqttSslProtocol.TLSv1_0:
                    return SocketProtectionLevel.Tls10;
                case MqttSslProtocol.TLSv1_1:
                    return SocketProtectionLevel.Tls11;
                case MqttSslProtocol.TLSv1_2:
                    return SocketProtectionLevel.Tls12;
                default:
                    throw new ArgumentException(RS.UnsupportedSslVersion);
            }
        }
    }
}
#endif
