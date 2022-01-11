using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfStarter.Data.Export
{
    public abstract class FileCSVtoSQL 
    {
        public CancellationToken cancellationToken;

        public abstract bool Run(int maxRecords);

        public abstract string FailedToAllString();
    }
}
