using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public abstract class IChoice
        {
            [XmlAttribute("id")]
            public string ID = "";

            public string Text = "";
            public string NextNodeID = "";
        }

        public class Choice : IChoice { }
    }
}