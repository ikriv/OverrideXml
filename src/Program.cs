using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Ikriv.Xml;

namespace OverrideXmlSample
{
    public class Continent
    {
        public string Name { get; set; }
        public List<Country> Countries { get; set;}
    }

    public class Country
    {
        public string Name { get; set; }
        public string Capital { get; set; }
    }

    class Program
    {
        private static void Serialize(Continent continent, XmlAttributeOverrides overrides)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "urn:unused");

            using (var s = new StringWriter())
            {
                var serializer = (overrides == null) 
                    ? new XmlSerializer(typeof(Continent))
                    : new XmlSerializer(typeof(Continent), overrides);

                serializer.Serialize(s, continent, ns);
                Console.WriteLine(s);
            }
        }

        private static XmlAttributeOverrides GetOverrides()
        {
            return new OverrideXml()
                .Override<Continent>()
                    .XmlRoot("continent")
                    .Member("Name").XmlAttribute("name")
                    .Member("Countries").XmlElement("state")
                .Override<Country>()
                    .Member("Name").XmlAttribute("name")
                    .Member("Capital").XmlAttribute("capital")
                .Commit();
        }

        private static XmlAttributeOverrides GetOverridesRaw()
        {
            var overrides = new XmlAttributeOverrides();

            overrides.Add(typeof(Continent), new XmlAttributes { XmlRoot = new XmlRootAttribute("continent") });

            overrides.Add(typeof(Continent), "Name", 
                new XmlAttributes { XmlAttribute = new XmlAttributeAttribute("name")});

            overrides.Add(typeof(Continent), "Countries", 
                new XmlAttributes { XmlElements = { new XmlElementAttribute("state") } });

            overrides.Add(typeof(Country), "Name", 
                new XmlAttributes { XmlAttribute = new XmlAttributeAttribute("name")});

            overrides.Add(typeof(Country), "Capital", 
                new XmlAttributes { XmlAttribute = new XmlAttributeAttribute("capital")});

            return overrides;
        }

        static void Main(string[] args)
        {
            var europe = new Continent
            {
                Name = "Europe",
                Countries = new List<Country>
                {
                    new Country { Name = "France", Capital = "Paris" },
                    new Country { Name = "Germany", Capital = "Berlin" },
                    new Country { Name = "Spain", Capital = "Madrid" }
                }
            };

            Console.WriteLine("Default serialization");
            Serialize(europe, null);

            Console.WriteLine();
            Console.WriteLine("Serialization with overrides");
            Serialize(europe, GetOverrides());

            Console.WriteLine();
            Console.WriteLine("Serialization with raw overrides");
            Serialize(europe, GetOverridesRaw());

        }
    }
}
