using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfStarter.Data.Export;

namespace WpfStarter.Data.Export
{
    /// <summary>
    /// 
    /// </summary>
    public class GeneralPurveyor
    {
        public string LinqExpression { get; set; }
        public string TargetFormat { get; set; }
        public string SourceFilePath { get; set; }    
        public Inference PostOperationInference { get; set; }
        public Exception CurrentExeption { get; set; }
    }
}
