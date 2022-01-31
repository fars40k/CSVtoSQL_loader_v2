namespace WpfStarter.Data.Export
{
    public interface IParametrisedAction<T>
    {
        T Settings { get; set; }
    }
}
