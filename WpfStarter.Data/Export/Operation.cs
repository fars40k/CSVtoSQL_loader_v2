using Prism.Ioc;
using System.Threading.Tasks;
using System.Windows;

namespace WpfStarter.Data.Export
{
    /// <summary>
    /// Base class for any database operation
    /// </summary>
    public class Operation : IDatabaseAction
    {
        protected string _description;
        protected IProgress<string> _progress = new Progress<string>();
        protected CancellationToken _cancelToken = new CancellationToken(false);
        
        public Operation(string newDescription)
        {
           _description = newDescription;
        }

        public Operation()
        {
            
        }

        public override string ToString()
        {
            return _description ?? "missing description";
        }

        public virtual string Run()
        {   
            throw new NotImplementedException();
        }

        public virtual async Task<string> RunTask(IContainerProvider provider)
        {
            _cancelToken = provider.Resolve<CancellationToken>("DataCancellationToken");
            _progress = provider.Resolve<Progress<string>>("DataProgress");
            _progress.Report("0 / ???");
            return Run();
        }

    }
}
