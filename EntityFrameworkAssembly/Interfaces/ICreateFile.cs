using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoSQL.Models.Operations
{
    public interface ICreateFile
    {
        bool CreateFile(string path, string name);
    }
}
