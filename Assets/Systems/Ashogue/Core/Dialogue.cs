using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public class Dialogue
        {
            [XmlAttribute("id")]
            public string ID = "Untitled Dialogue";

            public string StartNode = "";

            [XmlIgnore]
            public Dictionary<string, ANode> Nodes = new Dictionary<string, ANode>();

            [XmlArray("Nodes")]
            [XmlArrayItem("TextNode", typeof(TextNode))]
            [XmlArrayItem("WaitNode", typeof(WaitNode))]
            [XmlArrayItem("EventNode", typeof(EventNode))]
            [XmlArrayItem("EndNode", typeof(EndNode))]
            public ANode[] XmlNodes
            {
                get { return Nodes.Values.ToArray(); }
                set { Nodes = value.ToDictionary(i => i.ID, i => i); }
            }
        }
    }
}