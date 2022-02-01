namespace WpfStarter.Data.Export
{
    /// <summary>
    /// Adds agregated field to database operation
    /// </summary>
    public interface IParametrisedAction<T>
    {
        T Settings { get; set; }
    }
}
