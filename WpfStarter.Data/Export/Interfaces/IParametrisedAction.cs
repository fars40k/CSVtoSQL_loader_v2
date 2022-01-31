namespace WpfStarter.Data.Export
{
    internal interface IParametrisedAction<T>
    {
        T Settings { get; set; }
    }
}
