using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.Collections;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Utilities
{
    public class XmlHelper
    {
        public T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            T deserializedDtos =
                (T)xmlSerializer.Deserialize(reader);

            return deserializedDtos;
        }

        // This is syntax sugar method
        // May not be used
        public IEnumerable<T> DeserializeCollection<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(T[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            T[] supplierDtos =
                (T[])xmlSerializer.Deserialize(reader);

            return supplierDtos;
        }

        public string Serialize<T>(T obj, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new(typeof(T), xmlRoot);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new();

            using StringWriter sw = new StringWriter(sb);
            xmlSerializer.Serialize(sw, obj, namespaces);

            return sb.ToString().TrimEnd();
        }

        public string Serialize<T>(T[] obj, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new(typeof(T[]), xmlRoot);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new();

            using StringWriter sw = new StringWriter(sb);
            xmlSerializer.Serialize(sw, obj, namespaces);

            return sb.ToString().TrimEnd();
        }

        public string SerializeToXml<T>(T obj, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            if (obj is IEnumerable enumerable)
            {
                var listType = typeof(List<>).MakeGenericType(obj.GetType().GetGenericArguments()[0]);
                var list = (IList)Activator.CreateInstance(listType);

                foreach (var item in enumerable)
                {
                    list.Add(item);
                }

                obj = (T) list;
            }

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();

            using StringWriter sw = new StringWriter(sb);
            xmlSerializer.Serialize(sw, obj, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
