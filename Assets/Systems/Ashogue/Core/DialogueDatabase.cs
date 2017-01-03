using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    public abstract class IChoice
    {
        [XmlAttribute("id")]
        public string ID = "";

        public string Text = "";
        public string NextNodeID = "";
    }

    public abstract class IMetadata
    {
        [XmlAttribute("id")]
        public string ID = "";
    }

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

    public class Choice : IChoice { }

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

    [XmlRoot("DialogueCollection")]
    public class DialogueContainer
    {
        [XmlIgnore]
        public Dictionary<string, Dialogue> Dialogues = new Dictionary<string, Dialogue>();

        [XmlArray("Dialogues")]
        [XmlArrayItem("Dialogue", typeof(Dialogue))]
        public Dialogue[] XmlDialogues
        {
            get { return Dialogues.Values.ToArray(); }
            set { Dialogues = value.ToDictionary(i => i.ID, i => i); }
        }

        // Copypasta from http://wiki.unity3d.com/index.php?title=Saving_and_Loading_Data:_XmlSerializer
        // var monsterCollection = MonsterContainer.Load(Path.Combine(Application.dataPath, "monsters.xml"));
        // monsterCollection.Save(Path.Combine(Application.persistentDataPath, "monsters.xml"));
        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(DialogueContainer));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static DialogueContainer Load(string path)
        {
            var serializer = new XmlSerializer(typeof(DialogueContainer));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as DialogueContainer;
            }
        }

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public static DialogueContainer LoadFromText(string text)
        {
            var serializer = new XmlSerializer(typeof(DialogueContainer));
            return serializer.Deserialize(new StringReader(text)) as DialogueContainer;
        }
    }
}