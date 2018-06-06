using System;
using System.Net;
using System.Threading;
using System.Collections;
using System.IO;

// platform specific code
#if WINRT
using Windows.Networking.Sockets;
#elif NETMF
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.Security;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Net.Security;
#elif NETFX || NETSTANDARD
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Net.Security;
#endif

namespace Standard.Web.Mqtt
{
    /// <summary>
    /// MQTT client with SSL support.
    /// </summary>
    public partial class MqttSecureClient : MqttClient
    {
	}
}
