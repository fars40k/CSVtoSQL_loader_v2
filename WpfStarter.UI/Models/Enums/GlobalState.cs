namespace WpfStarter.UI.Models
{
    public partial class MainModel
    {
        public enum GlobalState
        {
            AppLoaded,
            FileSelected,
            DoingDbWork,
            Disabled,
            CriticalError
        }
    }
}
