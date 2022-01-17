using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        public enum EnumGlobalState
        {
            DbConnectionFailed,
            DbConnected,
            FileSelected,
            Disabled,
            CriticalError
        }
    }
}