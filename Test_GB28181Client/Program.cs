using System;
using LibGB28181SipClient;

namespace Test_GB28181Client
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("{0}", ("dxper".Equals("net")).ToString().ToUpper());

            SipClient sipClient = new SipClient();
        }
    }
}