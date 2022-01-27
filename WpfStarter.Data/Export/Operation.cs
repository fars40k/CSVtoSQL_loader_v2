using Prism.Ioc;
using System.Threading.Tasks;
using System.Windows;

namespace WpfStarter.Data.Export
{
    public class Operation : IDatabaseAction
    {
        protected string _description;
        protected CancellationToken cancellationToken;

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

        public virtual Task<string> RunAsync(IContainerProvider provider,CancellationToken newToken)
        {
            cancellationToken = newToken;

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
