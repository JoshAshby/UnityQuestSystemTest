using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public interface IDialogue
        {
            string ID { get; set; }
            string FirstNodeID { get; set; }

            Dictionary<string, INode> Nodes { get; set; }
        }

        public class Dialogue : IDialogue
        {
            private string _id = Guid.NewGuid().ToString();
            [XmlAttribute("id")]
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }

            public string FirstNodeID { get; set; }

            private Dictionary<string, INode> _nodes = new Dictionary<string, INode>();
            [XmlIgnore]
            public Dictionary<string, INode> Nodes
            {
                get { return _nodes; }
                set { _nodes = value; }
            }

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