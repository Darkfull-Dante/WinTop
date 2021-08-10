using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WinTop.Graphics;

namespace WinTop.Components
{
    class ProcessCounter
    {

        private const string CATEGORY = "Process";
        private const string PROC_COUNTER = "% Processor Time";
        private const string MEM_COUNTER = "Private Bytes";


        public PerformanceCounter ProcUsage { get; set; }
        public PerformanceCounter MemUsage { get; set; }
        public float CurrentProcUsage { get; set; }
        public long CurrentMemUsage { get; set; }

        public ProcessCounter(string instance)
        {
            ProcUsage = new PerformanceCounter(CATEGORY, PROC_COUNTER, instance);
            MemUsage = new PerformanceCounter(CATEGORY, MEM_COUNTER, instance);
            Update();
        }

        public static explicit operator ProcessCounter(string instance)
        {
            return new ProcessCounter(instance);
        }

        public void Update()
        {
            CurrentProcUsage = ProcUsage.NextValue();
            CurrentMemUsage = (long)MemUsage.NextValue();
        }
    }
}
