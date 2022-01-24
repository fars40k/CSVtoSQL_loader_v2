using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
        public string LINQExpression { get; set; } = "";

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

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";

                using (XmlWriter xmlWriter = XmlWriter.Create(nonDuplicatefilePath, settings))
                {
                    xmlWriter.WriteStartElement("TestProgram");

                    using (PersonsContext pC = new PersonsContext())
                    {

                        // If not empty data filters changes source of items
                        object list;
                        if (LINQExpression == "")
                        {
                            list = pC.Persons;
                        }
                        else
                        {
                            list = pC.Persons
                                         .Where(LINQExpression)
                                         .ToList();
                        }

                        foreach (Person person in (IEnumerable)list)
                        {
                            PersonToXmlRecord(xmlWriter, person);
                        }
                    }

                    xmlWriter.WriteEndDocument();
                    xmlWriter.Close();
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
            xmlWriter.WriteAttributeString("Id",person.ID.ToString().Trim());

            xmlWriter.WriteStartElement("Date");
            xmlWriter.WriteString(person.Date.ToShortDateString().Trim());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("FirstName");
            xmlWriter.WriteString(person.FirstName.ToString().Trim());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("LastName");
            xmlWriter.WriteString(person.LastName.ToString().Trim());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("SurName");
            xmlWriter.WriteString(person.SurName.ToString().Trim());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(person.City.ToString().Trim());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Country");
            xmlWriter.WriteString(person.Country.ToString().Trim());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        public void SetSavePath(string newFilePath)
        {
            filePath = newFilePath;
        }
    }
}
