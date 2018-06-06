#if WINRT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using System.Threading;

namespace Standard.Web.Mqtt
{
    public class MqttNetworkChannel : IMqttNetworkChannel
    {
        // stream socket for communication
        protected StreamSocket socket;

        // remote host information
        protected HostName remoteHostName;
        protected int remotePort;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket">Socket opened with the client</param>
        public MqttNetworkChannel(StreamSocket socket)
        {
            this.socket = socket;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="remoteHostName">Remote Host name</param>
        /// <param name="remotePort">Remote port</param>
        public MqttNetworkChannel(string remoteHostName, int remotePort)
        {
            this.remoteHostName = new HostName(remoteHostName);
            this.remotePort = remotePort;
        }

        public virtual bool DataAvailable
        {
            get { return true; }
        }

        public virtual int Receive(byte[] buffer)
        {
            IBuffer result;

            // read all data needed (until fill buffer)
            int idx = 0;
            while (idx < buffer.Length)
            {
                // fixed scenario with socket closed gracefully by peer/broker and
                // Read return 0. Avoid infinite loop.

                // read is executed synchronously
                result = this.socket.InputStream.ReadAsync(buffer.AsBuffer(), (uint)buffer.Length, InputStreamOptions.None).AsTask().Result;
                if (result.Length == 0)
                    return 0;
                idx += (int)result.Length;
            }
            return buffer.Length;
        }

        public virtual int Receive(byte[] buffer, int timeout)
        {
            CancellationTokenSource cts = new CancellationTokenSource(timeout);

            try
            {
                IBuffer result;

                // read all data needed (until fill buffer)
                int idx = 0;
                while (idx < buffer.Length)
                {
                    // fixed scenario with socket closed gracefully by peer/broker and
                    // Read return 0. Avoid infinite loop.

                    // read is executed synchronously
                    result = this.socket.InputStream.ReadAsync(buffer.AsBuffer(), (uint)buffer.Length, InputStreamOptions.None).AsTask(cts.Token).Result;
                    if (result.Length == 0)
                        return 0;
                    idx += (int)result.Length;
                }
                return buffer.Length;
            }
            catch (TaskCanceledException)
            {
                return 0;
            }
        }

        public virtual int Send(byte[] buffer)
        {
            // send is executed synchronously
            return (int)this.socket.OutputStream.WriteAsync(buffer.AsBuffer()).AsTask().Result;
        }

        public virtual void Close()
        {
            this.socket.Dispose();
        }

        public virtual void Connect()
        {
            this.socket = new StreamSocket();

			// connection is executed synchronously
			this.socket.ConnectAsync(this.remoteHostName, this.remotePort.ToString(), SocketProtectionLevel.PlainSocket).AsTask().Wait();
        }

        public virtual void Accept()
        {
            // TODO : SSL support with StreamSocket / StreamSocketListener seems to be NOT supported
            return;
        }
    }
}

#endif
