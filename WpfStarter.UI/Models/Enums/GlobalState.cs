namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        public enum GlobalState
        {
            DbConnectionFailed = 10,
            DbConnected = 11,
            FileSelected = 21,
            Disabled = 30,
            CriticalError = 40
        }
    }
}