using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;

namespace Standard.Web.Mqtt
{
	internal class DnsUtility
	{
		public static IPHostEntry GetHostEntry(string hostNameOrAddress)
		{
#if NETSTANDARD
			return Dns.GetHostEntryAsync(hostNameOrAddress).Result;
#else
			return Dns.GetHostEntry(hostNameOrAddress);
#endif
		}
	}

	internal static partial class Polyfill
	{
#if NETSTANDARD
		public static void Close(this NetworkStream stream)
		{
			stream.Flush();
		}

		public static void Close(this SslStream stream)
		{
			stream.Flush();
		}

		public static void Close(this Socket socket)
		{
			try
			{
				socket.Shutdown(SocketShutdown.Both);
			}
			catch
			{
				// An error occurred when attempting to access the socket or socket has been closed
				// Refer to: https://msdn.microsoft.com/en-us/library/system.net.sockets.socket.shutdown(v=vs.110).aspx
			}
			socket.Dispose();
		}

		public static void AuthenticateAsClient(this SslStream stream, string targetHost)
		{
			stream.AuthenticateAsClientAsync(targetHost).Wait();
			return;
		}
#endif
	}
}

