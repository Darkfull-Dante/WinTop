using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

namespace WinTop.System
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateVisitor : IVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="computer"></param>
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardware"></param>
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensor"></param>
        public void VisitSensor(ISensor sensor) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void VisitParameter(IParameter parameter) { }
    }
}
