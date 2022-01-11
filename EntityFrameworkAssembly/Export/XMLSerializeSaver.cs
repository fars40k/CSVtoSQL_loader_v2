using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfStarter.Data.Export
{
    public class XMLSerializeSaver : EntityToXML
    {
        public string filePath { get; private set; }
        XmlSerializer serializer;

        public XMLSerializeSaver(string newFilePath, object typeToSerial)
        {
            serializer = new XmlSerializer(typeToSerial.GetType());
            filePath = newFilePath;
        }

        public override bool Run()
        {
            throw new NotImplementedException();
        }
    }
}
