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

        public string ToString()
        {
            return (TotalFailed != 0) ? TotalFailed.ToString() + " / " + TotalProcessed.ToString()
                                      : TotalProcessed.ToString() + " / " + TotalProcessed.ToString();
        }
    }
}
