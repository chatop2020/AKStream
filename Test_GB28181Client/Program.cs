using System;
using System.Net;
using SIPSorcery.Net;
using SIPSorcery.Sys;

namespace Test_GB28181Client
{
    class Program
    {
        static SctpUdpTransport _sctpTransport;
        static void Main(string[] args)
        {
            Console.WriteLine("SCTP Client Test Console");

      

             _sctpTransport = new SctpUdpTransport();
            var association = _sctpTransport.Associate(
                new IPEndPoint(IPAddress.Parse("192.168.0.50"), 11111), 4444, 13);

            association.OnAssociationStateChanged += (state) =>
            {
                Console.WriteLine($"SCTP client association state changed to {state}.");
                if (state == SctpAssociationState.Established)
                {
                    //byte[] buffer = new byte[66301];
                    //Crypto.GetRandomBytes(buffer);
                    //association.SendData(0, 0, buffer);
                    //association.SendData(0, 0, "hi\n");
                }
            };

            association.OnData += (frame) =>
            {
                if (frame.UserData?.Length > 0)
                {
                    Console.WriteLine($"Data received: {Encoding.UTF8.GetString(frame.UserData)}");
                    //Console.WriteLine($"Data received {frame.UserData.Length}.");
                }
                else
                {
                    Console.WriteLine($"Data chunk received with no data.");
                }
            };

            association.OnAbortReceived += (reason) =>
            {
                Console.WriteLine($"ABORT received from peer, reason {reason}.");
            };

            Console.WriteLine("press any key to exit...");
            Console.ReadLine();

            if (association.State == SctpAssociationState.Established)
            {
                Console.WriteLine("Sending shutdown...");
                association.Shutdown();
                await Task.Delay(1000);
            }

            Console.WriteLine("Exiting.");
        }
    }
}