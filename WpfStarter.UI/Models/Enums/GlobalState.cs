namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        public enum GlobalState
        {
            DbConnectionFailed,
            DbConnected,
            FileSelected,
            Disabled,
            CriticalError
        }
    }
}