using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandToFitbit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Scheduler scheduler = new Scheduler();
            Task t = scheduler.Start();
            t.Wait();
        }
    }
}
