using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.Data.Export
{
    public class Inference : IPostprocessingObject
    {
        public int TotalProcessed { get; set; }
        public int TotalFailed { get; set; }

        public Inference()
        {
            TotalProcessed = 0;
            TotalFailed = 0;
        }

        public string ToString(string afore)
        {
            return (TotalFailed != 0) ? afore + TotalFailed.ToString() + " / " + TotalProcessed.ToString()
                                      : TotalProcessed.ToString() + " / " + TotalProcessed.ToString();
        }
    }
}
