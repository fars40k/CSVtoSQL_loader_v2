using System.Collections;
using System.Linq.Dynamic.Core;
using System.Resources;
using System.Xml;
using Prism.Ioc;

namespace WpfStarter.Data.Export
{
    public class XMLSaver : Operation, IRequiringBuildLinq, IRequiringSavepathSelection
    {
        public string FilePath { get; private set; } = "";
        public string TargetFormat { get; set; }
        public string LinqExpression { get; set; } = "";

        public XMLSaver(IContainerExtension container)
        {
            var resourceManager = container.Resolve<ResourceManager>();
            _description = resourceManager.GetString("OpConvToXML") ?? "missing";
            TargetFormat = ".xml";
        }

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


                int totalEntries = 0;
                object list;
                if (LinqExpression == "")
                {
                    list = pC.Persons;
                    totalEntries = pC.Persons.Count();
                }
                else
                {
                    list = pC.Persons
                                 .Where(LinqExpression)
                                 .ToList();
                    List<Person> persons = (List<Person>)list;
                    totalEntries = persons.Count();
                }

                int iterationsSum = 0;
                foreach (Person person in (IEnumerable)list)
                {
                    WritePersonToXmlRecord(xmlWriter, person);
                    if (cancellationToken.IsCancellationRequested) break;
                    if (iterationsSum % 100 == 0) _progress.Report(iterationsSum + " / " + totalEntries);
                    iterationsSum++;
                }

                xmlWriter.WriteEndDocument();

                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();
                _progress.Report(iterationsSum + " / " + totalEntries);
                return iterationsSum.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
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
