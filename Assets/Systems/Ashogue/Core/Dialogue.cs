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

            [XmlIgnore]
            public Dictionary<string, INode> Nodes = new Dictionary<string, INode>();

            [XmlArray("Nodes")]
            [XmlArrayItem("TextNode", typeof(TextNode))]
            [XmlArrayItem("WaitNode", typeof(WaitNode))]
            [XmlArrayItem("EventNode", typeof(EventNode))]
            [XmlArrayItem("EndNode", typeof(EndNode))]
            public INode[] XmlNodes
            {
                get { return Nodes.Values.ToArray(); }
                set { Nodes = value.ToDictionary(i => i.ID, i => i); }
            }
        }
    }
}