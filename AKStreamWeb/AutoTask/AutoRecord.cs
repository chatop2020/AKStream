using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using AKStreamWeb.Services;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.WebRequest;

namespace AKStreamWeb.AutoTask
{
    public class AutoRecord
    {

        private void KeepRecord()
        {
            while (true)
            {
                
                Thread.Sleep(1000);
            }
        }

        
     

        public AutoRecord()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    KeepRecord();
                }
                catch
                {
                  
                }
            })).Start();
        }
    }
}