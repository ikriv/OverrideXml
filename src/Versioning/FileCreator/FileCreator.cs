using System;
using System.Xml;
using System.Xml.Serialization;

/* FileCreator, version 1 */
namespace FileCreator
{
    public static class Names
    {
        public const string XmlNamespace = "http://ikriv.com/OverrideXml/FileCreator/v1";
    }

    public class Organization
    {
        public string Name { get; set;}
        public string Headquarters { get; set;}
    }

    [XmlRoot(Namespace=Names.XmlNamespace)]
    public class MyData
    {
        public DateTime CreationTime { get; set; }
        public Organization Organization { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string fileName = args.Length >= 1 ? args[0] : "file.xml";

            var data = new MyData
            {
                CreationTime = DateTime.UtcNow,
                Organization = new Organization { Name = "United Nations", Headquarters = "New York" }
            };

            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", Names.XmlNamespace);

            using (var writer = XmlWriter.Create(fileName, settings))
            {
                var serializer = new XmlSerializer(typeof(MyData)); // default namespace
                serializer.Serialize(writer, data, namespaces);
            }
        }
    }
}
