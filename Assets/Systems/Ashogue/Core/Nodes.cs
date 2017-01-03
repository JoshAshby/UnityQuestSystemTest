using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public abstract class INode
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

        public abstract class IChoiceNode : INode
        {
            [XmlIgnore]
            public Dictionary<string, IChoice> Choices = new Dictionary<string, IChoice>();

            [XmlArray("Choices")]
            [XmlArrayItem("Choice", typeof(Choice))]
            public IChoice[] XmlChoices
            {
                get { return Choices.Values.ToArray(); }
                set { Choices = value.ToDictionary(i => i.ID, i => i); }
            }
        }

        public class TextNode : IChoiceNode
        {
            public string Text = "";
        }

        public class WaitNode : IChoiceNode
        {
            public float Seconds = 0f;
        }

        public class EventNode : IChoiceNode
        {
            public string Message = "";
        }

        public class EndNode : INode { }
    }
}