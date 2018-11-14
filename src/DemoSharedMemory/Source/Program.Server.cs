using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Security;
using Standard.IPC.SharedMemory;

namespace SharedMemoryDemo
{
    partial class Program
    {
        [SecuritySafeCritical]
        internal static void RunServer(int bufferSize = 1048576, int count = 50)
        {
            byte[] readData;
            int size = sizeof(byte) * bufferSize;

            // Generate data to be written
            byte[][] dataList = new byte[256][];
            for (var j = 0; j < 256; j++)
            {
                var data = new byte[bufferSize];
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)((i + j) % 255);
                }
                dataList[j] = data;
            }

            Console.WriteLine("Press <enter> to start Server");
            Console.ReadLine();

            Console.WriteLine("Creating shared memory circular buffer");
            using (var theServer = new CircularBuffer("TEST", count, size))
            {
                Console.WriteLine("Circular buffer producer created.");
                Console.WriteLine("Ready for client...");
                Thread.Sleep(1000);

                int skipCount = 0;
                long iterations = 0;
                long totalBytes = 0;
                long lastTick = 0;
                Stopwatch sw = Stopwatch.StartNew();                
                int threadCount = 0;
                Action writer = () =>
                {
                    int myThreadIndex = Interlocked.Increment(ref threadCount);
                    int linesOut = 0;
                    bool finalLine = false;
                    for (; ; )
                    {
                        readData = dataList[iterations % 255];

                        int amount = theServer.Write(readData, 100);
                        //int amount = theServer.Write<byte>(readData, 100);

                        if (amount == 0)
                        {
                            Interlocked.Increment(ref skipCount);
                        }
                        else
                        {
                            Interlocked.Add(ref totalBytes, amount);
                            Interlocked.Increment(ref iterations);
                        }

                        if (threadCount == 1 && Interlocked.Read(ref iterations) > 500)
                            finalLine = true;

                        if (myThreadIndex < 3 && (finalLine || sw.ElapsedTicks - lastTick > 1000000))
                        {
                            lastTick = sw.ElapsedTicks;

                            Console.WriteLine("Write: {0}, Wait: {1}, {2}MB/s", 
                                ((double)totalBytes / 1048576.0).ToString("F0"), 
                                skipCount, 
                                (((totalBytes / 1048576.0) / sw.ElapsedMilliseconds) * 1000).ToString("F2"));

                            linesOut++;
                            if (finalLine || (myThreadIndex > 1 && linesOut > 10))
                            {
                                Console.WriteLine("Completed.");
                                break;
                            }
                        }
                    }
                };

                writer();
                Console.WriteLine("");
                skipCount = 0;
                iterations = 0;
                totalBytes = 0;
                lastTick = 0;
                sw.Reset();
                sw.Start();

                Console.WriteLine("Testing throughput...");
#if NETFX
                Task s1 = Task.Factory.StartNew(writer);
#else
                ThreadPool.QueueUserWorkItem((o) => { writer(); });
#endif
                //Task s2 = Task.Factory.StartNew(writer);
                //Task s3 = Task.Factory.StartNew(writer);
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
        }
    }
}
