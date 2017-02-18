using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace TreeAshogue
{
    namespace Data
    {
        public interface IDialogue
        {
            string ID { get; set; }

            string Name { get; set; }
            string FirstNodeID { get; set; }

            Dictionary<string, INode> Nodes { get; set; }
            TNode AddNode<TNode>(string ID = null) where TNode : INode, new();
            INode AddNode(Type TNode, string ID = null);
            void RemoveNode(string ID);
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

            private string _name = "";
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            private string _firstNodeID = "";
            public string FirstNodeID
            {
                get { return _firstNodeID; }
                set { _firstNodeID = value; }
            }

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

            public TNode AddNode<TNode>(string ID = null) where TNode : INode, new()
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                TNode node = new TNode { ID = ID };
                Nodes.Add(ID, node);

                return node;
            }

            public INode AddNode(Type TNode, string ID = null)
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                INode node = (INode)Activator.CreateInstance(TNode);

                node.ID = ID;
                Nodes.Add(ID, node);

                return node;
            }

            public void RemoveNode(string ID)
            {
                Nodes.Remove(ID);
            }
        }
    }
}