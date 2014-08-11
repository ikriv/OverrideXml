using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Ikriv.Xml;

/* FileCreator, version 2 */
namespace FileCreator
{
    public static class Names
    {
        public const string XmlNamespaceV1 = "http://ikriv.com/OverrideXml/FileCreator/v1";
        public const string XmlNamespace = "http://ikriv.com/OverrideXml/FileCreator/v2";
    }

    public class Organization
    {
        [XmlAttribute] public string Name { get; set;}
        [XmlAttribute] public string Headquarters { get; set;}
    }

    [XmlRoot(Namespace=Names.XmlNamespace)]
    public class MyData
    {
        [XmlAttribute] public DateTime CreationTime { get; set; }
        public List<Organization> Organizations { get; set; }
    }

    class Program
    {
        static XmlAttributeOverrides GetOverridesForV1()
        {
            return new OverrideXml()
                    .Override<MyData>()
                        .Attr(new XmlRootAttribute { Namespace = Names.XmlNamespaceV1 })
                        .Member("Organizations").XmlElement("Organization")
                    .Override<Organization>()
                        .Member("Name").XmlElement()
                        .Member("Headquarters").XmlElement()
                    .Commit();
        }

        static MyData GetData(string file)
        {
            if (file == null)
            {
                return new MyData 
                { 
                    CreationTime = DateTime.UtcNow, 
                    Organizations = new List<Organization>
                    {
                        new Organization { Name = "United Nations", Headquarters = "New York" },
                        new Organization { Name = "FIFA", Headquarters = "Zurich" }
                    }
                };
            }

            using (var reader = XmlReader.Create(file))
            {
                var serializer = new XmlSerializer(typeof(MyData));
                if (serializer.CanDeserialize(reader))
                {
                    return (MyData)serializer.Deserialize(reader);
                }

                var serializerV1 = new XmlSerializer(typeof(MyData), GetOverridesForV1());
                return (MyData)serializerV1.Deserialize(reader);
            }
        }

        static string GetInputFileName(string[] args)
        {
            if (args.Length >=2 ) return args[0];
            return null;
        }

        static string GetOutputFileName(string[] args)
        {
            if (args.Length >= 2) return args[1];
            if (args.Length == 1) return args[0];
            return "file.v2.xml";
        }

        static void Main(string[] args)
        {
            var data = GetData( GetInputFileName(args) );
            data.CreationTime = DateTime.UtcNow;

            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", Names.XmlNamespace);

            using (var writer = XmlWriter.Create(GetOutputFileName(args), settings))
            {
                var serializer = new XmlSerializer(typeof(MyData)); // default namespace
                serializer.Serialize(writer, data, namespaces);
            }
        }
    }
}
