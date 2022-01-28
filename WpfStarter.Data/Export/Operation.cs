using Prism.Ioc;
using System.Threading.Tasks;
using System.Windows;

namespace WpfStarter.Data.Export
{
    public class Operation : IDatabaseAction
    {
        protected string _description;
        protected IProgress<string> _progress = new Progress<string>();
        protected CancellationToken cancellationToken = new CancellationToken(false);
        

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
            return "false";
        }

        public virtual Task<string> RunAsync(IContainerProvider provider, CancellationToken newToken, IProgress<string> newReporter)
        {
            cancellationToken = newToken;
            _progress = newReporter;
            _progress.Report("0 / ???");

            TaskCompletionSource<string> tcSource = provider.Resolve<TaskCompletionSource<string>>();       
            Task.Run(() =>
            {
                try
                {
                    var result = Run();
                }
                catch (Exception ex)
                {
                    tcSource.SetException(ex);
                }               
            });

            return tcSource.Task;
        }

    }
}
