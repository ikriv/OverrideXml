using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Ikriv.Xml;

namespace ThirdPartySample
{
    class Program
    {
        private static void Serialize(object obj, XmlAttributeOverrides overrides)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "urn:unused");

            using (var s = new StringWriter())
            {
                var serializer = (overrides == null) 
                    ? new XmlSerializer(obj.GetType())
                    : new XmlSerializer(obj.GetType(), overrides);

                serializer.Serialize(s, obj, ns);
                Console.WriteLine(s);
            }
        }

        static void Main(string[] args)
        {
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
    
            Console.WriteLine("Standard AppDomainSetup serialization");
            Serialize(setup, null);

            var overrides = new OverrideXml()
                .Override<AppDomainSetup>()
                    .Member("ApplicationBase").XmlAttribute()
                    .Member("ConfigurationFile").XmlAttribute()
                    .Member("DisallowPublisherPolicy").XmlAttribute().XmlDefaultValue(false)
                    .Member("DisallowBindingRedirects").XmlAttribute().XmlDefaultValue(false)
                    .Member("DisallowCodeDownload").XmlAttribute().XmlDefaultValue(false)
                    .Member("DisallowApplicationBaseProbing").XmlAttribute().XmlDefaultValue(false)
                    .Member("ApplicationName").XmlAttribute()
                    .Member("LoaderOptimization").XmlAttribute().XmlDefaultValue(LoaderOptimization.NotSpecified)
                    .Member("SandboxInterop").XmlAttribute().XmlDefaultValue(false)
                .Commit();

            Console.WriteLine();
            Console.WriteLine("Custom AppDomainSetup serialization");
            Serialize(setup, overrides);
        }
    }
}
