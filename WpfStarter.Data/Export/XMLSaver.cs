using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WpfStarter.Data.Export
{
    public class XMLSaver : Operation, ILinqBuildRequired
    {
        public string filePath { get; private set; }
        DataViewsLocalisation _dataViewsLocalisation;

        public XMLSaver(IContainerExtension container)
        {
            DataViewsLocalisation dwl = container.Resolve<DataViewsLocalisation>();
            Description = dwl._dataViewsStrings["Operation 3"];
        }

        public bool Run()
        {
            string nonDuplicatefilePath = filePath;
            try
            {
                // Checks if file exist and save in incremental marked file
                if (File.Exists(filePath))
                {
                    int increment = 0;

                    while (File.Exists(nonDuplicatefilePath))
                    {
                        increment++;
                        nonDuplicatefilePath = filePath.Replace(".", ("_" + increment.ToString() + "."));
                    }
                }
                using (XmlWriter xmlWriter = XmlWriter.Create(nonDuplicatefilePath))
                {
                    xmlWriter.WriteStartElement("TestProgram");

                    using (PersonsContext pC = new PersonsContext())
                    {
                        foreach (Person person in pC.Persons)
                        {
                            PersonToXmlRecord(xmlWriter,person);
                        }
                    }

                    xmlWriter.WriteEndDocument();
                    return (true);
                }; 
                    
            }
            catch (Exception ex)
            {               
                return (false);
            }
        }


        public void PersonToXmlRecord(XmlWriter xmlWriter,Person person)
        {
            xmlWriter.WriteStartElement("Record");
            xmlWriter.WriteAttributeString("Id", person.ID.ToString());
            xmlWriter.WriteStartElement("Date");
            xmlWriter.WriteString(person.Date.ToString());
            xmlWriter.WriteStartElement("FirstName");
            xmlWriter.WriteString(person.FirstName.ToString());
            xmlWriter.WriteStartElement("LastName");
            xmlWriter.WriteString(person.LastName.ToString());
            xmlWriter.WriteStartElement("SurName");
            xmlWriter.WriteString(person.SurName.ToString());
            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(person.City.ToString());
            xmlWriter.WriteStartElement("Country");
            xmlWriter.WriteString(person.Country.ToString());
        }
    }
}
