using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public interface INode
        {
            string ID { get; set; }
            Dictionary<string, IMetadata> Metadata { get; set; }
        }

        public interface IBranchedNode
        {
            Dictionary<string, IBranch> Branches { get; set; }
        }

        public interface INextNode
        {
            string NextNodeID { get; set; }
        }

        public abstract class ANode : INode
        {
            private string _id = Guid.NewGuid().ToString();
            [XmlAttribute("id")]
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }

            private Dictionary<string, IMetadata> _metadata = new Dictionary<string, IMetadata>();
            [XmlIgnore]
            public Dictionary<string, IMetadata> Metadata
            {
                get { return _metadata; }
                set { _metadata = value; }
            }

            [XmlArray("Metadata")]
            [XmlArrayItem("BoolMetadata", typeof(BoolMetadata))]
            [XmlArrayItem("FloatMetadata", typeof(FloatMetadata))]
            [XmlArrayItem("StringMetadata", typeof(StringMetadata))]
            public IMetadata[] XmlMetadata
            {
                get { return Metadata.Values.ToArray(); }
                set { Metadata = value.ToDictionary(i => i.ID, i => i); }
            }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class CustomDataAttribute : Attribute { }

        public class TextNode : ANode, IBranchedNode
        {
            [CustomData]
            public string Text { get; set; }

            private Dictionary<string, IBranch> _branches = new Dictionary<string, IBranch>();
            [XmlIgnore]
            public Dictionary<string, IBranch> Branches
            {
                get { return _branches; }
                set { _branches = value; }
            }

            [XmlArray("Branches")]
            [XmlArrayItem("SimpleBranch", typeof(SimpleBranch))]
            public IBranch[] XmlBranches
            {
                get { return Branches.Values.ToArray(); }
                set { Branches = value.ToDictionary(i => i.ID, i => i); }
            }
        }

        public class WaitNode : ANode, INextNode
        {
            [CustomData]
            public float Seconds { get; set; }

            public string NextNodeID { get; set; }
        }

        public class EventNode : ANode, INextNode
        {
            [CustomData]
            public string Message { get; set; }

            public string NextNodeID { get; set; }
        }

        public class EndNode : ANode { }
    }
}