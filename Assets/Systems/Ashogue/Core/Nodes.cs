using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public abstract class ANode
        {
            [XmlAttribute("id")]
            public string ID = "Untitled Node";

            [XmlIgnore]
            public Dictionary<string, IMetadata> Metadata = new Dictionary<string, IMetadata>();

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

        public abstract class AChainedNode : ANode
        {
            public string NextNodeID = "";
        }

        public abstract class ABranchedNode : ANode
        {
            [XmlIgnore]
            public Dictionary<string, IBranch> Branches = new Dictionary<string, IBranch>();

            [XmlArray("Branches")]
            [XmlArrayItem("SimpleBranch", typeof(SimpleBranch))]
            public IBranch[] XmlBranches
            {
                get { return Branches.Values.ToArray(); }
                set { Branches = value.ToDictionary(i => i.ID, i => i); }
            }
        }

        public class TextNode : ABranchedNode
        {
            public string Text = "";
        }

        public class WaitNode : AChainedNode
        {
            public float Seconds = 0f;
        }

        public class EventNode : AChainedNode
        {
            public string Message = "";
        }

        public class EndNode : ANode { }
    }
}