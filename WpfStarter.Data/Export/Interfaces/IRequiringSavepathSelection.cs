namespace WpfStarter.Data.Export
{
    internal interface IRequiringSavepathSelection : IDatabaseAction
    {
        public string TargetFormat { get; set; } 

        public void SetSavePath(string newFilePath);
    }
}
