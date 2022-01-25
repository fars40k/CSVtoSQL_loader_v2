namespace WpfStarter.Data.Export
{
    internal interface ISavePathSelectionRequired : IDatabaseAction
    {
        public string TargetFormat { get; set; } 

        public void SetSavePath(string newFilePath);
    }
}
