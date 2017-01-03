using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public abstract class IMetadata
        {
            [XmlAttribute("id")]
            public string ID = "";
        }

        public class BoolMetadata : IMetadata
        {
            public bool Value = false;
        }

        public class FloatMetadata : IMetadata
        {
            public float Value = 0f;
        }

        public class StringMetadata : IMetadata
        {
            public string Value = "";
        }
    }
}