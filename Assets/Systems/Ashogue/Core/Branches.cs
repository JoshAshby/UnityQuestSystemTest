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

            string Name { get; set; }

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

            private string _name = "";
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            private string _text = "";
            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }

            private string _nextNodeID = "";
            public string NextNodeID
            {
                get { return _nextNodeID; }
                set { _nextNodeID = value; }
            }
        }
    }
}