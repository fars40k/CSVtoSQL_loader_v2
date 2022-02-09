using System.Collections;
using System.Linq.Dynamic.Core;
using System.Resources;
using System.Xml;
using Prism.Ioc;

namespace WpfStarter.Data.Export
{
    public class DefaultXmlExporter : Operation, IRequiringBuildLinq, IRequiringSavepathSelection, IParametrisedAction<Inference>
    {
        public string FilePath { get; private set; } = "";
        public string TargetFormat { get; set; }
        public string LinqExpression { get; set; } = "";
        public Inference Settings { get; set; }

        public DefaultXmlExporter(IContainerExtension container)
        {
            var resourceManager = container.Resolve<ResourceManager>();
            _description = resourceManager.GetString("OpConvToXML") ?? "missing";
            TargetFormat = ".xml";
        }

        public override string Run()
        {
            try
            {

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";

                XmlWriter xmlWriter = XmlWriter.Create(FilePath, settings);
                xmlWriter.WriteStartElement("TestProgram");

                PersonsContext pC = new PersonsContext();

                // Changes source of items if the LINQ Expression contains filtering
                // data conditions and gets the total value of the records
                int totalEntries = 0;
                object recordsList;
                if (LinqExpression == "")
                {
                    recordsList = pC.Persons.ToList();
                    totalEntries = pC.Persons.Count();
                }
                else
                {
                    recordsList = pC.Persons
                                 .Where(LinqExpression)
                                 .ToList();

                    List<Person> persons = (List<Person>)recordsList;
                    totalEntries = persons.Count();
                }

                int iterationsSum = 0;
                foreach (Person person in (IEnumerable)recordsList)
                {
                    WritePersonToXmlRecord(xmlWriter, person);
                    if (_cancelToken.IsCancellationRequested)
                    {
                        xmlWriter.WriteStartElement("Operation");
                        xmlWriter.WriteString("Canceled");
                        xmlWriter.WriteEndElement();
                        xmlWriter.Flush();

                        Thread.Sleep(30);
                        break;
                    }
                    if (iterationsSum % 100 == 0)
                    {   
                        _progress.Report(iterationsSum + " / " + totalEntries);
                    }
                    iterationsSum++;
                }

                pC.Dispose();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                Thread.Sleep(30);

                if (Settings != null) Settings.TotalProcessed = totalEntries;

                if (_cancelToken.IsCancellationRequested) throw new OperationCanceledException();

                return "";

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
            xmlWriter.Flush();
        }

        public void SetSavePath(string newFilePath)
        {
            FilePath = newFilePath;
        }
    }
}
