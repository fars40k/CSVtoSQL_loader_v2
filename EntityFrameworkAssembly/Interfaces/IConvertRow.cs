using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoSQL.Models.Operations
{
    public interface IConvertRow
    {
        string ConvertRule(string tablerow);
    }
}
