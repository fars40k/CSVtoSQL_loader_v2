using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVtoSQL.Models
{
    public partial class MainModel
    {
        public enum EnumGlobalState
        {
            AppLoaded,
            FileDecided,
            DbConnected,
            FileAnalysed,
            ConvertAndDbWork,
            Disabled,
            CriticalError
        }
    }
}
