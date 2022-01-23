using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WpfStarter.Data.Export
{
    public class XMLSaver : Operation, ILinqBuildRequired, ISavePathSelectionRequired
    {
        public XMLSaver(IContainerExtension container)
        {
            var ResourceManager = container.Resolve<ResourceManager>();
            Description = ResourceManager.GetString("OpConvToXML") ?? "missing";
            targetFormat = ".xml";
        }

        public string filePath { get; private set; }
        public string targetFormat { get; set; }
        public string LINQExpression { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Run()
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
                    return ("true");
                }; 
                    
            }
            catch (Exception ex)
            {               
                return ("false");
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

        public void SetSavePath(string newFilePath)
        {
            filePath = newFilePath;
        }
    }
}
