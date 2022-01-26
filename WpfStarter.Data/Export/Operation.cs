using System.Windows;

namespace WpfStarter.Data.Export
{
    public class Operation : IDatabaseAction
    {
        protected string Description;

        public Operation(string newDescription)
        {
           Description = newDescription;
        }

        public Operation()
        {
            
        }

        public override string ToString()
        {
            return Description ?? "missing description";
        }

        public virtual string Run()
        {   
            return "false";
        }
    }
}
