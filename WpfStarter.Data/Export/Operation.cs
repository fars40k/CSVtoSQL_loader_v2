using System.Windows;

namespace WpfStarter.Data.Export
{
    public class Operation : IDatabaseAction
    {
        public string Description { get; set; }

        public Operation(string newDescription)
        {
            if (Description == null) Description = newDescription;
        }

        public Operation()
        {
            Description = "missing";
        }

        public override string ToString()
        {
            return Description ?? "missing description";
        }

        public virtual string Run()
        {
            MessageBox.Show("Empty base class running requested");
            return "";
        }
    }
}
