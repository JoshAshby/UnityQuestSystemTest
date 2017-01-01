using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    public class Choice
    {
        [XmlAttribute("id")]
        public string ID;

        public string Text;
        public string NextNodeID;
    }

    public class Node
    {
        [XmlAttribute("id")]
        public string ID;

        public string Text;
    }

    public class Dialogue
    {
        [XmlAttribute("id")]
        public string ID;

        [XmlArray("Nodes")]
        [XmlArrayItem("Node")]
        public List<Node> Nodes;

        [XmlArray("Choices")]
        [XmlArrayItem("Choice")]
        public List<Choice> Choices;
    }

    [XmlRoot("DialogueCollection")]
    public class DialogueContainer
    {
        [XmlArray("Dialogues")]
        [XmlArrayItem("Dialogue")]
        public List<Dialogue> Dialogues = new List<Dialogue>();

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