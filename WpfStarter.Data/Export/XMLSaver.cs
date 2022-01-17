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
    public class XMLSaver : EntityToXML
    {
        public string filePath { get; private set; }
        XmlSerializer serializer;

        public XMLSaver(string newFilePath, object typeToSerial)
        {
            serializer = new XmlSerializer(typeToSerial.GetType());
            filePath = newFilePath;
        }

        public override bool Run()
        {
            try
            {
                XDocument xDoc;
                using (PersonsContext pC = new PersonsContext())
                {
                    if (!File.Exists(filePath))
                    {
                        xDoc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null),
                        new XElement("TestProgram")
                        );
                    }
                    else
                    {
                        xDoc = XDocument.Load(filePath);
                    }

                    foreach (Person person in pC.Persons)
                    {
                        xDoc.Element("TestProgram").Add(PersonToXmlRecord(person));
                    }                   
                }

                xDoc.Save(filePath);
                return (true);

            }
            catch (Exception ex)
            {
                return (false);
            }
        }


        public XElement PersonToXmlRecord(Person person)
        {
            XElement record = new XElement("Record",
                new XAttribute("Id",person.ID.ToString()),
                new XElement("Date", person.Date.ToString()),
                new XElement("FirstName",person.FirstName.ToString()),
                new XElement("LastName", person.LastName.ToString()),
                new XElement("SurName", person.SurName.ToString()),
                new XElement("City", person.City.ToString()),
                new XElement("Country", person.Country.ToString())
                );

            return(record);
        }
    }
}
