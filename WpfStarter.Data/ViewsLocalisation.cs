using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStarter.Data
{
    internal class ViewsLocalisation
    {
        public readonly string[] ColumnNames = new string[6];
        public readonly string[] FilterPressetNames = new string[2];
        public readonly string[] OperationNames = new string[3];

        public string[] LocalisedColumnNames { get; private set; } = new string[6];
        public string[] LocalisedPresetNames { get; private set; } = new string[2];
        public string[] LocalisedOperationNames { get; private set; } = new string[3];

        public ViewsLocalisation() 
        {
            ColumnNames[0] = "Date";
            ColumnNames[1] = "FirstName";
            ColumnNames[2] = "SurName";
            ColumnNames[3] = "LastName";
            ColumnNames[4] = "City";
            ColumnNames[5] = "Country";
            FilterPressetNames[0] = "All records";
            FilterPressetNames[1] = "Only records with \b match";
            OperationNames[0] = "Fill table from .csv file";
            OperationNames[1] = "Save database data to Excel file";
            OperationNames[2] = "Save database data to .xml file";

            LocalisedPresetNames = FilterPressetNames;
            LocalisedColumnNames = ColumnNames;
            LocalisedOperationNames = OperationNames;
        }

        public void SetColumnNames(params string[] newNames)
        {
            int i = 0;
            foreach (string name in newNames)
            {
                if ((name.Length >= 1)&&(name.Length <30))
                {
                    LocalisedColumnNames[i] = name;
                }
                i++;
            }
        }

        public void SetFilterPressetNames(params string[] newNames)
        {
            int i = 0;
            foreach (string name in newNames)
            {
                if ((name.Length >= 1) && (name.Length < 50))
                {
                    LocalisedPresetNames[i] = name;
                }
                i++;
            }
        }

        public void SetOperationsNames(params string[] newNames)
        {
            int i = 0;
            foreach (string name in newNames)
            {
                if ((name.Length >= 1) && (name.Length < 50))
                {
                    LocalisedPresetNames[i] = name;
                }
                i++;
            }
        }
    }
}
