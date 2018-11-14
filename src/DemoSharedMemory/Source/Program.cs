using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security;

// NOTE
// ----
// To use security critical methods in a lambda function, you need to decorate both the class and method with
// SecuritySafeCritical or SecurityCritical
// https://stackoverflow.com/questions/13956481/how-to-access-a-security-critical-field-from-an-anonymous-delegate-or-lambda
//

namespace SharedMemoryDemo
{
    [SecuritySafeCritical]
    internal partial class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine("Welcome to the shared memory demo");
			Console.WriteLine("=================================");
			Console.WriteLine("[s] Run in server mode");
			Console.WriteLine("[c] Run in client mode");
			Console.WriteLine("[p] Run in single process mode");
			Console.WriteLine("Enter your selection:");
			ConsoleKeyInfo key = Console.ReadKey();

			while (true)
			{
				if (key.Key == ConsoleKey.S)
					break;
				else if (key.Key == ConsoleKey.C)
					break;
				else if (key.Key == ConsoleKey.P)
					break;

				Console.Error.WriteLine("ERROR! You need to enter either 's', 'c', or 'p'.");
				key = Console.ReadKey();
			}

            int bufferSize = 1048576;
            int count = 50;

			if (key.Key == ConsoleKey.S)
			{
				Console.WriteLine("Starting server pipe. Press 'q' to quit...");
				RunServer(bufferSize, count);
			}
			else if (key.Key == ConsoleKey.C)
			{
				Console.WriteLine("Starting server pipe. Press 'q' to quit...");
				RunClient();
			}
			else
			{
				Console.WriteLine("Starting single process...");

	            int serverCount = 1; // number of writers. higher = slower
	            int clientCount = 1;  // number of readers. higher = slower
	            int elements = 100000; // number of elements to process

				RunSingleProcess(serverCount, clientCount, elements, bufferSize, count);
			}
        }
    }
}
