using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConfigWrapper
{
    public class Wrapper
    {
        public string MulticastAddress { get; set; }
        public int MulticastPort { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public Wrapper XmlDeserializer()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Wrapper));
            using (TextReader reader = new StreamReader("config.xml"))
            {
                return (Wrapper)serializer.Deserialize(reader);
            }
        }
    }
}
