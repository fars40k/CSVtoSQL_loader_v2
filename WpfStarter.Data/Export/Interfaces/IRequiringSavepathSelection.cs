namespace WpfStarter.Data.Export
{
    public interface IRequiringSavepathSelection : IDatabaseAction
    {
        public string TargetFormat { get; set; } 

        public void SetSavePath(string newFilePath);
    }
}
