namespace WpfStarter.Data.Export
{
    public interface IRequiringBuildLinq : IDatabaseAction
    {
       public string LinqExpression { get; set; }
    }
}