namespace WpfStarter.Data
{
    public class DataViewsLocalisation
    {
        public Dictionary<string, string> _dataViewsStrings { get; private set; }

        public DataViewsLocalisation()
        {
            _dataViewsStrings = new Dictionary<string, string>();
            _dataViewsStrings.Add("Operation 1", "Read CSV");
            _dataViewsStrings.Add("Operation 2", "Database to Excel");
            _dataViewsStrings.Add("Operation 3", "Database to XML");
            _dataViewsStrings.Add("Date", "Date");
            _dataViewsStrings.Add("FirstName", "First name");
            _dataViewsStrings.Add("LastName", "Last name");
            _dataViewsStrings.Add("SurName", "Sur name");
            _dataViewsStrings.Add("City", "City");
            _dataViewsStrings.Add("Country", "Country");
            _dataViewsStrings.Add("Filter 1", "Select all");
            _dataViewsStrings.Add("Filter 2", "Select only");
        }

        public void SetNewViewString(string key, string local)
        {
            if (_dataViewsStrings.ContainsKey(key)) _dataViewsStrings[key] = local;
        }
    }
}