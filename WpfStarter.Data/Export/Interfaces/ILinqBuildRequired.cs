namespace WpfStarter.Data.Export
{
    public interface ILinqBuildRequired : IDatabaseAction
    {
       public string LINQExpression { get; set; }
    }
}