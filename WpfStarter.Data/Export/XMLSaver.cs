using System.Collections;
using System.Linq.Dynamic.Core;
using System.Resources;
using System.Xml;
using Prism.Ioc;

namespace WpfStarter.Data.Export
{
    public class XMLSaver : Operation, ILinqBuildRequired, ISavePathSelectionRequired
    {
        public XMLSaver(IContainerExtension container)
        {
            var ResourceManager = container.Resolve<ResourceManager>();
            Description = ResourceManager.GetString("OpConvToXML") ?? "missing";
            TargetFormat = ".xml";
        }

        public string FilePath { get; private set; } = "";
        public string TargetFormat { get; set; }
        public string LINQExpression { get; set; } = "";

        public override string Run()
        {
            string nonDuplicatefilePath = FilePath;
            try
            {

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";

                XmlWriter xmlWriter = XmlWriter.Create(FilePath, settings);

                xmlWriter.WriteStartElement("TestProgram");

                PersonsContext pC = new PersonsContext();


                // Changes source of items if LINQ Expression contains filtering data conditions

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
                    WritePersonToXmlRecord(xmlWriter, person);
                }

                xmlWriter.WriteEndDocument();

                return "true";

            }
            catch (Exception ex)
            {               
                return "false";
            }
        }

        public void WritePersonToXmlRecord(XmlWriter xmlWriter,Person person)
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
            FilePath = newFilePath;
        }
    }
}
