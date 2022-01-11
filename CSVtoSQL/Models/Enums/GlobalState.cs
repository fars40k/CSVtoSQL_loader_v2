namespace WpfStarter.UI.Models
{
    public partial class MainModel
    {
        public enum GlobalState
        {
            AppLoaded,
            FileDecided,
            DoingDbWork,
            Disabled,
            CriticalError
        }
    }
}
