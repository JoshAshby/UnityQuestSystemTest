using System;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public interface IBranch
        {
            string ID { get; set; }

            string Text { get; set; }
            string NextNodeID { get; set; }
        }

        public class SimpleBranch : IBranch
        {
            private string _id = Guid.NewGuid().ToString();
            [XmlAttribute("id")]
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }

            public string Text { get; set; }
            public string NextNodeID { get; set; }
        }
    }
}